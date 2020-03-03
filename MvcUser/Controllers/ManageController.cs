using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;
using Entities.ViewModels.ManageViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcUser.Services;

namespace MvcUser.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ISmsSender _smsSender;
        private readonly IEmailSender _emailSender;

        public ManageController(UserManager<User> userManager, SignInManager<User> signInManager, ISmsSender smsSender, IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _smsSender = smsSender;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            ViewData["StatusMessage"] =
                message == ManageMessageId.AddMiddleNameSuccess ? "Отчество добавлено."
                : message == ManageMessageId.AddPhoneSuccess ? "Номер телефона успешно добавлен."
                :message == ManageMessageId.ConfirmEmailSuccess?"Аккаунт успешно подтвержден."
                :message == ManageMessageId.ChangePasswordSuccess?"Пароль успешно изменен."
                : message == ManageMessageId.Error ? "Ошибка."
                : "";

            var user = await GetCurrentUserAsync();
            var model = new IndexViewModel
            {
                PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
                MiddleName = user.MiddleName,
                Email = user.Email,
                IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user),
                BrowserRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                IsLeader = user.IsLeader,
                TeamId = user.TeamId
            };
            return View(model);
        }

        public IActionResult AddMiddleName()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMiddleName(AddMiddleNameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user.MiddleName == null)
            {
                user.MiddleName = model.MiddleName;
                var Result = await _userManager.UpdateAsync(user);
                if (Result.Succeeded)
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.AddMiddleNameSuccess });
                else
                {
                    ModelState.AddModelError(string.Empty, "Что-то пошло не так...");
                }
            }

            //ModelState.AddModelError(string.Empty, "Отчество уже добавлено!");
            return View(model);
        }
        public IActionResult AddPhoneNumber()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //Generate the token and send it
            var user = await GetCurrentUserAsync();
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
            await _smsSender.SendSmsAsync(model.PhoneNumber, "Ваш безопасный код: " + code);
            return RedirectToAction(nameof(VerifyPhoneNumber), new { PhoneNumber = model.PhoneNumber });
        }

        [HttpGet]
        public async Task<IActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(await GetCurrentUserAsync(), phoneNumber);
            //Send an SMS to verify the phone number
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePhoneNumberAsync(user, model.PhoneNumber, model.Code);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.AddPhoneSuccess });
                }
            }
            //If we got this far, something failed, redisplay the form
            ModelState.AddModelError(string.Empty, "Неверный код");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Mailing()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Mailing(string subject, string message)
        {
            var users = await _userManager.Users.Where(f => f.EmailConfirmed == true).ToListAsync();
            foreach (var user in users)
            {
                await _emailSender.SendEmailAsync(user.Email, subject, message);
                return View("ConfirmEmail");
            }
            return View("Error");
        }
       
        public async Task<IActionResult> ConfirmUserAccount()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var callbackUrl = Url.Action("ConfirmUserEmail", "Manage",
                    new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(user.Email, "Подтверждение аккаунта",
                    $"Подтвердите Ваш аккаунт, перейдя по ссылке: <a href='{callbackUrl}'>link</a>");
                return View("ConfirmEmail");
            }
            return View("Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmUserEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ConfirmEmailSuccess });
            else
                return View("Error");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
                return View(model);
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
            AddPhoneSuccess,
            AddMiddleNameSuccess,
            ConfirmEmailSuccess,
            ChangePasswordSuccess,
            Error
        }

        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(User);
        }

        #endregion
    }
}