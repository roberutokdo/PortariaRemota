using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using WebPortariaRemota.Models;
using WebPortariaRemota.Models.WebApiContext;
using WebPortariaRemota.Security;

namespace WebPortariaRemota.Controllers
{
    public class AccountController : Controller
    {
        private EncryptLogin Encrypt = EncryptLogin.Instance;

        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel loginModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var login = new Login();
                HttpStatusCode result = await login.GetLogin(loginModel.Username, Encrypt.GetEncryptLoginPass(loginModel.Password));

                if (result == HttpStatusCode.OK)
                {
                    await this.SignInUser(loginModel.Username, true);

                    return this.RedirectToAction("Welcome", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuário ou Senha inválidos.");
                }
            }

            return this.View();
        }

        private async Task SignInUser(string username, bool isPersistent)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username)
            };

            var claimIdenties = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimPrincipal = new ClaimsPrincipal(claimIdenties);
            var authenticationManager = Request.HttpContext;

            await authenticationManager.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimPrincipal, new AuthenticationProperties() { IsPersistent = isPersistent });
        }

        [Authorize]
        public IActionResult SignOut()
        {
            var authenticationManager = Request.HttpContext;
            authenticationManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}