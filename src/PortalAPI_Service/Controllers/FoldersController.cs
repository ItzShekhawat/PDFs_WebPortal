using Microsoft.AspNetCore.Mvc;
using PortalAPI_Service.Repositories.FoldersRepos;
using PortalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace PortalAPI_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private readonly IFoldersRepo _foldersRepo;

        public FoldersController(IFoldersRepo foldersRepo)
        {
            _foldersRepo = foldersRepo;
            
        }


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
                return StatusCode(500, ex.Message);
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

        [HttpGet]
        [Route("pdf_file/{pdf}")]
        public async Task<IActionResult> GetPDF_FileAsync(string pdf)
        {
            try
            {
                var result = await _foldersRepo.GetSubordersAsync(pdf);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
