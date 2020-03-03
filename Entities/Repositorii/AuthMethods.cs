using Entities.Models;
using Entities.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Entities.Repositorii
{
    public class AuthMethods
    {
        UserManager<User> _userManager;
        SignInManager<User> _signInManager;

        
        public AuthMethods(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ServiceResponseObject<AuthResponse>> Register(RegisterViewModel model)
        {
          
        ServiceResponseObject <AuthResponse> DataContent = new ServiceResponseObject<AuthResponse>();
            
            User user = new User
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName =
                   model.FirstName,
                LastName = model.LastName
            };
            
         
            //Добавление пользователя
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                //Установка куки
                await _signInManager.SignInAsync(user, false);

                DataContent.ResponseData = new AuthResponse()
                {
                    UserId = user.Id,
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Latitude = user.Latitude,
                    Longitude = user.Longitude
                };
                DataContent.Message = "Успешно!";
                DataContent.Status = ResponseResult.OK;
                return DataContent;
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    DataContent.Message += error.Description;
                } 
                DataContent.Status = ResponseResult.Error;
                return DataContent;
            }
        }

        public async Task<ServiceResponseObject<AuthResponse>> Login (LoginViewModel model)
        {
            ServiceResponseObject<AuthResponse> DataContent = new ServiceResponseObject<AuthResponse>();

            User user = await _userManager.FindByEmailAsync(model.Email);
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded && user!=null)
            {
                DataContent.ResponseData = new AuthResponse()
                {
                    UserId = user.Id,
                    UserName = model.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Longitude = user.Longitude,
                    Latitude = user.Latitude
                };
                DataContent.Message = "Авторизация прошла успешно!";
                DataContent.Status = ResponseResult.OK;
                return DataContent;
            }
            else
            {
                DataContent.Message = "Неправильный логин и(или) пароль";
                DataContent.Status = ResponseResult.Error;
                return DataContent;
            }
        }

    }
}
