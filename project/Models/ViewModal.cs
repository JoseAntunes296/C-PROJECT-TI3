using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class ViewModal
    {
        public LoginViewModel LoginModel { get; set; }
        public IEnumerable<project> Projects { get; set; }
    }
}