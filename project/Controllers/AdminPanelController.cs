using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Project.Controllers
{
    public class AdminPanelController : Controller
    {
        [HttpGet]
        public ActionResult ManageProjects()
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
        public ActionResult ManageUsers()
        {
            return View();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateProject(string name, string description, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    var projectAdd = new project()
                    {
                        name = name,
                        description = description,
                        startDate = startDate,
                        endDate = endDate
                    };

                    dbContext.projects.Add(projectAdd);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                return View("Error", exception);
            }

            return View();
        }
        // Action para obter os dados do projeto
        [HttpPost]
        public JsonResult GetProject(int projectId)
        {
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

        // Action para obter os dados do projeto
        [HttpPost]
        public JsonResult GetTask(int taskId)
        {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    var task = dbContext.tasks.FirstOrDefault(p => p.IdTask == taskId);

                    if (task != null)
                    {
                        return Json(new
                        {
                            name = task.name,
                            description = task.description,
                            deadline = task.deadline,
                            status = task.status
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

        [HttpPost]
        public JsonResult UpdateProject(int projectId, string name, string description, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    var project = dbContext.projects.FirstOrDefault(p => p.IdProject == projectId);

                    if (project != null)
                    {
                        // Atualiza as propriedades do projeto com base nos dados recebidos
                        project.name = name;
                        project.description = description;
                        project.startDate = startDate;
                        project.endDate = endDate;

                        // Salva as alterações no banco de dados
                        dbContext.SaveChanges();

                        return Json(new { success = true });
                    }
                }

                return Json(new { error = "Projeto não encontrado" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        [HttpPost]
        public ActionResult UpdateTask(int taskId, string name, string description, DateTime deadline, int status)
        {
            using (var dbContext = new ProjectContext())
            {
                var task = dbContext.tasks.FirstOrDefault(t => t.IdTask == taskId);

                if (task != null)
                {
                    // Atualiza as propriedades da tarefa com base nos dados recebidos
                    task.name = name;
                    task.description = description;
                    task.deadline = deadline;
                    task.status = status == 1 ? "1" : "0";

                    // Salva as alterações no banco de dados
                    dbContext.SaveChanges();

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

    }
}