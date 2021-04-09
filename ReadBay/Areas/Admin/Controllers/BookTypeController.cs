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
    [Authorize(Roles = SD.Role_Admin)]
    public class BookTypeController : Controller
    {
        // Need to get from dependency injection in startup.cs
        private readonly IUnitOfWork _unitOfWork;

        public BookTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // Index View GET: Admin/BookType
        public IActionResult Index()
        {
            return View();
        }

        //GET: Admin/BookType/Upsert
        public IActionResult Upsert(int? id)
        {
            BookType bookType = new BookType();
            if (id == null)
            {
                //this is for create
                return View(bookType);
            }
            //this is for edit
            bookType = _unitOfWork.BookType.Get(id.GetValueOrDefault());
            if (bookType == null)
            {
                return NotFound();
            }
            return View(bookType);

        }

        // Upsert Post Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(BookType bookType)
        {
            // Checks if all the validations in the model are valid or not eg name Required, max length 
            // Performs similiar functionality as validationScriptsPartial
            if (ModelState.IsValid)
            {
                if (bookType.Id == 0)
                {
                    _unitOfWork.BookType.Add(bookType);
                }
                else
                {
                    _unitOfWork.BookType.Update(bookType);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(bookType);
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            //Retrieve all the BookTypes and return in a Json format (dataTables)
            var allObj = _unitOfWork.BookType.GetAll();
            return Json(new { data = allObj });
        }

        // API Call for Delete called in BookType.js
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.BookType.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            // If the object is not null we will delete
            _unitOfWork.BookType.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }

        #endregion
    }
}
