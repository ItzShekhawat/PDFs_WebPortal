using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PortalAPI_Service.Repositories.FoldersRepos;
using System;
using System.Threading.Tasks;

namespace PortalAPI_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewsController : ControllerBase
    {
        private readonly IFoldersRepo _foldersRepo;
        private readonly IMemoryCache _memoryCache;

        public ViewsController(IFoldersRepo foldersRepo, IMemoryCache memoryCache)
        {
            _foldersRepo = foldersRepo;
            _memoryCache = memoryCache;
          
        }



        [HttpGet("{tablename}")] // A Generic Api that can get the sub-Folders of ( Clients, Orders and Sub-Clients )
        public async Task<IActionResult> GetSubFolders(string tablename, string father_name)
        {
            
            try
            {
                if (!_memoryCache.TryGetValue(father_name, out var result))
                {
                    if(tablename == "pdf" || tablename == "PDF")
                    {
                        result = await _foldersRepo.GetPDFAsync(father_name);
                    }
                    else
                    { 
                        result = await _foldersRepo.GetSubFolders(father_name, tablename);
                    }

                    //Set in  cache
                    _memoryCache.Set(father_name, result, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [HttpGet("ordersCheck/{Search}")]
        public async Task<IActionResult> GetSearchValues(string Search)
        {
            Search = Search.ToUpper();
            try
            {
                if (!_memoryCache.TryGetValue(Search, out var result))
                {
                    result = await _foldersRepo.GetSearchList(Search);

                    //Set in  cache
                    _memoryCache.Set(Search, result, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(3)));
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /*
        [HttpGet]
        public async Task<IActionResult> GetClientsAsync()
        {
            try
            {
                var result = await _foldersRepo.GetClientsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(501, ex.Message);
            }
        }

        [HttpGet]
        [Route("Orders/{Client_name}")]
        public async Task<IActionResult> GetOrdersAsync(string Client_name)
        {
            try
            {
                var result = await _foldersRepo.GetOrdersAsync(Client_name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("Suborder/{Order}")]
        public async Task<IActionResult> GetSubordersAsync(string Order)
        {
            try
            {
                var result = await _foldersRepo.GetSubordersAsync(Order);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("pdf/{suborder}")]
        public async Task<IActionResult> GetPDFAsync(string suborder)
        {
            try
            {
                var result = await _foldersRepo.GetPDFAsync(suborder);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        */




        [HttpGet]
        [Route("pdf_file/{pdf_Id}")]
        public async Task<IActionResult> GetPDF_FileAsync(int pdf_Id)
        {
            try
            {
                var result = await _foldersRepo.GetPDF_FileAsync(pdf_Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



        [HttpGet]
        [Route("streamPDF/")]
        public IActionResult ShowPDF(string File_path)
        {
            Console.WriteLine(File_path);
            
            if (string.IsNullOrEmpty(File_path))
            {
                return StatusCode(501, File_path);
            }
            else
            {
                string[] slash_path = File_path.Trim().Split(@"\");
                string name = slash_path[^1];
                try
                {
                    //Console.WriteLine($"{PDF_INFO.FF_Name}\n{PDF_INFO.Location_path}\n{PDF_INFO.FK_Father}");

                    if (!_memoryCache.TryGetValue(File_path, out byte[] PDF_Bytes))
                    {
                        PDF_Bytes = System.IO.File.ReadAllBytes(File_path);

                        //var stream = new FileStream(PDF_INFO.Location_path, FileMode.Open, FileAccess.Read, FileShare.Read);
                        //StreamResult = new(stream, "application/pdf");

                        _memoryCache.Set(File_path, PDF_Bytes, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(2)));
                    }
                    HttpContext.Response.Headers.Add("Content-Disposition", "inline; filename="+name);

                    Console.WriteLine("Stream Done");
                    //Stream stream = new MemoryStream(PDF_Bytes);


                    return new FileContentResult(PDF_Bytes, "application/pdf");
                }
                catch (Exception ex)
                {
                    return StatusCode(501, ex.Message);
                }

            }
        }
       

    }
}