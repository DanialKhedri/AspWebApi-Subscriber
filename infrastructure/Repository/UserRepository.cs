using Application.Utilities.JWT;
using Application.Utilities.PasswordHasher;
using Domain.Interfaces.IRepository;
using infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace infrastructure.Repository;


public class UserRepository : IUserRepository
{

    #region Ctor

    private readonly DataContext _dataContext;
    private readonly IConfiguration _configuration;

    public UserRepository(DataContext dataContext, IConfiguration configuration)
    {
        _dataContext = dataContext;
        _configuration = configuration;

    }


    #endregion


    //Register
    public async Task<bool> Register(Domain.Entities.User user)
    {

        if (user != null)
        {

            try
            {
                user.Password = PasswordHasher.HashPassword(user.Password);

                await _dataContext.Users.AddAsync(user);
                await _dataContext.SaveChangesAsync();

                return true;
            }

            catch
            {
                return false;
            }


        }
        return false;

    }


    //LogIn
    public async Task<Domain.Entities.User> LogIn(Domain.Entities.User user)
    {

        if (user != null)
        {
            var OriginUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName &&
                                    u.Password == PasswordHasher.HashPassword(user.Password));
            if (OriginUser == null)
                return null;


            // تولید Access Token
            var Token = await JWTTokenService.GenerateToken(OriginUser, _configuration);

            // تولید Refresh Token
            var refreshToken = JWTTokenService.GenerateRefreshToken();

            // تنظیم زمان انقضای Refresh Token (مثلاً 10 روز)
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(10);



            // ذخیره Refresh Token در دیتابیس
            OriginUser.RefreshToken = refreshToken;
            OriginUser.RefreshTokenExpiryTime = refreshTokenExpiryTime; // اضافه شده

            _dataContext.Users.Update(OriginUser);
            await _dataContext.SaveChangesAsync();

            Domain.Entities.User LogUser = new Domain.Entities.User()
            {
                Id = OriginUser.Id,
                JWTToken = Token,
                RefreshToken = refreshToken,
                UserName = OriginUser.UserName,
                Password = OriginUser.Password,
            };


            return LogUser;


        }
        else
            return null;
    }


    //Refresh Token
    public async Task<string> RefreshToken(string refreshToken)
    {
        // بررسی صحت Refresh Token در دیتابیس
        var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null; // یا خطا بازگردانید
        }

        // تولید Access Token جدید
        var newAccessToken = await JWTTokenService.GenerateToken(user, _configuration);

        // بازگرداندن Access Token جدید
        return newAccessToken;


    }


}

