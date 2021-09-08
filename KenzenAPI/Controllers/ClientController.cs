using KenzenAPI.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KenzenAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientController : Controller
    {
        ILogger Logger;
        IConfiguration Config;
        public ClientController(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }

        [HttpPost]
        [APIRouteAuth("User")]
        [Route("Save")]
        public IActionResult Save(Client c)
        {
            try
            {
                AzureWrapper.ProcessResult oPR = c.Save(Config["CnxnString"], Config["LogPath"]);
                if (oPR.Exception != null)
                    throw oPR.Exception;

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
