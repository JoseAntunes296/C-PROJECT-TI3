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
        public ActionResult Projects()
        {
            var dbContext = new ProjectContext();
            var projects = dbContext.projects.ToList();

            if (projects is null)
            {
                Exception exception = new Exception("A lista de projetos está vazia.");
                throw exception;
            }

            var loginModel = new LoginViewModel();
            var userId = loginModel.IdUser;

            var userProjects = dbContext.projectAssignments
                .Where(up => up.userId == userId)
                .Select(up => up.projectId)
                .ToList();

            // Filtra os projetos para remover aqueles em que o usuário já está associado
            projects = projects.Where(p => !userProjects.Contains(p.IdProject)).ToList();

            var viewModel = new ViewModal
            {
                LoginModel = loginModel,
                Projects = projects
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult GetProject(int projectId)
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
        [HttpPost]
        public ActionResult PickProject(int userId, int projectId)
        {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    // Cria uma nova instância de ProjectAssignment
                    var projectAssignment = new projectAssignment
                    {
                        userId = userId,
                        projectId = projectId,
                        status = "Active"
                    };

                    // Adiciona o objeto ProjectAssignment ao contexto do banco de dados
                    dbContext.projectAssignments.Add(projectAssignment);

                    // Salva as alterações no banco de dados
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }

            return View();
        }

    }
}