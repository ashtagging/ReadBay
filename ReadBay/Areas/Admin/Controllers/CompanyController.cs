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
    public class CompanyController : Controller
    {
        // Need to get from dependency injection in startup.cs
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // Index View GET: Admin/Companies
        public IActionResult Index()
        {
            return View();
        }

        //GET: Admin/Companies/Upsert
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if (id == null)
            {
                //this is for create
                return View(company);
            }
            //this is for edit
            company = _unitOfWork.Company.Get(id.GetValueOrDefault());
            if (company == null)
            {
                return NotFound();
            }
            return View(company);

        }

        // Upsert Post Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            // Checks if all the validations in the model are valid or not eg name Required, max length 
            // Performs similiar functionality as validationScriptsPartial
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            //Retrieve all the Companies and return in a Json format (dataTables)
            var allObj = _unitOfWork.Company.GetAll();
            return Json(new { data = allObj });
        }

        // API Call for Delete called in Company.js
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Company.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            // If the object is not null we will delete
            _unitOfWork.Company.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }

        #endregion
    }
}
