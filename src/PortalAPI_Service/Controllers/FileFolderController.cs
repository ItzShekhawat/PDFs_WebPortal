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
    [Route("api/[controller]")]
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
        [Route("/path={F_path},{File_or_Folder}")]
        public async Task<IActionResult> GetFolders(string F_path, bool File_or_Folder)
        {
            try
            {
                var Folders_path = await _Drepo.GetTheSubFolder_File(F_path, File_or_Folder);
                return Ok(Folders_path);
            }
            catch (Exception)
            {
                return StatusCode(501, F_path);
            }
        }



        [HttpPost("/upDir/")] // This should Upload the Client/Orders/Suborders/PDF Table

        public async Task<IActionResult> Upload_Update_Folders([FromBody] List<string> DirList, string TableName, bool doUpdate)
        {
            var result =  await _Drepo.Upload_Update_Folders(DirList, TableName, doUpdate);
            return result ? Ok(result) : StatusCode(404, "Something wrong in Update/Upload Folder");

        }

        [HttpPost("/upFile/")] // This should Upload the Client/Orders/Suborders/PDF Table

        public async Task<IActionResult> Upload_UpDate_Files([FromBody] List<string> DirList, bool doUpdate)
        {
            var result = await _Drepo.Upload_Update_File(DirList, doUpdate);
            return result ? Ok(result) : StatusCode(404, "Something wrong in Update/Upload File");

        }





        //https://localhost:44315/path=Z%3A%5CSPAC%5CCommesse
        // "Z:\\SPAC\\Commesse\\ITT\\12ITT Macchina Pastiglie\\12001\\PDF\\Karni_Singh_Shekhawat_CV.pdf"
    }
}
