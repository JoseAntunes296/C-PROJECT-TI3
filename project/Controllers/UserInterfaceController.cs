using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.WebControls;

namespace Project.Controllers
{
    public class UserInterfaceController : Controller
    {
        [HttpGet]
        public ActionResult UserProjectsDetails()
        {
            if (Session["IdUser"] == null)
            {
                Response.Redirect("/Home/Index");
            }
            else
            {
                using (var entity = new ProjectContext())
                {
                    int userId = (int)Session["IdUser"];
                    // Consulta para obter os projetos do usuário com base no ID
                    var userProjects = entity.projectAssignments
                        .Where(pa => pa.userId == userId)
                        .Select(pa => pa.project)
                        .ToList();

                    userProjects = userProjects.OrderBy(p => p.IdProject).ToList();


                    // Crie uma instância do modelo ViewModal e preencha-a com os dados do usuário
                    var viewModel = new ViewModal
                    {
                        UserProjects = userProjects
                    };

                    return View(viewModel);
                }
            }
            return View();
        }
    }
}