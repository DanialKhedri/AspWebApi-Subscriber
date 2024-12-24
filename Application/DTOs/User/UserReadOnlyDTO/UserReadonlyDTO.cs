using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User.UserReadOnlyDTO;

public class UserReadonlyDTO
{
    public int Id { get; set; }

    public string UserName { get; set; }

    public bool IsAdmin { get; set; }

    public string PhoneNumber { get; set; }
}
