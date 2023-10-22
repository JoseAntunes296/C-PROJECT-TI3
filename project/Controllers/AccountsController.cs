using Project.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;
using System;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Text;
using System.Data.Entity;

namespace Project.Controllers
{
    public class AccountsController : Controller
    {
        [HttpPost]
        public JsonResult ChangePassword(string Npassword, string Cpassword, int userId)
        {
            using (var dbContext = new ProjectContext())
            {
                try
                {
                    if (Npassword == Cpassword)
                    {
                        var user = dbContext.users.FirstOrDefault(p => p.IdUser == userId);

                        if (user != null)
                        {
                            string passHash = EncryptPassword(Npassword);
                            user.password = passHash;
                            dbContext.SaveChanges();
                            return Json(new { success = true, message = "Password Atualizada com sucesso." });
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Houve um erro ao atualizar a password" });
                }
            }
            return Json(new { success = false, error = "Ocorreu um erro interno" });
        }
        [HttpGet]
        private NetworkCredential GetNetworkCredentials()
        {
            string email = "spamaccot404@gmail.com";
            string password = "ifxglxtprupxpaov";

            return new NetworkCredential(email, password);
        }
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            using (var context = new ProjectContext())
            {
                // Verifique se o email existe na tabela "users"
                bool emailExists = context.users.Any(u => u.email == email);
                var user = context.users.FirstOrDefault(p => p.email == email);

                if (emailExists)
                {
                    string temporaryPassword = GenerateRandomPassword();
                    string passHash = EncryptPassword(temporaryPassword);

                    string emailHtml = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                      <meta charset='UTF-8'>
                      <title>Bem-vindo!</title>
                      <style>
                        /* Estilos CSS */
                        body {{
                          font-family: Arial, sans-serif;
                          background-color: #f1f1f1;
                          margin: 0;
                          padding: 0;
                        }}
                        .container {{
                          max-width: 600px;
                          margin: 0 auto;
                          padding: 20px;
                          background-color: #ffffff;
                          border-radius: 10px;
                          box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
                        }}
                        h1 {{
                          color: #333333;
                          text-align: center;
                        }}
                        p {{
                          color: #666666;
                          margin-bottom: 20px;
                        }}
                        .button {{
                          display: inline-block;
                          background-color: #4CAF50;
                          color: #ffffff;
                          padding: 10px 20px;
                          text-decoration: none;
                          border-radius: 4px;
                        }}
                      </style>
                    </head>
                    <body>
                      <div class='container'>
                        <h1>Alteração temporária efetuada!</h1>
                        <p>Caríssimo(a),</p>
                        <p>A sua password foi alterada, assim que efecturar o login, recomendamos que altere para uma sua.</p>
                        <p>Password temporária: {temporaryPassword}</p>
                        <p>Utilize o link para fazer login e explorar todas as funcionalidades disponíveis:</p>
                        <p style='text-align: center;'>
                          <a class='button' href='https://localhost:44382' style='color:white'>Aceda ao nosso site</a>
                        </p>
                        <p>Atenciosamente,</p>
                        <p>A equipa</p>
                      </div>
                    </body>
                    </html>
                ";

                    SendEmail(email, "Recuperação de Senha", emailHtml);

                    user.password = passHash;
                    context.SaveChanges();
                    return Json(new { success = true, message = "Um email de recuperação de password foi enviado para o email fornecido." });
                }
                else
                {
                    return Json(new { success = false, message = "O email fornecido não corresponde a nenhuma conta." });
                }
            }
        }
        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return password;
        }
        private void SendEmail(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient())
            {
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = GetNetworkCredentials();

                using (var message = new MailMessage(
                    from: new MailAddress("spamaccot404@gmail.com"),
                    to: new MailAddress(toEmail)
                ))
                {
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    client.Send(message);
                }
            }
        }
        [HttpPost]
        public ActionResult AddUser(string username, string email, int isAdmin, int status)
        {
            if (!string.IsNullOrEmpty(email))
            {
                try
                {
                    using (var dbContext = new ProjectContext())
                    {
                        string password = GenerateRandomPassword();
                        string token = GenerateUniqueToken(dbContext);

                        string emailHtml = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                      <meta charset='UTF-8'>
                      <title>Bem-vindo!</title>
                      <style>
                        /* Estilos CSS */
                        body {{
                          font-family: Arial, sans-serif;
                          background-color: #f1f1f1;
                          margin: 0;
                          padding: 0;
                        }}
                        .container {{
                          max-width: 600px;
                          margin: 0 auto;
                          padding: 20px;
                          background-color: #ffffff;
                          border-radius: 10px;
                          box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
                        }}
                        h1 {{
                          color: #333333;
                          text-align: center;
                        }}
                        p {{
                          color: #666666;
                          margin-bottom: 20px;
                        }}
                        .button {{
                          display: inline-block;
                          background-color: #4CAF50;
                          color: #ffffff;
                          padding: 10px 20px;
                          text-decoration: none;
                          border-radius: 4px;
                        }}
                      </style>
                    </head>
                    <body>
                      <div class='container'>
                        <h1>Bem-vindo!</h1>
                        <p>Caríssimo(a),</p>
                        <p>É com grande satisfação que lhe damos as boas-vindas ao nosso projeto em C# Web Application.</p>
                        <p>Para aceder à sua conta, utilize as seguintes informações de login:</p>
                        <p>Username: {username}</p>
                        <p>Password: {password}</p>
                        <p>Utilize o link para fazer login e explorar todas as funcionalidades disponíveis:</p>
                        <p style='text-align: center;'>
                          <a class='button' href='https://localhost:44382' style='color:white'>Aceda ao nosso site</a>
                        </p>
                        <p>Seja bem-vindo(a)</p>
                        <p>Atenciosamente,</p>
                        <p>A equipa</p>
                      </div>
                    </body>
                    </html>
                ";

                        SendEmail(email, "Bem-vindo", emailHtml);
                        string hashedPassword = EncryptPassword(password);

                        user newUser = new user
                        {
                            username = username,
                            email = email,
                            password = hashedPassword,
                            token = token,
                            administrator = isAdmin,
                            profileImg = "defaultUser.jpg",
                            userStatus = status
                        };

                        dbContext.users.Add(newUser);
                        dbContext.SaveChanges();
                    }

                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, error = ex.Message });
                }
            }
            return Json(new { success = false, error = "Email não fornecido" });
        }
        private string GenerateUniqueToken(ProjectContext dbContext)
        {
            string token;
            bool isTokenUnique;

            do
            {
                token = Guid.NewGuid().ToString();
                isTokenUnique = !dbContext.users.Any(u => u.token == token);
            } while (!isTokenUnique);

            return token;
        }
        [HttpPost]
        public JsonResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return Json(new { success = false, error = "Username or Password is wrong" });
            }
            else
            {
                using (var entity = new ProjectContext())
                {
                    byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                    string encodedPassword = Convert.ToBase64String(passwordBytes);

                    var user = entity.users.FirstOrDefault(x => x.email == email);

                    if (user == null)
                    {
                        return Json(new { success = false, error = "User does not exist" });
                    }
                    else if (user.password != encodedPassword)
                    {
                        return Json(new { success = false, error = "Incorrect password" });
                    }
                    else if (user.userStatus == 0)
                    {
                        return Json(new { success = false, error = "Account is inactive" });
                    }
                    else
                    {
                        var userDetails = entity.users
                            .Where(x => x.email.Equals(email) && x.password.Equals(encodedPassword))
                            .Select(x => new { x.IdUser, x.administrator })
                            .FirstOrDefault();

                        if (userDetails != null)
                        {
                            Session["IdUser"] = userDetails.IdUser;
                            Session["administrator"] = userDetails.administrator;

                            return Json(new { success = true });
                        }
                    }
                }
            }

            return Json(new { success = false, error = "An error occurred" });
        }
        private string EncryptPassword(string password)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            string encodedPassword = Convert.ToBase64String(passwordBytes);
            return encodedPassword;
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

                        var updatedUser = new
                        {
                            IdUser = user.IdUser,
                            Username = user.username,
                            Email = user.email,
                            ProfileImg = user.profileImg
                        };
                        return Json(new { success = true, user = updatedUser });
                    }
                }
                return Json(new { success = false, error = "User not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
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
            Session.Clear();
            return View();
        }
        [HttpPost]
        public ActionResult DeleteUser(int userId)
        {
            using (var dbcontext = new ProjectContext())
            {
                var user = dbcontext.users.FirstOrDefault(u => u.IdUser == userId);

                if (user != null)
                {
                    bool hasProjectAssignments = dbcontext.projectAssignments.Any(p => p.userId == userId);
                    bool hasTasks = dbcontext.tasks.Any(t => t.UserTaskId == userId);

                    if (hasProjectAssignments || hasTasks)
                    {
                        if (hasTasks)
                        {
                            var tasks = dbcontext.tasks.Where(t => t.UserTaskId == userId);
                            foreach (var task in tasks)
                            {
                                task.UserTaskId = null;
                            }
                        }
                        if (hasProjectAssignments)
                        {
                            var projectAssignments = dbcontext.projectAssignments.Where(pa => pa.userId == userId);
                            dbcontext.projectAssignments.RemoveRange(projectAssignments);
                        }
                        dbcontext.users.Remove(user);
                        dbcontext.SaveChanges();

                        return Json(new { success = true });
                    }
                    else
                    {
                        dbcontext.users.Remove(user);
                        dbcontext.SaveChanges();

                        return Json(new { success = true });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Utilizador não encontrado." });
                }
            }
        }
    }
}