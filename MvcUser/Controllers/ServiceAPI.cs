using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Context;
using Entities.Models;
using Entities.Repositorii;
using Entities.ViewModels;
using Entities.ViewModels.AccountViewModels;
using Entities.ViewModels.BadgeViewModels;
using Entities.ViewModels.ManageViewModels;
using Entities.ViewModels.TeamManageViewModels;
using Entities.ViewModels.TeamViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcUser.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MvcUser.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceAPI : ControllerBase
    {
        UserManager<User> _userManager;
        SignInManager<User> _signInManager;
        TeamContext _teamContext;
        IEmailSender _emailSender;


        public ServiceAPI(UserManager<User> userManager, SignInManager<User> signInManager, TeamContext teamContext, IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _signInManager = signInManager;
            _teamContext = teamContext;
        }


        //Получение сформированного ответа регистрации: "успешно" или описание ошибки...
        //...,а также получение фамилии,имени, логина
        //http://geometry.tmc-centert.ru/api/serviceapi/register
        [HttpPost]
        [Route("Register")]
        public async Task<ServiceResponseObject<AuthResponse>> Register(RegisterViewModel model)
        {
            AuthMethods AuthData = new AuthMethods(_userManager,_signInManager);
            var Result = await AuthData.Register(model);
            return Result;
        }

        //Получение сформированного ответа авторизации: "Авторизация прошла успешно!" или ...
        //... "Неправильный логин и(или) пароль",а также получение фамилии,имени,логина
        //http://geometry.tmc-centert.ru/api/serviceapi/login
        [HttpPost]
        [Route("Login")]
        public async Task<ServiceResponseObject<AuthResponse>> Login (LoginViewModel model)
        {
            AuthMethods AuthData = new AuthMethods(_userManager, _signInManager);
            var Result = await AuthData.Login(model);
            return Result;
        }

        [HttpPost]
        [Route("SetUserLocation")]
        public async Task<ServiceResponseObject<BaseResponseObject>> SetUserLocation(string UserId, double Lon1, double Lat1)
        {
            User user = await _userManager.FindByIdAsync(UserId);
            ServiceResponseObject<BaseResponseObject> DataContent = new ServiceResponseObject<BaseResponseObject>();

            if (user != null)
            {
                user.Longitude = Lon1;
                user.Latitude = Lat1;
                var Result = await _userManager.UpdateAsync(user);
                if (Result.Succeeded)
                {
                    DataContent.Message = "Успешно!";
                    DataContent.Status = ResponseResult.OK;
                }
                else
                {
                    foreach (var error in Result.Errors)
                    {
                        DataContent.Message += error.Description;
                    }
                    DataContent.Status = ResponseResult.Error;
                }
                return DataContent;
            }

            DataContent.Message = "Пользователь не найден!";
            DataContent.Status = ResponseResult.Error;
            return DataContent;
        }

        [HttpPost]
        [Route("GetUsersFromLocation")]
        public async Task<ServiceResponseObject<ListResponse<AuthResponse>>> GetUsersFromLocation(string UserId, double Lon1, double Lat1)
        {
            User user = await _userManager.FindByIdAsync(UserId);
            ServiceResponseObject<ListResponse<AuthResponse>> DataContent = new ServiceResponseObject<ListResponse<AuthResponse>>();

            GeometryMethods geometry = new GeometryMethods();

            if (user != null)
            {
                var usersInLocation = _userManager.Users.Where(s => s.Id != UserId) //&& geometry.ArcInMeters(s.Latitude,s.Longitude,Lon1,Lat1)/1000.0 > 500.0)
                    .Select(s => new AuthResponse
                    {
                        UserId = s.Id,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        UserName = s.UserName,
                        Latitude = s.Latitude,
                        Longitude = s.Longitude,
                        Distance = geometry.ArcInMeters(s.Latitude, s.Longitude, Lat1, Lon1)/1000.0
                        
                    }).ToList();

                List<AuthResponse> result = new List<AuthResponse>();
                foreach (var User in usersInLocation )
                {
                    if (User.Distance < 500.0)
                    {
                        result.Add(User);
                    }
                }

                DataContent.Message = "Успешно!";
                DataContent.Status = ResponseResult.OK;
                DataContent.ResponseData = new ListResponse<AuthResponse>()
                {
                    Objects = result
                };
                return DataContent;
            }

            DataContent.Message = "Пользователь не найден!";
            DataContent.Status = ResponseResult.Error;
            return DataContent;
        }

        [HttpPost]
        [Route("LoggedIn")]
        public async Task<ServiceResponseObject<UserProfileViewModel>> LoggedIn(string UserId)
        {
            ManageMethods ManageData = new ManageMethods(_userManager, _teamContext, _signInManager);
            var Result = await ManageData.LoggedIn(UserId);
            return Result;
        }

        [HttpPost]
        [Route("AddMiddleName")]
        public async Task<ServiceResponseObject<BaseResponseObject>> AddMiddleName(string UserId, string MiddleName)
        {
            ManageMethods ManageData = new ManageMethods(_userManager, _teamContext, _signInManager);
            var user = await _userManager.FindByIdAsync(UserId);
            var Result = await ManageData.AddMiddleName(user,MiddleName);
            return Result;
        }


        [HttpGet]
        [Route("UserSettings")]
        public async Task<ServiceResponseObject<IndexViewModel>> UserSettings(string UserId)
        {
            ManageMethods ManageData = new ManageMethods(_userManager, _teamContext, _signInManager);
            var user = await _userManager.FindByIdAsync(UserId);
            var Result = await ManageData.Index(user);
            return Result;
        }

        [HttpGet]
        [Route("ConfirmAccount")]
        public async Task<ServiceResponseObject<BaseResponseObject>> ConfirmAccount(string UserId)
        {
            ServiceResponseObject<BaseResponseObject> DataContent = new ServiceResponseObject<BaseResponseObject>();
            var user = await _userManager.FindByIdAsync(UserId);
            if (user != null)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var callbackUrl = Url.Action("ConfirmUserEmail", "Manage",
                    new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(user.Email, "Подтверждение аккаунта",
                    $"Подтвердите Ваш аккаунт, перейдя по ссылке: <a href='{callbackUrl}'>link</a>");
                DataContent.Message = "Письмо с подтверждением успешно отправлено на почту!";
                DataContent.Status = ResponseResult.OK;
                return DataContent;
            }
            DataContent.Message = "Что-то пошло не так!";
            DataContent.Status = ResponseResult.Error;
            return DataContent;
        }

        [HttpPost]
        [Route("CreateTeam")]
        public async Task<ServiceResponseObject<TeamResponse>> CreateTeam(string UserId, CreateTeamViewModel model)
        {
            TeamMethods TeamData = new TeamMethods(_userManager, _teamContext);
            var Result = await TeamData.Create(UserId, model);
            return Result;
        }

        [HttpPost]
        [Route("UploadBadge")]
        public async Task<ServiceResponseObject<BaseResponseObject>> UploadBadge(string UserId, CreateBadgeViewModel model)
        {
            TeamMethods TeamData = new TeamMethods(_userManager, _teamContext);
            var Result = await TeamData.UploadBadge(UserId, model);
            return Result;
        }

        [HttpGet]
        [Route("GetTeams")]
        public async Task<ServiceResponseObject<ListResponse<TeamResponse>>> GetTeams()
        {
            var teams = await _teamContext.Teams.Select(s => new TeamResponse
            {
                TeamId = s.Id,
                Name = s.Name,
                Leader = s.Leader
            }).ToListAsync();
            ServiceResponseObject<ListResponse<TeamResponse>> TeamData = new ServiceResponseObject<ListResponse<TeamResponse>>();
            if (teams != null)
            {
                TeamData.ResponseData = new ListResponse<TeamResponse>
                {
                    Objects = teams
                };
                TeamData.Message = "Успешно!";
                TeamData.Status = ResponseResult.OK;
                return TeamData;
            }
            TeamData.Message = "Список групп пуст.";
            TeamData.Status = ResponseResult.Error;
            return TeamData;
        }

        [HttpPost]
        [Route("BelongToTeam")]
        public async Task<ServiceResponseObject<TeamResponse>> BelongToTeam(string UserId)
        {
            TeamMethods TeamData = new TeamMethods(_userManager, _teamContext);
            var Result = await TeamData.BelongToTeam(UserId);
            return Result;
        }

        [HttpPost]
        [Route("GetUsersFromTeam")]
        public async Task<ServiceResponseObject<ListResponse<UserFromTeamResponse>>> GetUsersFromTeam(Guid id)
        {
            TeamMethods TeamData = new TeamMethods(_userManager, _teamContext);
            var Result = await TeamData.GetUsersFromTeam(id);
            return Result;
        }

        [HttpGet]
        [Route("GetTeam")]
        public async Task<ServiceResponseObject<TeamPageViewModel>> GetTeam(Guid id,string userId)
        {
            TeamMethods TeamData = new TeamMethods(_userManager, _teamContext);
            var Result = await TeamData.GetTeam(id,userId);
            return Result;
        }

        [HttpPut]
        [Route("EditTeam")]
        public async Task<ServiceResponseObject<BaseResponseObject>> EditTeam(string userId, EditTeamViewModel model)
        {
            ServiceResponseObject<BaseResponseObject> TeamData = new ServiceResponseObject<BaseResponseObject>();
            var user = await _userManager.FindByIdAsync(userId);
            //var team = await _teamContext.Teams.FirstOrDefaultAsync(f => f.Id == user.TeamId);

            if (user == null)
            {
                TeamData.Message = "Пользователь не найден.";
                TeamData.Status = ResponseResult.Error;
                return TeamData;
            }

            var team = await _teamContext.Teams.FirstOrDefaultAsync(f => f.Id == user.TeamId);
            if (user.IsLeader)
            {
                team.Name = model.Name;
                team.Section = model.Section;
                team.Url = model.Url;
                team.City = model.City;
                team.Date = model.Date;
                team.Description = model.Description;

                _teamContext.Update(team);
                await _teamContext.SaveChangesAsync();
                TeamData.Message = "Успешно.";
                TeamData.Status = ResponseResult.OK;
                return TeamData;
            }
            TeamData.Message = "В доступе отказано. Только командир может редактировать данные группы.";
            TeamData.Status = ResponseResult.Error;
            return TeamData;
        }

        [HttpPost]
        [Route("AddRange")]
        public async Task<ServiceResponseObject<BaseResponseObject>> AddRange(string userId, string range, string adminId)
        {
            var admin = await _userManager.FindByIdAsync(adminId);
            if (!admin.IsLeader || admin == null)
            {
                ServiceResponseObject<BaseResponseObject> ContentData = new ServiceResponseObject<BaseResponseObject>();
                ContentData.Message = "В доступе отказано.";
                ContentData.Status = ResponseResult.Error;
                return ContentData;
            }
            TeamMethods TeamData = new TeamMethods(_userManager, _teamContext);
            var Result = await TeamData.AddRange(userId, range);
            return Result;
        }

        [HttpGet]
        [Route("LogOutTeam")]
        public async Task<ServiceResponseObject<BaseResponseObject>> LogOutTeam(string userId, string adminId)
        {
            var admin = await _userManager.FindByIdAsync(adminId);
            if (!admin.IsLeader || admin == null)
            {
                ServiceResponseObject<BaseResponseObject> ContentData = new ServiceResponseObject<BaseResponseObject>();
                ContentData.Message = "В доступе отказано.";
                ContentData.Status = ResponseResult.Error;
                return ContentData;
            }
            TeamMethods TeamData = new TeamMethods(_userManager, _teamContext);
            var Result = await TeamData.DeleteUserFromTeam(userId);
            return Result;
        }

        [HttpPost]
        [Route("Join")]
        public async Task<ServiceResponseObject<BaseResponseObject>> Join(string userId, Guid id)
        {
            TeamMethods TeamData = new TeamMethods(_userManager, _teamContext);
            var Result = await TeamData.Join(userId, id);
            return Result;
        }

        [HttpPost]
        [Route("LogOff")]
        public async Task<ServiceResponseObject<BaseResponseObject>> LogOff()
        {
            //удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            ServiceResponseObject<BaseResponseObject> DataContent = new ServiceResponseObject<BaseResponseObject>();
            DataContent.Status = ResponseResult.OK;
            DataContent.Message = "Вы успешно вышли из приложения!";
            return DataContent;
        }

        /*[HttpGet]
        [Route("SendRequest")]
        public async Task<ServiceResponseObject<BaseResponseObject>> SendRequest(string userId, Guid teamId)
        {
            TeamMethods TeamData = new TeamMethods(_userManager, _teamContext);
            var Result = await TeamData.SendRequest(userId, teamId);
            return Result;
        }*/





        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
