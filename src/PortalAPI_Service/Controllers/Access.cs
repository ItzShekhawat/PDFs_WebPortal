using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortalAPI_Service.DbContextConnection;
using PortalModels;
using PortalAPI_Service.Repositories.AccessRepos;
using System;
using System.Collections.Generic;
using System.Linq;

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
        [Route("{username}/{password}")]

        public IActionResult CheckUser(string username, string password)
        {
            var result = _loginRepo.CheckUser(username, password);
          
            return result == null ? NotFound(username) : (IActionResult)Ok(result);
        }



    }
}
