using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortalAPI_Service.DbContextConnection;
using PortalModels;
using PortalAPI_Service.Repositories.AccessRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace PortalAPI_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Access : ControllerBase
    {
        private readonly ILoginRepo _loginRepo;
        public Access(ILoginRepo loginRepo)
        {
            _loginRepo = loginRepo;
        }

        [HttpGet]
        [Route("all")]
        public IActionResult GetUsers()
        {
            var result = _loginRepo.AllUsers();
            return result == null ? NotFound() : Ok(result);
        }


        [HttpPost]
        //[Route("{username}/{password}")]
        public async Task<IActionResult> CheckUser([FromBody]LoginForm user)
        {
            Console.WriteLine("Inizio");
            try
            {
                if (!ModelState.IsValid) { return BadRequest(); }

                var result = await _loginRepo.CheckUser(user.Username, user.Password);
                Console.WriteLine("Passed");
                return result == null ? NotFound() : (IActionResult)Ok(result);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Checking user credentials : " + ex);
                return null;
            }

        }

    }
}
