using Project.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;
using System;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Xml.Linq;

namespace Project.Controllers
{
    public class AccountsController : Controller
    {
        [HttpPost]
        public ActionResult Login(LoginViewModel credentials)
        {
            if (credentials.Email == "" || credentials.Password == "" || credentials.Email == null || credentials.Password == null)
            {
                ModelState.AddModelError("", "Username or Password is wrong");
            }
            else
            {
                using (var entity = new ProjectContext())
                {
                    byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(credentials.Password);
                    string encodedPassword = Convert.ToBase64String(passwordBytes);

                    bool userExist = entity.users.Any(x => x.email == credentials.Email && x.password == encodedPassword);
                    user u = entity.users.FirstOrDefault(x => x.email == credentials.Email && x.password == encodedPassword);

                    if (userExist)
                    {
                        FormsAuthentication.SetAuthCookie(u.email, false);

                        // Consulta ao banco de dados para obter o ID do usuário
                        var user = entity.users
                                .Where(x => x.email.Equals(credentials.Email) && x.password.Equals(encodedPassword))
                                .Select(x => new { x.IdUser, x.administrator })
                                .FirstOrDefault();

                        if (user != null)
                        {
                            credentials.IdUser = user.IdUser;
                            credentials.administrator = user.administrator;

                            return RedirectToAction("Index", "Home", credentials);
                        }
                    }
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult UpdateUser(HttpPostedFileBase image, int userId, string username, string email)
        {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    var user = dbContext.users.FirstOrDefault(x => x.IdUser == userId);

                    if (user != null)
                    {
                        if (image != null && image.ContentLength > 0)
                        {
                            string fileName = Path.GetFileName(image.FileName);
                            string path = Path.Combine(Server.MapPath("~/Content/UserImages/"), fileName);
                            image.SaveAs(path);

                            user.profileImg = fileName;
                        }

                        user.username = username;
                        user.email = email;

                        dbContext.SaveChanges();
                    }

                    // Retornar objeto JSON como resposta
                    return Json(user);
                }
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }


        [HttpGet]
        public ActionResult GetDataUser(int userId)
        {
            using (var entity = new ProjectContext())
            {
                var user = entity.users.FirstOrDefault(x => x.IdUser == userId);

                if (user != null)
                {
                    var userData = new { username = user.username, email = user.email, imageName = user.profileImg };
                    return Json(userData, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}