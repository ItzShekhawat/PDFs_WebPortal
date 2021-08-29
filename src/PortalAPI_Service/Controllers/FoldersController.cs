using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace PortalAPI_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        // GET: api/<FoldersController>
        [HttpGet]
        public IEnumerable<string> GetClients()
        {
            
        }

        // GET api/<FoldersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<FoldersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<FoldersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FoldersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
