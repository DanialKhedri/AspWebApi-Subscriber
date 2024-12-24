using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces.IRepository;

public interface IUserRepository
{

    public Task<bool> Register(Entities.User user);
    public Task<Entities.User> LogIn(Entities.User user);
    public Task<string> RefreshToken(string RefreshToken);
   

}
