using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Project.Controllers
{
    public class ProjectsAvailableController : Controller
    {
        // GET: ProjectsAvailable
        public ActionResult Projects()
        {
            var dbContext = new ProjectContext();
            var projects = dbContext.projects.Include(p => p.tasks).ToList();

            if (projects is null)
            {
                Exception exception = new Exception("A lista de projetos está vazia.");
                throw exception;
            }

            return View(projects);
        }
        [HttpPost]
        public ActionResult GetProject(int projectId) {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    var project = dbContext.projects.FirstOrDefault(p => p.IdProject == projectId);

                    if (project != null)
                    {
                        return Json(new
                        {
                            name = project.name,
                            description = project.description,
                            startDate = project.startDate,
                            endDate = project.endDate
                        });
                    }
                }

                return Json(new { error = "Projeto não encontrado" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}