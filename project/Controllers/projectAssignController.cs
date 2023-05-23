using Project.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class ProjectAssignController : Controller
    {
        // GET: projectAssign
        [HttpGet]
        public ActionResult ProgAssign()
        {
            using (var dbContext = new ProjectContext())
            {
                var project = dbContext.projects.ToList();
                if (project is null)
                {
                    Exception exception = new Exception("coise");
                    throw exception;
                }
                return View(project);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetProjectByID(int projectID)
        {
            var dbContext = new ProjectContext();
            dbContext.Configuration.LazyLoadingEnabled = false;

            var task = await dbContext.projects
                .Include(x => x.projectAssignments)
                .FirstOrDefaultAsync(x => x.IdProject.Equals(projectID));

            if (task is null)
                return Json("Erro ao obter o projeto.", JsonRequestBehavior.AllowGet);

            return Json(task, JsonRequestBehavior.AllowGet);
        }
    }
}