using Microsoft.AspNetCore.Identity;
using Entities.Models;
using Entities.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Entities.Repositorii;
using MvcUser.Services;


namespace MvcUser.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //регистрация пользователя на сайте 
        //http://geometry.tmc-centert.ru/Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Формирование ответа запроса
                AuthMethods AuthData = new AuthMethods(_userManager, _signInManager);
                var Result=await AuthData.Register(model);
                //если успешно...
                if (Result.Status==ResponseResult.OK)
                {
                    
                    //...возвращение на страницу входа
                   return RedirectToAction("Index", "Home");
                }
                else
                {
                    //ошибка регистрации
                    ModelState.AddModelError(string.Empty, Result.Message);
                }
               /* User user = new User { Email = model.Email, UserName = model.Email, FirstName =
                    model.FirstName, LastName = model.LastName, MiddleName = model.MiddleName};
                //Добавление пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //Установка куки
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }*/
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        //Вход в приложение на сайт 
        //http://geometry.tmc-centert.ru/Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Формирование ответа запроса авторизации
                AuthMethods AuthData = new AuthMethods(_userManager, _signInManager);
                var Result = await AuthData.Login(model);
                //если успешно...
                if (Result.Status == ResponseResult.OK)
                {
                    //проверяем, принажлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        //...вход в приложение
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Result.Message);
                }
                //var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                //if (result.Succeeded)
                //{
                //    //проверяем, принажлежит ли URL приложению
                //    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                //    {
                //        return Redirect(model.ReturnUrl);
                //    }
                //    else
                //    {
                //        return RedirectToAction("Index", "Home");
                //    }
                //}
                //else
                //{
                //    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                //}
            }
            return View(model);
        }


        //Выход из приложения
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            //удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}