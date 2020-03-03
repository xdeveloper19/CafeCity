using Entities.Context;
using Entities.Models;
using Entities.Repositorii;
using Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MvcUser.Controllers
{
    [Authorize(Roles = "leader")]
    public class TeamManageController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly TeamContext _teamContext;

        public TeamManageController(UserManager<User> userManager, TeamContext teamContext)
        {
            _userManager = userManager;
            _teamContext = teamContext;
        }


        [HttpGet]
        public IActionResult Index(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.EditTeamPageSuccess ? "Данные группы успешно сохранены."
                : message == ManageMessageId.AddPostSuccess ? "Должность успешно добавлена."
                : message == ManageMessageId.DeleteUserSuccess ? "Студент успешно удален из группы."
                : message == ManageMessageId.Error ? "Ошибка."
                : "";

            return View();
        }

        public async Task<IActionResult> Edit()
        {
            var user = await GetCurrentUserAsync();
            var id = user.TeamId;

            if (id == null)
            {
                return NotFound();
            }

            var team = await _teamContext.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Team team)
        {
            var user = await GetCurrentUserAsync();
            var id = user.TeamId;

            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _teamContext.Update(team);
                    await _teamContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.EditTeamPageSuccess });
            }
            return View(team);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersFromTeam()
        {
            TeamMethods teamMethods = new TeamMethods(_userManager, _teamContext);
            var user = await GetCurrentUserAsync();
            var id = user.TeamId;

            var Result = await teamMethods.GetUsersFromTeam(id);
            if (Result.Status == ResponseResult.Error)
            {
                return NotFound();
            }
            var users = Result.ResponseData.Objects;
            return View(users);
        }

        /*[HttpPost]
        public async Task<IActionResult> GetSlivers()
        {
            TeamMethods teamData = new TeamMethods(_userManager,_teamContext);
            var Result = await teamData.GetSlivers();
            if (Result.Status == ResponseResult.Error)
            {
                return NotFound();
            }
            var slivers = Result.ResponseData.Objects;
            return View(slivers);
        }*/

        [HttpGet]
        public IActionResult AddRange(string userId)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRange(string userId, string range)
        {
            TeamMethods TeamData = new TeamMethods(_userManager, _teamContext);
            var Result = await TeamData.AddRange(userId, range);
            if (Result.Status == ResponseResult.OK)
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.AddPostSuccess });
            else
            {
                ModelState.AddModelError(string.Empty, Result.Message);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        public async Task<IActionResult> LogOutTeam(string userId)
        {
            TeamMethods TeamData = new TeamMethods(_userManager, _teamContext);
            var Result = await TeamData.DeleteUserFromTeam(userId);
            if (Result.Status == ResponseResult.OK)
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.DeleteUserSuccess });
            else
            {
                ModelState.AddModelError(string.Empty, Result.Message);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public enum ManageMessageId
        {
            EditTeamPageSuccess,
            AddPostSuccess,
            DeleteUserSuccess,
            Error
        }

        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(User);
        }

        private bool TeamExists(Guid id)
        {
            return _teamContext.Teams.Any(e => e.Id == id);
        }

        #endregion
    }
}