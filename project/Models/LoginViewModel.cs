using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class LoginViewModel
    {
        public int IdUser{ get; set; }
        public int administrator{ get; set; }
        public string Email{ get; set; }
        public string Password{ get; set; }
    }
}