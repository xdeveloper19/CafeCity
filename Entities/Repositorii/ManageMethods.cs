using Entities.Context;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.AccountViewModels;
using Entities.ViewModels.ManageViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Entities.Repositorii
{
    public class ManageMethods
    {
        private readonly UserManager<User> _userManager;
        private readonly TeamContext _teamContext;
        private readonly SignInManager<User> _signInManager;

        public ManageMethods(UserManager<User> userManager, TeamContext teamContext, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _teamContext = teamContext;
        }


        public async Task<ServiceResponseObject<UserProfileViewModel>> LoggedIn(string UserId)
        {
            var currentUser = await _userManager.FindByIdAsync(UserId);
            ServiceResponseObject<UserProfileViewModel> DataContent = new ServiceResponseObject<UserProfileViewModel>();
            if (currentUser != null)
            {
                var userSliver = await _teamContext.UserSlivers.FirstOrDefaultAsync(s => s.UserId == UserId);
                var userBadge = await _teamContext.UserBadges.FirstOrDefaultAsync(s => s.UserId == UserId);

                if (userBadge != null)
                {
                    var badge = await _teamContext.Badges.FirstOrDefaultAsync(s => s.Id == userBadge.BadgeId);
                    if (badge != null)
                    {
                        var team = await _teamContext.Teams.FirstOrDefaultAsync(s => s.Id == badge.TeamId);

                        DataContent.ResponseData = new UserProfileViewModel()
                        {
                            FirstName = currentUser.FirstName,
                            LastName = currentUser.LastName,
                            MiddleName = (currentUser.MiddleName == null) ? "Не указано" : currentUser.MiddleName,
                            Team = team.Name,
                            TeamId = team.Id,
                            Rang = (currentUser.IsLeader == false) ? "Не указан" : "Командир",
                            Section = team.Section
                        };
                    }
                }
                else
                {
                    DataContent.ResponseData = new UserProfileViewModel()
                    {
                        FirstName = currentUser.FirstName,
                        LastName = currentUser.LastName,
                        MiddleName = (currentUser.MiddleName == null) ? "Не указано" : currentUser.MiddleName,
                        Team = "Не состою",
                        Rang = (userSliver == null) ? "Не указан" : userSliver.Sliver.Rang,
                        Section = "Не указан" 
                    };
                }

                DataContent.Message = "Успешно!";
                DataContent.Status = ResponseResult.OK;
                return DataContent;
            }
            DataContent.Message = "Пользователь не найден!";
            DataContent.Status = ResponseResult.Error;
            return DataContent;
        }

        public async Task<ServiceResponseObject<IndexViewModel>> Index(User user)
        {
            ServiceResponseObject<IndexViewModel> DataContent = new ServiceResponseObject<IndexViewModel>();
            if (user != null)
            {

                DataContent.ResponseData = new IndexViewModel()
                {
                    PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
                    MiddleName = user.MiddleName,
                    Email = user.Email,
                    IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user),
                    BrowserRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                    IsLeader = user.IsLeader,
                    TeamId = user.TeamId
                };

                DataContent.Message = "Успешно!";
                DataContent.Status = ResponseResult.OK;
                return DataContent;
            }
            DataContent.Message = "Пользователь не найден!";
            DataContent.Status = ResponseResult.Error;
            return DataContent;
        }

        public async Task<ServiceResponseObject<BaseResponseObject>> AddMiddleName(User user, string MiddleName)
        {
            ServiceResponseObject<BaseResponseObject> DataContent = new ServiceResponseObject<BaseResponseObject>();
            if (user == null)
            {
                DataContent.Message = "Пользователь не найден!";
                DataContent.Status = ResponseResult.Error;
                return DataContent;
            }
            if (user.MiddleName == null)
            {
                user.MiddleName = MiddleName;
                var Result = await _userManager.UpdateAsync(user);
                if (Result.Succeeded)
                {
                    DataContent.Message = "Успешно!";
                    DataContent.Status = ResponseResult.OK;
                    return DataContent;
                }
                else
                {
                    DataContent.Message = "Что-то пошло не так!";
                    DataContent.Status = ResponseResult.Error;
                    return DataContent;
                }
            }
            DataContent.Message = "Отчество уже добавлено!";
            DataContent.Status = ResponseResult.Error;
            return DataContent;
        }


        
    }
}
