using Microsoft.AspNetCore.Mvc;
using PortalModels;
using PortalAPI_Service.Repositories.AccessRepos;
using System;

using System.Threading.Tasks;


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
        private IActionResult GetUsers()
        {
            var result = _loginRepo.AllUsers();
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        private async Task<IActionResult> CheckUser([FromBody]LoginForm user)
        {
            Console.WriteLine("Inizio");
            try
            {
                if (!ModelState.IsValid) { return BadRequest(); }

                var result = await _loginRepo.CheckUser(user.Username, user.Password);
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
