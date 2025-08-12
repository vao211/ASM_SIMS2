using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIMS_test.Models.ViewModels.Authen;
using WebSIMS.Services;

namespace WebSIMS.Controllers
{
    [Authorize] 
    public class AuthenController : Controller
    {
        private readonly UserService _userService;
        
        public AuthenController(UserService service)
        {
            _userService = service;
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {

            return View();
        }
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string username = model.Username.Trim();
                string password = model.Password.Trim();
                var user = await _userService.LoginUserAsync(username, password);
                if (user == null)
                {
                    ViewData["MessageLogin"] = "Account Invalid, please try again !";
                    return View(model);
                }
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("UserID", user.UserID.ToString()) 
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                return RedirectToAction("Index", "Dashboard");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            foreach (var item in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(item);
            }
            return RedirectToAction("Index", "Home");
        }

    }
}