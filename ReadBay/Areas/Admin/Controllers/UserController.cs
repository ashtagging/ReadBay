using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadBay.Models;
using ReadBay.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadBay.DataAccess.Data.Repository.IRepository;
using ReadBay.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace ReadBay.Areas.Admin.Controllers
{
    //Have to explicitly define that this is in the Admin Area
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    [Authorize(Roles = SD.Role_Employee)]
    public class UserController : Controller
    {
        // Need to get from dependency injection in startup.cs
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        // Index View GET: Admin/Categories
        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            //Retrieve all the categories and return in a Json format (dataTables)
            var userList = _db.ApplicationUsers.Include(u => u.Company).ToList();
            var userRole = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked, we will unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Operation Successful." });
        }
        #endregion
    }
}
