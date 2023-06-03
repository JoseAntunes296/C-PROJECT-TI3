using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class ViewModal
    {
        public IEnumerable<project> Projects { get; set; }
        public IEnumerable<project> UserProjects { get; set; }
    }
}