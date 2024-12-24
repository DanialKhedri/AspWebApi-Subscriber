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

    [Required(ErrorMessage = "شماره موبایل الزامی است")]
    [StringLength(11, ErrorMessage = "شماره تلفن باید دقیقا 11 رقم باشد", MinimumLength = 11)]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "شماره تلفن باید فقط شامل اعداد باشد")]
    public string PhoneNumber { get; set; }


    [Required(ErrorMessage = "نام و نام خانوادگی الزامی است")]
    public string UserName { get; set; }




    [Required(ErrorMessage = "گذرواژه الزامی است")]
    [StringLength(40, MinimumLength = 8, ErrorMessage = "گذرواژه باید حداقل 8 و حداکثر 40 کاراکتر باشد.")]

    public string Password { get; set; }

    [Required(ErrorMessage = "تکرار گذرواژه الزامی است")]
    [StringLength(40, MinimumLength = 8, ErrorMessage = "گذرواژه باید حداقل 8 و حداکثر 40 کاراکتر باشد.")]

    public string RePassword { get; set; }




}
