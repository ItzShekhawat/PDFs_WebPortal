﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using PortalAPI_Service.Repositories.FoldersRepos;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PortalAPI_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private readonly IFoldersRepo _foldersRepo;
        private readonly IMemoryCache _memoryCache;

        public FoldersController(IFoldersRepo foldersRepo, IMemoryCache memoryCache)
        {
            _foldersRepo = foldersRepo;
            _memoryCache = memoryCache;

        }


        [HttpGet("{father_name}/{tablename}")] // A Generic Api that can get the sub-Folders of ( Clients, Orders and Sub-Clients )
        public async Task<IActionResult> GetSubFolders(string father_name, string tablename)
        {
            try
            {
                if(!_memoryCache.TryGetValue(tablename, out var result))
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
        [Route("pdfShow/path={pdf_path},name={pdf_name}")]
        public async Task<IActionResult> ShowPDF(string pdf_path, string pdf_name)
        {


            try
            {
                Console.WriteLine(pdf_path + pdf_name + ".pdf");
                var stream = new FileStream(pdf_path + pdf_name + ".pdf", FileMode.Open);
                FileStreamResult StreamResponse = new(stream, "application/pdf");
                Console.WriteLine(StreamResponse.ToString());
                return StreamResponse;

            }
            catch (Exception ex)
            {
                return StatusCode(501, ex.Message);
            }

        }
    }
}