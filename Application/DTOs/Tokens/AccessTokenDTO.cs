#region usings
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Application.DTOs.Tokens;

public class AccessTokenDTO
{

    [Required]
    public string Token { get; set; }

}
