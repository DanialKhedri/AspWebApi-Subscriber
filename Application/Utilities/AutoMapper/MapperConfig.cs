#region
using Application.DTOs.User.UserLogInDTO;
using Application.DTOs.User.UserReadOnlyDTO;
using Application.DTOs.User.UserRegisterDTO;
using AutoMapper;
using Domain.Entities;
#endregion

namespace Application.Utilities.AutoMapper;

public class MapperConfig : Profile
{

    public MapperConfig()
    {

        //User
        CreateMap<UserRegisterDTO, User>().ReverseMap();
        CreateMap<UserLogInDTO, User>().ReverseMap();
        CreateMap<UserResultLogInDTO, User>().ReverseMap();
        CreateMap<UserReadonlyDTO, User>().ReverseMap();


    }










}
