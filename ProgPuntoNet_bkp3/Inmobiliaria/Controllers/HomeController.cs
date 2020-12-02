using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration config;
        private readonly IRepositorioPropietario repo;

        public HomeController(IConfiguration config, IRepositorioPropietario repositorio)
        {
            this.config = config;
            this.repo = repositorio;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Seguro()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            return View(claims);
        }

        [Authorize]
        public IActionResult Admin()
        {
            return View();
        }

        // GET: Home/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Home/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginView loginView)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: loginView.Clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8));
                    var p = repo.GetByEmail(loginView.Email);
                    if (p == null || p.Clave != hashed)
                    {
                        ViewBag.Mensaje = "Datos inválidos";
                        return View();
                    }
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, p.Email),
                        //new Claim(ClaimTypes.Name, p.Nombre),
                        //new Claim(ClaimTypes.Email, p.Email),
                        new Claim("Identity", p.IdPropietario.ToString()),
                        new Claim(ClaimTypes.Role, p.IdPropietario < 10 ? "RolAdmin" : "RolEmpresa"),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        //AllowRefresh = <bool>,
                        // Refreshing the authentication session should be allowed.
                        AllowRefresh = true,
                        //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        //IsPersistent = true,
                        // Whether the authentication session is persisted across 
                        // multiple requests. When used with cookies, controls
                        // whether the cookie's lifetime is absolute (matching the
                        // lifetime of the authentication ticket) or session-based.

                        //IssuedUtc = <DateTimeOffset>,
                        // The time at which the authentication ticket was issued.

                        //RedirectUri = <string>
                        // The full path or absolute URI to be used as an http 
                        // redirect response value.
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();//Ver mensaje a devolver
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View();
            }
        }

        // GET: Home/Login
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        public ActionResult Restringido()
        {
            return View();
        }
    }
}