#region Usings
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Application.DTOs.User.UserRegisterDTO;

public class UserRegisterDTO
{
    [Required(ErrorMessage = "Username is Required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password Is Requirde")]
    [StringLength(40, MinimumLength = 8, ErrorMessage = "The password must be at least 8 and at most 40 characters.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Password repetition is required.")]
    [StringLength(40, MinimumLength = 8, ErrorMessage = "The password must be at least 8 and at most 40 characters.")]
    public string RePassword { get; set; }

}
