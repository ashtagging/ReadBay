using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadBay.Models;
using ReadBay.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadBay.DataAccess.Data.Repository.IRepository;

namespace ReadBay.Areas.Admin.Controllers
{
    //Have to explicitly define that this is in the Admin Area
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        // Need to get from dependency injection in startup.cs
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // Index View GET: Admin/Categories
        public IActionResult Index()
        {
            return View();
        }

        //GET: Admin/Categories/Upsert
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id == null)
            {
                //this is for create
                return View(category);
            }
            //this is for edit
            category = _unitOfWork.Category.Get(id.GetValueOrDefault());
            if (category == null)
            {
                return NotFound();
            }
            return View(category);

        }

        // Upsert Post Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            // Checks if all the validations in the model are valid or not eg name Required, max length 
            // Performs similiar functionality as validationScriptsPartial
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    _unitOfWork.Category.Add(category);
                }
                else
                {
                    _unitOfWork.Category.Update(category);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            //Retrieve all the categories and return in a Json format (dataTables)
            var allObj = _unitOfWork.Category.GetAll();
            return Json(new { data = allObj });
        }

        // API Call for Delete called in Category.js
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Category.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            // If the object is not null we will delete
            _unitOfWork.Category.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }

        #endregion
    }
}
