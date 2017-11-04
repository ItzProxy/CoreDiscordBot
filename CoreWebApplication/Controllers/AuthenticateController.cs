using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using Microsoft.AspNetCore.Authentication;

namespace CoreWebApplication.Controllers
{
    public class AuthenticateController : Controller
    {
        public IActionResult DoChallenge()
        {
            var authProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home")
            };
            return Challenge(authProperties, "Discord");
        }

        public IActionResult LoginDiscord()
        {
            var authProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home")
            };
            return Challenge(authProperties, "Discord");
        }
    }
}