using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;
using PortalAPI_Service.Repositories.FoldersRepos;
using PortalModels;
using System;
using System.Net;
using System.Net.Http;
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


        [HttpGet("{father_name}/{tablename}")] // A Generic Api that can get the sub-Folders of ( Clients, Orders and Sub-Clients )
        public async Task<IActionResult> GetSubFolders(string father_name, string tablename)
        {
            try
            {
                if (!_memoryCache.TryGetValue(tablename, out var result))
                {
                    result = await _foldersRepo.GetSubFolders(father_name, tablename);

                    //Set cache
                    _memoryCache.Set(tablename, result, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(3)));
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

        */



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
        [Route("streamPDF")]
        public async Task<IActionResult> ShowPDF()
        {

            try
            {
                //Console.WriteLine($"{PDF_INFO.FF_Name}\n{PDF_INFO.Location_path}\n{PDF_INFO.FK_Father}");

                if(!_memoryCache.TryGetValue("PDF", out byte[] PDF_Bytes)){

                    string pdfFilePath = @"Z:\SPAC\Commesse\SIAB\305 Macchina Trote\305001\PDF\hello.pdf";
                    PDF_Bytes = System.IO.File.ReadAllBytes(pdfFilePath);

                    //var stream = new FileStream(PDF_INFO.Location_path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    //StreamResult = new(stream, "application/pdf");

                    _memoryCache.Set("PDF", PDF_Bytes, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1)));

                }

                Console.WriteLine("Stream Done");

                return new FileContentResult(PDF_Bytes, "application/pdf");

            }
            catch (Exception ex)
            {
                return StatusCode(501, ex.Message);
            }

        }
    }
}