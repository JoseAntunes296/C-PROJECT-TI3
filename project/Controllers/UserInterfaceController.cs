using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class UserInterfaceController : Controller
    {
        // GET: UserInterface
        public ActionResult UserProfile()
        {
            return View();
        }
        public ActionResult UserProjectsDetails()
        {
            return View();
        }
    }
}