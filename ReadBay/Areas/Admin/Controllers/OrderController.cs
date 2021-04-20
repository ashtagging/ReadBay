using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ReadBay.DataAccess.Repository.IRepository;
using ReadBay.Models;
using ReadBay.Models.ViewModels;
using ReadBay.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace ReadBay.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public OrderDetailVM OrderVM { get; set; }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            OrderVM = new OrderDetailVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == id, includeProperties: "Product")
            };
            return View(OrderVM);
        }

        // Order Summary Details
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Details")]
        public IActionResult Details(string stripeToken)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id,includeProperties: "ApplicationUser");
            if (stripeToken != null)
            {
                // process the payment
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Description = "Order ID : " + orderHeader.Id,
                    Source = stripeToken
                };

                var service = new ChargeService();
                Charge charge = service.Create(options);

                if (charge.Id == null)
                {
                    orderHeader.PaymentStatus = SD.PaymentStatusRejected;
                }
                else
                {
                    orderHeader.TransactionId = charge.Id;
                }

                if (charge.Status.ToLower() == "succeeded")
                {
                    orderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    orderHeader.PaymentDate = DateTime.Now;
                }
                _unitOfWork.Save();
            }
            return RedirectToAction("Details", "Order", new {id = orderHeader.Id });
        }

        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ProcessingOrder(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            orderHeader.OrderStatus = SD.StatusInProcess;
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        // Cancel order and refund using built in Stripe classes
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            if (orderHeader.PaymentStatus == SD.StatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
                    Reason = RefundReasons.RequestedByCustomer,
                    Charge = orderHeader.TransactionId

                };
                var service = new RefundService();
                Refund refund = service.Create(options);

                orderHeader.OrderStatus = SD.StatusRefunded;
                orderHeader.PaymentStatus = SD.StatusRefunded;
            }
            else
            {
                orderHeader.OrderStatus = SD.StatusCancelled;
                orderHeader.PaymentStatus = SD.StatusCancelled;
            }

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        //public IActionResult UpdateOrderDetails()
        //{
        //    var orderHEaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
        //    orderHEaderFromDb.Name = OrderVM.OrderHeader.Name;
        //    orderHEaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
        //    orderHEaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
        //    orderHEaderFromDb.City = OrderVM.OrderHeader.City;
        //    orderHEaderFromDb.County = OrderVM.OrderHeader.County;
        //    orderHEaderFromDb.PostCode = OrderVM.OrderHeader.PostCode;
        //    if (OrderVM.OrderHeader.Carrier != null)
        //    {
        //        orderHEaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
        //    }
        //    if (OrderVM.OrderHeader.TrackingNumber != null)
        //    {
        //        orderHEaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
        //    }

        //    _unitOfWork.Save();
        //    TempData["Error"] = "Order Details Updated Successfully.";
        //    return RedirectToAction("Details", "Order", new { id = orderHEaderFromDb.Id });
        //}


        // API CALLS
        [HttpGet]
        public IActionResult GetOrderList(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaderList;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                orderHeaderList = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            }

            // Modifies what is shown based on filter selected on Order
            switch (status)
            {
                case "pending":
                    orderHeaderList = orderHeaderList.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaderList = orderHeaderList.Where(u => u.OrderStatus == SD.StatusApproved || u.OrderStatus == SD.StatusInProcess || u.OrderStatus == SD.StatusPending);
                    break;
                case "completed":
                    orderHeaderList = orderHeaderList.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "rejected":
                    orderHeaderList = orderHeaderList.Where(u => u.OrderStatus == SD.StatusCancelled || u.OrderStatus == SD.StatusRefunded || u.OrderStatus == SD.PaymentStatusRejected);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaderList });
        }        
    }
}

