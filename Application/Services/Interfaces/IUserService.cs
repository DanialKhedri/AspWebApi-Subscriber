using Application.DTOs.Tokens;
using Application.DTOs.User.UserLogInDTO;
using Application.DTOs.User.UserRegisterDTO;

namespace Application.Services.Interfaces;

public interface IUserService
{



    public Task<bool> Register(UserRegisterDTO userRegisterDTO);

    public Task<UserResultLogInDTO> LogIn(UserLogInDTO userLogInDTO);

    public Task<AccessTokenDTO> RefreshToken(string RefreshToken);



}
