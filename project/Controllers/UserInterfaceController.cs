using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.WebControls;
using System.Data.Entity;

namespace Project.Controllers
{
    public class UserInterfaceController : Controller
    {
        [HttpGet]
        public ActionResult UserProjectsDetails()
        {
            if (Session["IdUser"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                using (var entity = new ProjectContext())
                {
                    int userId = (int)Session["IdUser"];

                    // Consulta para obter os projetos do usuário com base no ID
                    var userProjects = entity.projects
                        .Where(p => p.projectAssignments.Any(pa => pa.userId == userId))
                        .OrderBy(p => p.IdProject)
                        .Include(p => p.tasks)
                        .ToList();
                    return View(userProjects);
                }
            }
        }
    }
}