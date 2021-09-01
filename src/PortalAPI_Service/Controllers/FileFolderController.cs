using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PortalAPI_Service.Repositories.DirectoryRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalAPI_Service.Controllers
{
    [Route("api/[]")]
    [ApiController]
    public class FileFolderController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDirRepo _Drepo;

        public FileFolderController(IMemoryCache memoryCache, IDirRepo Drepo)
        {
            _memoryCache = memoryCache;
            _Drepo = Drepo;
        }


        [HttpGet]
        [Route("/path={F_path}")]
        private async Task<IActionResult> GetFolders(string F_path)
        {
            try
            {
                var Folders = await _Drepo.GetTheSubFolder(F_path);
                return Ok(Folders);
            }
            catch (Exception)
            {
                return StatusCode(501, F_path);
            }
        }
    }
}
