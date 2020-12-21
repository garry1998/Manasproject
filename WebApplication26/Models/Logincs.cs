using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication26.Models
{
    public class Logincs
    {[Key]
        [Required]
        [Display(Name = "UserName")]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }
        [Required]

        [Display(Name = "UserPwd")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int checktype { get; set; }
    }
}
