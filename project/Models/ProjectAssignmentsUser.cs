using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Project.Models;

namespace Project.Models
{
    public class ProjectAssignmentsUser
    {

        public user user { get; set; } = new user();
        public project project { get; set; } = new project();
        public projectAssignment projectAssignment { get; set; } = new projectAssignment();



    }
}