using Application.DTOs.Tokens;
using Application.DTOs.User.UserLogInDTO;
using Application.DTOs.User.UserRegisterDTO;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interfaces.IRepository;
namespace Application.Services.Implements;


public class UserService : IUserService
{

    #region Ctor

    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;


    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;

    }


    #endregion




    public async Task<bool> Register(UserRegisterDTO userRegisterDTO)
    {
        if (userRegisterDTO == null)
            return false;


        var user = _mapper.Map<Domain.Entities.User>(userRegisterDTO);

        return await _userRepository.Register(user);


    }

    public async Task<UserResultLogInDTO> LogIn(UserLogInDTO userLogInDTO)
    {
        if (userLogInDTO == null)
            return null;

        var user = _mapper.Map<Domain.Entities.User>(userLogInDTO);

        var LogedUser = await _userRepository.LogIn(user);

        if (LogedUser != null)
        {

            UserResultLogInDTO resultLogInDTO = new UserResultLogInDTO
            {
                Id = LogedUser.Id,
                JWTToken = LogedUser.JWTToken,
                RefreshToken = LogedUser.RefreshToken,
                UserName = LogedUser.UserName,

            };

            return resultLogInDTO;

        }
        else
            return null;

    }


    public async Task<AccessTokenDTO> RefreshToken(string RefreshToken)
    {

        var accesstoken = await _userRepository.RefreshToken(RefreshToken);

        if (accesstoken != null)
        {
            AccessTokenDTO accessToken = new AccessTokenDTO()
            {
                Token = accesstoken,
            };

            return accessToken;

        }
        else
            return null;

    }

}










