using Application.DTOs.User.UserLogInDTO;
using Application.DTOs.User.UserRegisterDTO;
using Application.Services.Interfaces;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;

namespace AspWebApi_Subscriber.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{



    #region Ctor

    private readonly IUserService _userService;
    private readonly ILogger _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }


    #endregion


    //Register
    [HttpPost("[Action]")]
    public async Task<ActionResult> Register(UserRegisterDTO userRegisterDTO)
    {

        _logger.LogInformation(" Attempt Register: " + userRegisterDTO.PhoneNumber);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (userRegisterDTO == null)
            return BadRequest("UserDTO is Null");


        if (userRegisterDTO.Password != userRegisterDTO.RePassword)
            return BadRequest("Password and repeat password are not the same");

        //if (await _userService.IsPhoneNumberExist(userRegisterDTO.PhoneNumber)) //Check Phone Number Exist
            return BadRequest("This mobile number is already registered");



        var result = await _userService.Register(userRegisterDTO);

        if (result)
            return Ok();


        else
        {
            _logger.LogError("Something Went Worong in Register");
            return StatusCode(500);
        }

    }


    //Log In
    [HttpPost("[Action]")]
    public async Task<ActionResult<UserResultLogInDTO?>> LogIn(UserLogInDTO userLogInDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var UserLogged = await _userService.LogIn(userLogInDTO);

        if (UserLogged == null)
        {
            UserResultLogInDTO result = new UserResultLogInDTO();
            result.UserName = "";
            return BadRequest(result);

        }

        return Ok(UserLogged);

    }



    //RefreshToken
    [HttpPost("[Action]")]
    public async Task<ActionResult<AccessToken>> RefreshToken(string RefreshToken)
    {
        var AccessToken = await _userService.RefreshToken(RefreshToken);


        if (AccessToken == null)
        {
            return Unauthorized();
        }


        return Ok(AccessToken);
    }


}
