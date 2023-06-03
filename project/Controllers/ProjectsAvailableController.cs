using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Globalization;
using System.Threading.Tasks;

namespace Project.Controllers
{
    public class ProjectsAvailableController : Controller
    {
        public ActionResult Projects()
        {
            try
            {
                var dbContext = new ProjectContext();
                var projects = dbContext.projects.ToList();

                if (projects.Count == 0)
                {
                    throw new Exception("A lista de projetos está vazia.");
                }

                var userId = Convert.ToInt32(Session["IdUser"]);

                var userProjects = dbContext.projectAssignments
                    .Where(pa => pa.userId == userId)
                    .Select(pa => pa.projectId)
                    .ToList();

                projects = projects.Where(p => !userProjects.Contains(p.IdProject)).ToList();

                var viewModel = new ViewModal
                {
                    Projects = projects
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<ActionResult> GetProject(int projectId)
        {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    var project = await dbContext.projects.FirstOrDefaultAsync(p => p.IdProject == projectId);

                    if (project != null)
                    {
                        DateTime currentDate = DateTime.Now;

                        var projectData = new
                        {
                            projectId = project.IdProject,
                            name = project.name,
                            description = project.description,
                            startDate = project.startDate.ToString("yyyy-MM-dd"),
                            endDate = project.endDate.ToString("yyyy-MM-dd"),
                            isCompleted = project.endDate < currentDate
                        };

                        return Json(projectData, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { error = "Projeto não encontrado" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> PickProject(int userId, int projectId)
        {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    var projectAssignment = new projectAssignment
                    {
                        userId = userId,
                        projectId = projectId,
                        status = "1"
                    };

                    dbContext.projectAssignments.Add(projectAssignment);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

            return Json(new { success = true });
        }
    }
}