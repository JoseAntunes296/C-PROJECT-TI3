using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class UserProfile
    {
        public int IdUser { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string profileImg { get; set; }
    }
}