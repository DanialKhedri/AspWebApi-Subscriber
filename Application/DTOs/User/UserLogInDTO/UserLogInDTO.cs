using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User.UserLogInDTO;

public class UserLogInDTO
{

    [Required(ErrorMessage = "UserName is Required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "PhoneNumber is Required")]
    [StringLength(40, MinimumLength = 8, ErrorMessage = "گذرواژه باید حداقل 8 و حداکثر 40 کاراکتر باشد.")]
    public string Password { get; set; }

}
