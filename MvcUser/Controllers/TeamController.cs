using System;
using System.Linq;
using System.Threading.Tasks;
using Entities.Context;
using Entities.Models;
using Entities.Repositorii;
using Entities.ViewModels;
using Entities.ViewModels.BadgeViewModels;
using Entities.ViewModels.TeamViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MvcUser.Controllers
{
    [Authorize]
    public class TeamController : Controller
    {
        private readonly TeamContext _teamContext;
        private readonly UserManager<User> _userManager;

        public TeamController(TeamContext teamContext, UserManager<User> userManager)
        {
            _userManager = userManager;
            _teamContext = teamContext;
        }
        //GET: Team
        public async Task<IActionResult> Index()
        {
            var teams = await _teamContext.Teams.Select(s => new TeamResponse
            {
                TeamId = s.Id,
                Name = s.Name,
                Leader = s.Leader
            }).ToListAsync();
            
            return View(teams);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTeamViewModel model)
        {

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                TeamMethods TeamData = new TeamMethods(_userManager, _teamContext);
                var Result = await TeamData.Create(userId,model);
                if (Result.Status == ResponseResult.OK)
                    /*var currentUser = await _userManager.FindByIdAsync(userId);

                    Badge badgeTeam = new Badge();
                    //var role = new IdentityRole { Name = "leader" };
                    //IdentityResult result = await _roleManager.CreateAsync(role);

                    Team team = new Team
                    {
                        BadgeId = badgeTeam.Id,
                        Name = model.Name,
                        Description = model.Description,
                        City = model.City,
                        Date = model.Date,
                        Url = model.Url
                    };

                    UserBadge leaderBadge = new UserBadge
                    {
                        Badge = badgeTeam,
                        BadgeId = badgeTeam.Id,
                        UserId = userId
                    };

                    _teamContext.UserBadges.Add(leaderBadge);

                    team.Badges.Add(badgeTeam);
                    await _userManager.AddToRoleAsync(currentUser, "leader");
                    currentUser.IsLeader = true;
                    _teamContext.Teams.Add(team);
                    await _userManager.UpdateAsync(currentUser);
                    await _teamContext.SaveChangesAsync();*/
                    return RedirectToAction(nameof(UploadBadge));
                else
                {
                    ModelState.AddModelError(string.Empty, Result.Message);
                }
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult UploadBadge()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadBadge(CreateBadgeViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            /*var currentUser = await _userManager.FindByIdAsync(userId);

            var userBadge = _teamContext.UserBadges.Find(userId);
            var teamBadge  = await _teamContext.Badges.Where(s => s.Id == userBadge.BadgeId).FirstOrDefaultAsync();

            teamBadge.Title = model.Title;
            teamBadge.Description = model.Description;
            _teamContext.SaveChanges();

            if (model.Imagine != null)
            {
                byte[] imageData = null;

                using (var binaryReader = new BinaryReader(model.Imagine.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)model.Imagine.Length);
                }
                teamBadge.Imagine = imageData;
            }
            _teamContext.SaveChanges();*/
            TeamMethods TeamData = new TeamMethods(_userManager, _teamContext);
            var Result = await TeamData.UploadBadge(userId, model);
            if (Result.Status == ResponseResult.OK)
                return RedirectToAction("Index", "Home");
            else
            {
                ModelState.AddModelError(string.Empty, Result.Message);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            TeamMethods teamMethods = new TeamMethods(_userManager, _teamContext);
            var userId = _userManager.GetUserId(User);
            var Result = await teamMethods.GetTeam(id,userId);
            if (Result.Status == ResponseResult.Error)
            {
                return NotFound();
            }
            var model = Result.ResponseData;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetUsersFromTeam(Guid id)
        {
            TeamMethods teamMethods = new TeamMethods(_userManager, _teamContext);
            var Result = await teamMethods.GetUsersFromTeam(id);
            if (Result.Status == ResponseResult.Error)
            {
                return NotFound();
            }
            var users = Result.ResponseData.Objects;
            return View(users);
        }

        public async Task<IActionResult> Join(Guid id)
        {
            var UserId = _userManager.GetUserId(User);
            TeamMethods TeamData = new TeamMethods(_userManager, _teamContext);
            var Result = await TeamData.Join(UserId, id);
            if (Result.Status == ResponseResult.OK)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                ModelState.AddModelError(string.Empty, Result.Message);
                return View("Error");
            }
        }

        [Authorize(Roles ="leader")]
        public string ManageTeam()
        {
            return "I am a leader!";
        }
    }
}