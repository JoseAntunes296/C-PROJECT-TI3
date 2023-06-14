using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.IO;

//for pdf
using iTextSharp.text;
using iTextSharp.text.pdf;
//for word
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using System.Diagnostics;

//for excel
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace Project.Controllers
{
    public class AdminPanelController : Controller
    {
        [HttpGet]
        public void GenerateUsersExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            List<user> users;
            using (var dbContext = new ProjectContext())
            {
                users = dbContext.users.ToList();
            }

            string folderPath = Server.MapPath("~/Content/EXCELs/");
            Directory.CreateDirectory(folderPath);
            string fileName = "Users.xlsx";
            string filePath = Path.Combine(folderPath, fileName);

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet usersWorksheet = excelPackage.Workbook.Worksheets.Add("Users");

                usersWorksheet.Cells[1, 1].Value = "ID";
                usersWorksheet.Cells[1, 2].Value = "Username";
                usersWorksheet.Cells[1, 3].Value = "Imagem de Perfil";
                usersWorksheet.Cells[1, 4].Value = "Email";
                usersWorksheet.Cells[1, 5].Value = "Nível de Administração";
                usersWorksheet.Cells[1, 6].Value = "Token";
                usersWorksheet.Cells[1, 7].Value = "Estado";

                for (int i = 0; i < users.Count; i++)
                {
                    usersWorksheet.Cells[i + 2, 1].Value = users[i].IdUser;
                    usersWorksheet.Cells[i + 2, 2].Value = users[i].username;
                    usersWorksheet.Cells[i + 2, 3].Value = users[i].profileImg;
                    usersWorksheet.Cells[i + 2, 4].Value = users[i].email;
                    usersWorksheet.Cells[i + 2, 5].Value = users[i].administrator;
                    usersWorksheet.Cells[i + 2, 6].Value = users[i].token;
                    usersWorksheet.Cells[i + 2, 7].Value = users[i].userStatus;
                }
                usersWorksheet.Cells.AutoFitColumns();
                excelPackage.SaveAs(new FileInfo(filePath));
            }
            System.Diagnostics.Process.Start(filePath);
        }
        [HttpGet]
        public void GenerateProjectTasksExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            List<project> projetos;
            using (var dbContext = new ProjectContext())
            {
                projetos = dbContext.projects.Include(p => p.tasks).ToList();
            }

            // Criar um novo arquivo Excel
            string folderPath = Server.MapPath("~/Content/EXCELs/");
            Directory.CreateDirectory(folderPath);
            string fileName = "ProjectsTasks.xlsx";
            string filePath = Path.Combine(folderPath, fileName);

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                // Criar a planilha de projetos
                ExcelWorksheet projectsWorksheet = excelPackage.Workbook.Worksheets.Add("Projects");

                // Definir os títulos das colunas da tabela de projetos
                projectsWorksheet.Cells[1, 1].Value = "ID";
                projectsWorksheet.Cells[1, 2].Value = "Projeto";
                projectsWorksheet.Cells[1, 3].Value = "Descrição";
                projectsWorksheet.Cells[1, 4].Value = "Data de Início";
                projectsWorksheet.Cells[1, 5].Value = "Data de Fim";

                // Preencher os dados dos projetos
                for (int i = 0; i < projetos.Count; i++)
                {
                    projectsWorksheet.Cells[i + 2, 1].Value = projetos[i].IdProject;
                    projectsWorksheet.Cells[i + 2, 2].Value = projetos[i].name;
                    projectsWorksheet.Cells[i + 2, 3].Value = projetos[i].description;
                    projectsWorksheet.Cells[i + 2, 4].Value = projetos[i].startDate;
                    projectsWorksheet.Cells[i + 2, 5].Value = projetos[i].endDate;
                }

                // Ajustar automaticamente a largura das colunas da tabela de projetos
                projectsWorksheet.Cells.AutoFitColumns();

                // Criar a planilha de tarefas
                ExcelWorksheet tasksWorksheet = excelPackage.Workbook.Worksheets.Add("Tasks");

                // Definir os títulos das colunas da tabela de tarefas
                tasksWorksheet.Cells[1, 1].Value = "Id Projeto";
                tasksWorksheet.Cells[1, 2].Value = "Projeto";
                tasksWorksheet.Cells[1, 3].Value = "Tarefa";
                tasksWorksheet.Cells[1, 4].Value = "Estado";
                tasksWorksheet.Cells[1, 5].Value = "Descrição";
                tasksWorksheet.Cells[1, 6].Value = "Data de Início";
                tasksWorksheet.Cells[1, 7].Value = "Data de Fim";


                // Preencher os dados das tarefas
                int rowIndex = 2;
                foreach (var projeto in projetos)
                {
                    foreach (var tarefa in projeto.tasks)
                    {
                        tasksWorksheet.Cells[rowIndex, 1].Value = projeto.IdProject;
                        tasksWorksheet.Cells[rowIndex, 2].Value = projeto.name;
                        tasksWorksheet.Cells[rowIndex, 3].Value = tarefa.name;
                        tasksWorksheet.Cells[rowIndex, 4].Value = tarefa.status;
                        tasksWorksheet.Cells[rowIndex, 5].Value = tarefa.description;
                        tasksWorksheet.Cells[rowIndex, 6].Value = "-";
                        tasksWorksheet.Cells[rowIndex, 7].Value = tarefa.deadline.ToString();

                        tasksWorksheet.Cells[rowIndex, 2].Hyperlink = new ExcelHyperLink("Projects!A" + (projetos.IndexOf(projeto) + 2), projeto.name);

                        rowIndex++;
                    }
                }

                // Ajustar automaticamente a largura das colunas da tabela de tarefas
                tasksWorksheet.Cells.AutoFitColumns();

                // Salvar o arquivo Excel
                excelPackage.SaveAs(new FileInfo(filePath));
            }

            // Abrir o arquivo com o programa associado aos arquivos .xlsx
            System.Diagnostics.Process.Start(filePath);
        }
        [HttpGet]
        public ActionResult GenerateProjectsTasksPDF()
        {
            using (var dbContext = new ProjectContext())
            {
                var projects = dbContext.projects.ToList();
                var tasks = dbContext.tasks.ToList();

                using (MemoryStream stream = new MemoryStream())
                {
                    iTextSharp.text.Document document = new iTextSharp.text.Document();
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);
                    document.Open();

                    foreach (var project in projects)
                    {
                        iTextSharp.text.Paragraph projectNameParagraph = new iTextSharp.text.Paragraph("Projeto: " + project.name);
                        document.Add(projectNameParagraph);

                        iTextSharp.text.Paragraph projectDescriptionParagraph = new iTextSharp.text.Paragraph("Descrição: " + project.description);
                        document.Add(projectDescriptionParagraph);

                        iTextSharp.text.Paragraph projectStartDateParagraph = new iTextSharp.text.Paragraph("Data de Início: " + project.startDate.ToString("dd/MM/yyyy"));
                        document.Add(projectStartDateParagraph);

                        iTextSharp.text.Paragraph projectEndDateParagraph = new iTextSharp.text.Paragraph("Data de Término: " + project.endDate.ToString("dd/MM/yyyy"));
                        document.Add(projectEndDateParagraph);

                        document.Add(new iTextSharp.text.Paragraph("\n"));

                        var projectTasks = tasks.Where(t => t.projectId == project.IdProject);
                        foreach (var task in projectTasks)
                        {
                            document.Add(new iTextSharp.text.Paragraph("Tarefa: " + task.name));

                            iTextSharp.text.Paragraph taskDescriptionParagraph = new iTextSharp.text.Paragraph("Descrição: " + task.description);
                            document.Add(taskDescriptionParagraph);

                            iTextSharp.text.Paragraph taskDeadlineParagraph = new iTextSharp.text.Paragraph("Prazo: " + task.deadline.ToString("dd/MM/yyyy"));
                            document.Add(taskDeadlineParagraph);

                            iTextSharp.text.Paragraph taskStatusParagraph = new iTextSharp.text.Paragraph("Estado: " + task.status);
                            document.Add(taskStatusParagraph);

                            document.Add(new iTextSharp.text.Paragraph("\n"));
                        }

                        document.Add(new iTextSharp.text.Paragraph("\n"));
                    }

                    document.Close();

                    string folderPath = Server.MapPath("~/Content/PDFs/");
                    Directory.CreateDirectory(folderPath);
                    string fileName = "ProjectsTasks.pdf";
                    string filePath = Path.Combine(folderPath, fileName);
                    System.IO.File.WriteAllBytes(filePath, stream.ToArray());

                    string fileUrl = Url.Content("~/Content/PDFs/ProjectsTasks.pdf");
                    return Json(new { url = fileUrl }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult GenerateUsersPDF()
        {
            using (var dbContext = new ProjectContext())
            {
                var users = dbContext.users.ToList();
                using (MemoryStream stream = new MemoryStream())
                {
                    iTextSharp.text.Document document = new iTextSharp.text.Document();
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);
                    document.Open();
                    foreach (var user in users)
                    {
                        iTextSharp.text.Paragraph UserNameParagraph = new iTextSharp.text.Paragraph("Username: " + user.username);
                        document.Add(UserNameParagraph);
                        iTextSharp.text.Paragraph UserEmailParagraph = new iTextSharp.text.Paragraph("Email: " + user.email);
                        document.Add(UserEmailParagraph);
                        iTextSharp.text.Paragraph UserProfileIMGParagraph = new iTextSharp.text.Paragraph("Imagem de Perfil: " + user.profileImg);
                        document.Add(UserProfileIMGParagraph);
                        iTextSharp.text.Paragraph UserAdministrationParagraph = new iTextSharp.text.Paragraph("Nível de Administração: " + user.administrator);
                        document.Add(UserAdministrationParagraph);
                        iTextSharp.text.Paragraph UserTokenParagraph = new iTextSharp.text.Paragraph("Token: " + user.token);
                        document.Add(UserTokenParagraph);
                        iTextSharp.text.Paragraph UserStatusParagraph = new iTextSharp.text.Paragraph("Estado: " + user.userStatus);
                        document.Add(UserStatusParagraph);
                    }
                    document.Close();

                    string folderPath = Server.MapPath("~/Content/PDFs/");
                    Directory.CreateDirectory(folderPath);
                    string fileName = "User.pdf";
                    string filePath = Path.Combine(folderPath, fileName);
                    System.IO.File.WriteAllBytes(filePath, stream.ToArray());

                    string fileUrl = Url.Content("~/Content/PDFs/User.pdf");
                    return Json(new { url = fileUrl }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public void GenerateUsersWord()
        {
            List<user> users;
            using (var dbContext = new ProjectContext())
            {
                users = dbContext.users.ToList();
            }
            string folderPath = Server.MapPath("~/Content/WORDs/");
            Directory.CreateDirectory(folderPath);
            string fileName = "UsersWord.docx";
            string filePath = Path.Combine(folderPath, fileName);

            using (WordprocessingDocument document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                Body body = mainPart.Document.AppendChild(new Body());

                foreach (var user in users)
                {
                    DocumentFormat.OpenXml.Wordprocessing.Paragraph paragraph = body.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph());

                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new Text("Username: " + user.username)));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break()));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new Text("Email: " + user.email)));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break()));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new Text("Imagem de Perfil: " + user.profileImg)));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break()));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new Text("Nível de Administração: " + user.administrator)));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break()));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new Text("Token: " + user.token)));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break()));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new Text("Estado: " + user.userStatus)));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break()));

                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new Text("\n")));
                }
                document.Save();
            }
            Process.Start(filePath);
        }
        public void GenerateProjectsTasksWord()
        {
            List<project> projetos;
            using (var dbContext = new ProjectContext())
            {
                projetos = dbContext.projects.Include(p => p.tasks).ToList();
            }

            string folderPath = Server.MapPath("~/Content/WORDs/");
            Directory.CreateDirectory(folderPath);
            string fileName = "ProjectsTasks.docx";
            string filePath = Path.Combine(folderPath, fileName);

            using (WordprocessingDocument document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = document.AddMainDocumentPart();
                mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                Body body = mainPart.Document.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Body());

                foreach (var projeto in projetos)
                {
                    DocumentFormat.OpenXml.Wordprocessing.Paragraph paragraph = body.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph());
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Projeto: " + projeto.name)));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break()));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Descrição: " + projeto.description)));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break()));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Data do Começo: " + projeto.startDate)));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break()));
                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Data de Conclusão: " + projeto.endDate)));

                    foreach (var tarefa in projeto.tasks)
                    {
                        paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Tarefa: " + tarefa.name)));
                        paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break()));
                        paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Descrição: " + tarefa.description)));
                        paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break()));
                        paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Data de Conclusão: " + tarefa.deadline)));
                        paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break()));
                        paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Text("Estado: " + tarefa.status)));
                    }

                    paragraph.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run(new DocumentFormat.OpenXml.Wordprocessing.Break()));
                }

                document.Save();
            }
            Process.Start(filePath);
        }
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
            var dbContext = new ProjectContext();
            var users = dbContext.users.ToList();

            if (users is null)
            {
                Exception exception = new Exception("A lista de utilizadores está vazia.");
                throw exception;
            }

            return View(users);
        }
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult CreateProject(string name, string description, DateTime startDate, DateTime endDate)
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
                return Json(new { success = false, message = "Erro ao criar o projeto.", exceptionMessage = exception.Message });
            }

            return Json(new { success = true, message = "Projeto criado com sucesso." });
        }
        //for manageProjects
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
        [HttpGet]
        public async Task<ActionResult> GetProjects()
        {
            using (var dbContext = new ProjectContext())
            {
                var projects = await dbContext.projects
                .Include(p => p.tasks)
                .Include(p => p.projectAssignments)
                .ToListAsync();

                var projectModels = projects.Select(p => new project
                {
                    IdProject = p.IdProject,
                    name = p.name
                });

                return Json(projectModels, JsonRequestBehavior.AllowGet);
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
                        project.name = name;
                        project.description = description;
                        project.startDate = startDate;
                        project.endDate = endDate;

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
        public JsonResult DeleteProject(int projectId)
        {
            using (var dbContext = new ProjectContext())
            {
                var project = dbContext.projects.FirstOrDefault(p => p.IdProject == projectId);

                if (project != null)
                {
                    var tasks = dbContext.tasks.Where(t => t.projectId == projectId);
                    foreach (var task in tasks)
                    {
                        task.UserTaskId = null;
                        dbContext.tasks.Remove(task);
                    }
                    dbContext.Database.ExecuteSqlCommand("ALTER TABLE projectAssignments NOCHECK CONSTRAINT ALL");

                    var projectAssignments = dbContext.projectAssignments.Where(pa => pa.projectId == projectId);
                    dbContext.projectAssignments.RemoveRange(projectAssignments);

                    dbContext.projects.Remove(project);

                    dbContext.Database.ExecuteSqlCommand("ALTER TABLE projectAssignments CHECK CONSTRAINT ALL");

                    dbContext.SaveChanges();

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Projeto não encontrado." });
                }
            }
        }
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
        public JsonResult SaveTask(int projectId, string taskName, string taskDetails, DateTime taskEndDate, string status)
        {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    var project = dbContext.projects.FirstOrDefault(p => p.IdProject == projectId);
                    if (project == null)
                    {
                        return Json(new { success = false, error = "Projeto não encontrado." });
                    }

                    // Verificar se a data de término da tarefa está dentro do intervalo de datas do projeto
                    if (!validateDates(project.startDate, project.endDate, taskEndDate))
                    {
                        return Json(new { success = false, error = "A data de término da tarefa está fora do intervalo de datas do projeto." });
                    }
                    else
                    {
                        var task = new task()
                        {
                            name = taskName,
                            projectId = projectId,
                            description = taskDetails,
                            deadline = taskEndDate,
                            status = status
                        };

                        dbContext.tasks.Add(task);
                        dbContext.SaveChanges();
                        return Json(new { success = true });
                    }

                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
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
        [HttpPost]
        public JsonResult DeleteTask(int taskId)
        {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    var task = dbContext.tasks.FirstOrDefault(t => t.IdTask == taskId);

                    if (task != null)
                    {
                        task.UserTaskId = null;
                        task.project = null;

                        dbContext.tasks.Remove(task);
                        dbContext.SaveChanges();

                        return Json(new { success = true });
                    }
                }
            }
            catch (Exception exception)
            {
                return Json(new { success = false, error = exception.Message });
            }

            return Json(new { success = false, error = "Tarefa não encontrada." });
        }
        [HttpPost]
        public ActionResult GetUser(int userId)
        {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    var user = dbContext.users.FirstOrDefault(p => p.IdUser == userId);

                    if (user != null)
                    {
                        return Json(new
                        {
                            username = user.username,
                            email = user.email,
                            administrador = user.administrator,
                            status = user.userStatus
                        });
                    }
                }

                return Json(new { error = "Utilizador não encontrado" });
            }
            catch (Exception exception)
            {
                return View("Error", exception);
            }

        }
        [HttpPost]
        public ActionResult UpdateUser(int userId, string email, int administrador, int status)
        {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    var user = dbContext.users.FirstOrDefault(p => p.IdUser == userId);

                    if (user != null)
                    {
                        user.email = email;
                        user.administrator = administrador;
                        user.userStatus = status;

                        dbContext.SaveChanges();

                        return Json(new { success = true });
                    }
                }
                return Json(new { error = "Utilizador não encontrado" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult abandonProjeto(int projectId, int userId)
        {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    var projeto = dbContext.projects.Include(p => p.projectAssignments)
                        .FirstOrDefault(p => p.IdProject == projectId && p.projectAssignments.Any(a => a.userId == userId));

                    if (projeto == null)
                    {
                        return Json(new { success = false, message = "Projeto não encontrado ou permissão negada." });
                    }

                    var projectAssignment = projeto.projectAssignments.FirstOrDefault(a => a.userId == userId && a.projectId == projectId);
                    if (projectAssignment != null)
                    {
                        dbContext.projectAssignments.Remove(projectAssignment);

                        foreach (var task in projeto.tasks)
                        {
                            task.UserTaskId = null;
                        }

                        dbContext.SaveChanges();
                    }
                    else
                    {
                        return Json(new { success = false, message = "Projeto não encontrado ou permissão negada." });
                    }

                    return Json(new { success = true, message = "Projeto abandonado com sucesso." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao abandonar o projeto." });
            }
        }
        [HttpPost]
        public JsonResult GetUserTask(int taskId)
        {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    var task = dbContext.tasks.FirstOrDefault(t => t.IdTask == taskId);

                    if (task == null)
                    {
                        return Json(new { success = false, message = "Tarefa não encontrada" });
                    }

                    var userHasTask = task.UserTaskId != null;

                    var taskDetails = new
                    {
                        name = task.name,
                        description = task.description,
                        deadline = task.deadline.ToShortDateString(),
                        userHasTask = userHasTask
                    };

                    return Json(taskDetails);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao obter os detalhes da tarefa" });
            }
        }
        [HttpPost]
        public JsonResult TakeTask(int taskId, int userId)
        {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    var task = dbContext.tasks.FirstOrDefault(t => t.IdTask == taskId);

                    if (task == null)
                    {
                        return Json(new { success = false, message = "Tarefa não encontrada" });
                    }

                    if (task.UserTaskId != null)
                    {
                        return Json(new { success = false, message = "A tarefa já está atribuída a um utilizador" });
                    }
                    task.UserTaskId = userId;

                    dbContext.SaveChanges();

                    return Json(new { success = true, message = "Tarefa atribuída com sucesso" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao atribuir a tarefa" });
            }
        }
        [HttpPost]
        public JsonResult AbandonTask(int taskId)
        {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    var task = dbContext.tasks.FirstOrDefault(t => t.IdTask == taskId);

                    if (task == null)
                    {
                        return Json(new { success = false, message = "Tarefa não encontrada" });
                    }

                    if (task.UserTaskId == null)
                    {
                        return Json(new { success = false, message = "A tarefa não está atribuída a nenhum utilizador" });
                    }
                    task.UserTaskId = null;

                    dbContext.SaveChanges();

                    return Json(new { success = true, message = "Tarefa abandonada com sucesso" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao abandonar a tarefa" });
            }
        }
        [HttpPost]
        public JsonResult CompleteTask(int taskId, int userId, string newStatus)
        {
            try
            {
                using (var dbContext = new ProjectContext())
                {
                    var task = dbContext.tasks.FirstOrDefault(t => t.IdTask == taskId && t.UserTaskId == userId);

                    if (task == null)
                    {
                        return Json(new { success = false, message = "Tarefa não encontrada ou permissão negada." });
                    }

                    // Atualizar o status da tarefa
                    task.status = newStatus;

                    dbContext.SaveChanges();

                    return Json(new { success = true, message = "Tarefa concluída com sucesso." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao concluir a tarefa." });
            }
        }
        private bool validateDates(DateTime projectStartDate, DateTime projectEndDate, DateTime taskEndDate)
        {
            var today = DateTime.Today;

            if (projectStartDate > today)
            {
                return false;
            }

            if (taskEndDate < projectStartDate || taskEndDate > projectEndDate)
            {
                return false;
            }

            return true;
        }
    }
}