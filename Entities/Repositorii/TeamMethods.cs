using Entities.Context;
using Entities.Models;
using Entities.ViewModels;
using Entities.ViewModels.BadgeViewModels;
using Entities.ViewModels.TeamViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Repositorii
{
    public class TeamMethods
    {
        private readonly UserManager<User> _userManager;
        private readonly TeamContext _teamContext;

        public TeamMethods(UserManager<User> userManager, TeamContext teamContext)
        {
            _userManager = userManager;
            _teamContext = teamContext;
        }

        public async Task<ServiceResponseObject<TeamResponse>> Create(string UserId, CreateTeamViewModel model)
        {
            var currentUser = await _userManager.FindByIdAsync(UserId);

            ServiceResponseObject<TeamResponse> DataContent = new ServiceResponseObject<TeamResponse>();
            if (currentUser != null && currentUser.IsLeader != true)
            {
                Badge badgeTeam = new Badge();
                Team team = new Team
                {
                    BadgeId = badgeTeam.Id,
                    Name = model.Name,
                    Leader = currentUser.FirstName + " " + currentUser.LastName,
                    Description = model.Description,
                    City = model.City,
                    Date = model.Date,
                    Section = model.Section,
                    Url = model.Url
                };

                UserBadge leaderBadge = new UserBadge
                {
                    BadgeId = badgeTeam.Id,
                    UserId = UserId
                };
                var sliver = _teamContext.Slivers.FirstOrDefault(s => s.Rang == "Командир");

                UserSliver leaderSliver = new UserSliver
                {
                    UserId = UserId,
                    SliverId = sliver.Id
                };

                sliver.UserSlivers.Add(leaderSliver);
                currentUser.TeamId = team.Id;
                _teamContext.UserBadges.Add(leaderBadge);
                badgeTeam.UserBadges.Add(leaderBadge);
                team.Badges.Add(badgeTeam);
                await _userManager.AddToRoleAsync(currentUser, "leader");
                currentUser.IsLeader = true;
                _teamContext.Teams.Add(team);
                await _userManager.UpdateAsync(currentUser);
                await _teamContext.Badges.AddAsync(badgeTeam);
                await _teamContext.SaveChangesAsync();
                DataContent.Message = "Отряд успешно создан!";
                DataContent.Status = ResponseResult.OK;
                DataContent.ResponseData = new TeamResponse()
                {
                    TeamId = team.Id,
                    Name = team.Name,
                    Leader = currentUser.LastName + " " + currentUser.FirstName + " " + currentUser.MiddleName
                };
                return DataContent;
            }
            DataContent.Message = "Упс, пользователь не найден или отряд уже создан.";
            DataContent.Status = ResponseResult.Error;
            return DataContent;
        }

        public async Task<ServiceResponseObject<BaseResponseObject>> UploadBadge(string UserId, CreateBadgeViewModel model)
        {
            ServiceResponseObject<BaseResponseObject> DataContent = new ServiceResponseObject<BaseResponseObject>();
            var currentUser = await _userManager.FindByIdAsync(UserId);

            if (currentUser != null)
            {
                var userBadge = await _teamContext.UserBadges.FirstOrDefaultAsync(f => f.UserId == UserId);
                var teamBadge = await _teamContext.Badges.FirstOrDefaultAsync(f => f.Id == userBadge.BadgeId);

                if (userBadge != null)
                {
                    teamBadge.Title = model.Title;
                    teamBadge.Description = model.Description;
                    teamBadge.FileType = (teamBadge.FileType != null)?model.Imagine.ContentType:null;
                    
                    _teamContext.SaveChanges();

                    if (model.Imagine == null)
                    {
                        DataContent.Message = "Ошибка. Загрузите фото значка.";
                        DataContent.Status = ResponseResult.Error;
                        return DataContent;
                    }
                    else
                    {
                        /*var uploads = Path.Combine(_environment.WebRootPath, "uploads");
                        using (var fileStream = new FileStream(Path.Combine(uploads,model.Imagine.FileName),FileMode.Create))
                        {
                            await model.Imagine.CopyToAsync(fileStream);
                        }*/

                        byte[] imageData = null;
                        var file = model.Imagine;

                        using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                        {
                            imageData = binaryReader.ReadBytes((int)file.Length);
                        }
                        teamBadge.FileContent = imageData;
                       
                    }
                    _teamContext.SaveChanges();
                    DataContent.Message = "Значок успешно создан!";
                    DataContent.Status = ResponseResult.OK;
                    return DataContent;
                }
                else
                {
                    DataContent.Message = "Ошибка. Заполните для начала информацию об отряде.";
                    DataContent.Status = ResponseResult.Error;
                    return DataContent;
                }
            }
            else
            {
                DataContent.Message = "Упс. Пользователь не найден.";
                DataContent.Status = ResponseResult.Error;
                return DataContent;
            }
        }

        public async Task<ServiceResponseObject<TeamResponse>> BelongToTeam(string UserId)
        {
            var userBadge = await _teamContext.UserBadges.FirstOrDefaultAsync(f => f.UserId == UserId);
            ServiceResponseObject<TeamResponse> TeamData = new ServiceResponseObject<TeamResponse>();

            if (userBadge != null)
            {
                var teamBadge = await _teamContext.Badges.FirstOrDefaultAsync(f => f.Id == userBadge.BadgeId);
                var Team = await _teamContext.Teams.FirstOrDefaultAsync(f => f.Id == teamBadge.TeamId);
                var TeamLeader = await _userManager.Users.FirstOrDefaultAsync(f => f.TeamId == Team.Id && f.IsLeader == true);
                
                TeamData.ResponseData = new TeamResponse
                {
                    TeamId = Team.Id,
                    Name = Team.Name,
                    Leader = TeamLeader.LastName + " " + TeamLeader.FirstName + " " + TeamLeader.MiddleName
                };

                TeamData.Message = "Принадлежность пользователя к отряду успешно определена.";
                TeamData.Status = ResponseResult.OK;
                return TeamData;
            }
            else
            {
                TeamData.Message = "Ошибка. Пользователь не найден или еще не вступил в отряд.";
                TeamData.Status = ResponseResult.Error;
                return TeamData;
            }
        }

        public async Task<ServiceResponseObject<ListResponse<UserFromTeamResponse>>> GetUsersFromTeam(Guid TeamId)
        {
            var Team = await _teamContext.Teams.FirstOrDefaultAsync(f => f.Id == TeamId);
            ServiceResponseObject<ListResponse<UserFromTeamResponse>> TeamData = new ServiceResponseObject<ListResponse<UserFromTeamResponse>>();
            
            if (Team != null)
            {
                var usersInTeam = await _userManager.Users.Where(f => f.TeamId == TeamId) 
                    .Select(s => new UserFromTeamResponse
                    {
                        UserId = s.Id,
                        Email = s.Email,
                        FIO = s.FirstName + " " + s.LastName
                    }).ToListAsync();

                foreach(var user in usersInTeam)
                {
                    var userSliver = await _teamContext.UserSlivers.FirstOrDefaultAsync(s => s.UserId == user.UserId);
                    if (userSliver == null || userSliver.SliverId == null)
                        user.Rang = "Кандидат";
                    else
                    {
                        var sliver = await _teamContext.Slivers.FirstOrDefaultAsync(f => f.Id == userSliver.SliverId);
                        user.Rang = sliver.Rang;
                        user.FileContent = sliver.FileContent;
                    }
                }
                
                TeamData.ResponseData = new ListResponse<UserFromTeamResponse>()
                {
                    Objects = usersInTeam
                };
                TeamData.Message = "Успешно!";
                TeamData.Status = ResponseResult.OK;
                return TeamData;
            }
            else
            {
                TeamData.Message = "Отряд не найден.";
                TeamData.Status = ResponseResult.Error;
                return TeamData;
            }
        }

        public async Task<ServiceResponseObject<TeamPageViewModel>> GetTeam(Guid TeamId, string UserId)
        {
            ServiceResponseObject<TeamPageViewModel> TeamData = new ServiceResponseObject<TeamPageViewModel>();
            var Team = await _teamContext.Teams.FindAsync(TeamId);
            var currentUser = await _userManager.FindByIdAsync(UserId);

            if (Team != null)
            {
                var Leader = await _userManager.Users.FirstOrDefaultAsync(f => f.IsLeader == true && f.TeamId == TeamId);
                var Badge = await _teamContext.Badges.FirstOrDefaultAsync(f => f.TeamId == TeamId);
                
                TeamData.ResponseData = new TeamPageViewModel
                {
                    TeamId = Team.Id,
                    LeaderId = Leader.Id,
                    LeaderName = Leader.LastName + " " + Leader.FirstName + " " + Leader.MiddleName,
                    TeamName = Team.Name,
                    Description = Team.Description,
                    City = Team.City,
                    Date = Team.Date,
                    Url = Team.Url,
                    IsLeader = currentUser.IsLeader,
                    Section = Team.Section,
                    Symbol = Badge.FileContent
                };
                TeamData.Message = "Отряд успешно найден.";
                TeamData.Status = ResponseResult.OK;
                return TeamData;
            }
            else
            {
                TeamData.Message = "Отряд не найден.";
                TeamData.Status = ResponseResult.Error;
                return TeamData;
            }
        }

        public async Task<ServiceResponseObject<BaseResponseObject>> Join (string userId, Guid id)
        {
            ServiceResponseObject<BaseResponseObject> ContentData = new ServiceResponseObject<BaseResponseObject>();
            var currentUser = await _userManager.FindByIdAsync(userId);
            var team = await _teamContext.Teams.FirstOrDefaultAsync(t => t.Id == id);

            if (currentUser == null)
            {
                ContentData.Status = ResponseResult.Error;
                ContentData.Message = "Пользователь не найден.";
                return ContentData;
            }

            if (currentUser.IsLeader)
            {
                ContentData.Status = ResponseResult.Error;
                ContentData.Message = "Ошибка. Командир не может вступить в отряд.";
                return ContentData;
            }

            if (team == null)
            {
                ContentData.Status = ResponseResult.Error;
                ContentData.Message = "Отряд не найден.";
                return ContentData;
            }

            var userBadge = await _teamContext.UserBadges.FirstOrDefaultAsync(s => s.UserId == currentUser.Id);
            var teamBadge = await _teamContext.Badges.FirstOrDefaultAsync(f => f.TeamId == team.Id);
            if (userBadge == null)
            {
                UserBadge badge = new UserBadge
                {
                    BadgeId = teamBadge.Id,
                    UserId = currentUser.Id
                };
                _teamContext.UserBadges.Add(badge);
                teamBadge.UserBadges.Add(badge);
                await _teamContext.SaveChangesAsync();
            }
            else
            {
                userBadge.BadgeId = teamBadge.Id;
                _teamContext.Update(userBadge);
                await _teamContext.SaveChangesAsync();
            }
            
            currentUser.TeamId = team.Id;
            await _userManager.UpdateAsync(currentUser);
            ContentData.Message = "Пользователь успешно стал кандидатом.";
            ContentData.Status = ResponseResult.OK;
            return ContentData;
        }

        public async Task<ServiceResponseObject<BaseResponseObject>> AddRange(string userId, string range)
        {
            ServiceResponseObject<BaseResponseObject> TeamData = new ServiceResponseObject<BaseResponseObject>();
            var currentUser = await _userManager.FindByIdAsync(userId);

            if (currentUser == null)
            {
                TeamData.Status = ResponseResult.Error;
                TeamData.Message = "Пользователь не найден.";
                return TeamData;
            }

            if (currentUser.TeamId == Guid.Empty)
            {
                TeamData.Status = ResponseResult.Error;
                TeamData.Message = "Пользователь не является участником отряда.";
                return TeamData;
            }
            else if(currentUser.IsLeader)
            {
                TeamData.Status = ResponseResult.Error;
                TeamData.Message = "Нельзя присвоить должность командиру.";
                return TeamData;
            }

            var sliver = await _teamContext.Slivers.FirstOrDefaultAsync(f => f.Rang == range);
            if (sliver != null)
            {
                var userSliver = await _teamContext.UserSlivers.FirstOrDefaultAsync(f => f.UserId == userId);
                if (userSliver != null)
                {
                    userSliver.SliverId = sliver.Id;
                    _teamContext.Update(userSliver);
                    await _teamContext.SaveChangesAsync();
                }
                else
                {
                    UserSliver item = new UserSliver
                    {
                        UserId = userId,
                        SliverId = sliver.Id
                    };

                    _teamContext.Add(item);
                    await _teamContext.SaveChangesAsync();
                }

                TeamData.Message = "Успешно.";
                TeamData.Status = ResponseResult.OK;
                return TeamData;
            }
            TeamData.Message = "Лычка не найдена.";
            TeamData.Status = ResponseResult.Error;
            return TeamData;
        }

        /*public async Task<ServiceResponseObject<ListResponse<Sliver>>> GetSlivers()
        {
            ServiceResponseObject<ListResponse<Sliver>> TeamData = new ServiceResponseObject<ListResponse<Sliver>>();
            var slivers = await _teamContext.Slivers.Where(f => f.Rang != "Командир").ToListAsync();

            if (slivers != null)
            {
                TeamData.ResponseData = new ListResponse<Sliver>
                {
                    Objects = slivers
                };
                TeamData.Message = "Успешно.";
                TeamData.Status = ResponseResult.OK;
                return TeamData;
            }
            TeamData.Message = "Лычки не найдены.";
            TeamData.Status = ResponseResult.Error;
            return TeamData;
        }*/

        public async Task<ServiceResponseObject<BaseResponseObject>> DeleteUserFromTeam(string userId)
        {
            ServiceResponseObject<BaseResponseObject> TeamData = new ServiceResponseObject<BaseResponseObject>();
            var currentUser = await _userManager.FindByIdAsync(userId);
            if (currentUser != null && !currentUser.IsLeader)
            {
                currentUser.TeamId = Guid.Empty;
                await _userManager.UpdateAsync(currentUser);
                var userSliver = await _teamContext.UserSlivers.FirstOrDefaultAsync(f => f.UserId == userId);
                var userBadge = await _teamContext.UserBadges.FirstOrDefaultAsync(f => f.UserId == userId);

                if (userSliver != null)
                {
                    userSliver.SliverId = null;
                    _teamContext.UserSlivers.Update(userSliver);
                    await _teamContext.SaveChangesAsync();
                }

                if (userBadge != null)
                {
                    userBadge.BadgeId = null;
                    _teamContext.UserBadges.Update(userBadge);
                    await _teamContext.SaveChangesAsync();
                }
                TeamData.Message = "Пользователь больше не состоит в отряде.";
                TeamData.Status = ResponseResult.OK;
                return TeamData;
            }
            TeamData.Message = "Ошибка. Пользователь не найден или является командиром.";
            TeamData.Status = ResponseResult.Error;
            return TeamData;
        }
        /*public async Task<ServiceResponseObject<BaseResponseObject>> SendRequest(string userId, Guid teamId)
        {
            var currentUser = await _userManager.FindByIdAsync(userId);
            ServiceResponseObject<BaseResponseObject> TeamData = new ServiceResponseObject<BaseResponseObject>();

            if (currentUser == null)
            {
                TeamData.Status = ResponseResult.Error;
                TeamData.Message = "Пользователь не найден.";
                return TeamData;
            }

            if (currentUser.IsLeader)
            {
                TeamData.Status = ResponseResult.Error;
                TeamData.Message = "Ошибка. Командир не может отправлять запрос на вступление.";
                return TeamData;
            }

            currentUser.TeamId = teamId;
            currentUser.IsInvited = true;
            var Result = await _userManager.UpdateAsync(currentUser);

            if (Result.Succeeded)
            {
                TeamData.Status = ResponseResult.OK;
                TeamData.Message = "Заявка отправлена.";
                return TeamData;
            }

            foreach (var error in Result.Errors)
            {
                TeamData.Message += error.Description;
            }
            TeamData.Status = ResponseResult.Error;
            return TeamData;
        }*/

    }
}
