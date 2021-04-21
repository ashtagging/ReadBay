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

        [HttpGet]
        public IActionResult GetAll()
        {
            //Retrieve all the Products and return in a Json format(dataTables)
            // Include properties added so the category or Book Type can be displayed in the UI
            var allObj = _unitOfWork.Product.GetAll(includeProperties: "Category,BookType");
            return Json(new { data = allObj });
        }
    }
}

