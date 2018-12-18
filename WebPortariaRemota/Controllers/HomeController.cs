using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebPortariaRemota.Models;

namespace WebPortariaRemota.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var authenticationManager = Request.HttpContext;
            if (authenticationManager.User != null)
                if(authenticationManager.User.Identity.IsAuthenticated)
                    Response.Redirect("Home/Welcome");

            return View();
        }

        [Authorize]
        public ActionResult Welcome()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
