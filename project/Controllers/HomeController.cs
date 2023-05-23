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
            string userEmail = User.Identity.Name;

            using (var entity = new ProjectContext())
            {
                var user = entity.users.FirstOrDefault(x => x.email == userEmail);

                if (user != null)
                {
                    int userId = user.IdUser;
                    int administrator = user.administrator;

                    // Use o ID do usuário e o status do administrador como necessário

                    // Exemplo de atribuição para a propriedade do ViewModel
                    var viewModel = new LoginViewModel
                    {
                        IdUser = userId,
                        Email = userEmail,
                        administrator = administrator
                    };

                    return View(viewModel);
                }
            }

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

                // Return a response indicating success or failure
                return Json(new { success = ModelState.IsValid });
            }
        }
    }
}
