using Idams.Core.Model.Dtos;
using Idams.Core.Services;
using Idams.WebApi.Utils;
using Idams.WebApi.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Idams.WebApi.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Index", "Challenge");
        }
    }
}
