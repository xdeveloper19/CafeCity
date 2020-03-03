using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Entities.Context;
using Entities.ViewModels.AccountViewModels;
using Entities.Repositorii;
//using MvcUser.Models;

namespace MvcUser.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly TeamContext _teamContext;

        public HomeController(UserManager<User> userManager, TeamContext teamContext, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _teamContext = teamContext;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            return View();
            return RedirectToAction("LoggedIn");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> LoggedIn()
        {
            var UserId = _userManager.GetUserId(User);
            ManageMethods ManageData = new ManageMethods(_userManager, _teamContext,_signInManager);
            var Result = await ManageData.LoggedIn(UserId);
            var model = Result.ResponseData;
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
