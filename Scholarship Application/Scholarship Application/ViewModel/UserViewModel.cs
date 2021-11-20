using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Scholarship_Application.Models
{
    public class UserViewModel
    {
        DemoEntities db = new DemoEntities();
        public int UserID { get; set; }
        [Required(ErrorMessage = "User Name Is Required.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password Is Required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool CheckUserNameAndPassword()
        {
            if (db.Users.Where(t => t.user_name == this.UserName && t.password == this.Password).FirstOrDefault() != null)
                return true;
            return false;
        }
        public bool CheckDuplicateUserName()
        {
            if (db.Users.Where(t => t.user_name == this.UserName).FirstOrDefault() != null)
                return false;
            return true;
        }
    }
    
}