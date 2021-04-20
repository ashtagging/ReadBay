using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReadBay.DataAccess.Repository.IRepository;
using ReadBay.Models;
using ReadBay.Models.ViewModels;
using ReadBay.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ReadBay.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class SearchController : Controller
    {
        // Need to get from dependency injection in startup.cs
        private readonly IUnitOfWork _unitOfWork;

        // Need this to be able to upload images to the server into a folder
        private readonly IWebHostEnvironment _hostEnvironment;

        public SearchController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        // Index View GET: Admin/Product
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var productFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,BookType");
            ShoppingCart cartObj = new ShoppingCart()
            {
                Product = productFromDb,
                ProductId = productFromDb.Id
            };
            return View(cartObj);
        }

    //GET: Admin/Product/Upsert
        public IActionResult Upsert(int? id)
        {
            // View Model added to hold product object and incorporate drop downs for Category & Book Type
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                BookTypeList = _unitOfWork.BookType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            if (id == null)
            {
                //this is for create
                return View(productVM);
            }
            //this is for edit
            productVM.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());
            if (productVM.Product == null)
            {
                return NotFound();
            }
            return View(productVM);

        }

        //HTTP Post Request for ShoppingCart in Details
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart CartObject)
        {
            CartObject.Id = 0;
            if (ModelState.IsValid)
            {
                //Add to cart
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObject.ApplicationUserId = claim.Value;

                // Rerieve Shopping cart from Db based on userId & productId
                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == CartObject.ApplicationUserId && u.ProductId == CartObject.ProductId, includeProperties: "Product");
                if (cartFromDb == null)
                {
                    // If no records exist in database for that product for that user
                    _unitOfWork.ShoppingCart.Add(CartObject);
                }
                else
                {
                    cartFromDb.Count += CartObject.Count;
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                }
                _unitOfWork.Save();

                //In session we will store number of items in the shopping cart
                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == CartObject.ApplicationUserId).ToList().Count();

                //ssShoppingCart = shoppingCart Session                              
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productFromDb = _unitOfWork.Product.
                        GetFirstOrDefault(u => u.Id == CartObject.ProductId, includeProperties: "Category,BookType");
                ShoppingCart cartObj = new ShoppingCart()
                {
                    Product = productFromDb,
                    ProductId = productFromDb.Id
                };
                return View(cartObj);
            }
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            //Retrieve all the Products and return in a Json format (dataTables)
            // Include properties added so the category or Book Type can be displayed in the UI
            var allObj = _unitOfWork.Product.GetAll(includeProperties: "Category,BookType");
            return Json(new { data = allObj });
        }

        // API Call for Delete called in Product.js
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Product.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            //Deletes the Image from the image folder
            string webRootPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, objFromDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            // If the object is not null we will delete
            _unitOfWork.Product.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }

        #endregion
    }
}

