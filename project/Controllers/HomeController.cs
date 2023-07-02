using Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();

        }
        [HttpGet]
        public ActionResult UserProfile(int userId)
        {
            using (var db = new ProjectContext())
            {
                var userProfile = db.users.FirstOrDefault(u => u.IdUser == userId);

                if (userProfile == null)
                {
                    return HttpNotFound();
                }

                return Json(userProfile, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Edit(UserProfile userProfile)
        {
            using (var db = new ProjectContext())
            {
                if (ModelState.IsValid)
                {
                    var existingUserProfile = db.users.FirstOrDefault(u => u.IdUser == userProfile.IdUser);
                    if (existingUserProfile != null)
                    {
                        existingUserProfile.username = userProfile.username;
                        existingUserProfile.email = userProfile.email;

                        db.SaveChanges();
                    }
                }

                return Json(new { success = ModelState.IsValid });
            }
        }
    }
}
