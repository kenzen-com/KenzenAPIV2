using Microsoft.AspNetCore.Mvc;
using AzureWrapper;
using Serilog;
using Microsoft.Extensions.Configuration;
using KenzenAPI.Classes;
using Newtonsoft.Json;
using System;

namespace KenzenAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : Controller
    {
        ILogger Logger;
        IConfiguration Config;
        public MessageController(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }
        string Cnxn = "DefaultEndpointsProtocol=https;AccountName=kenzenstorage1;AccountKey=O+C4dkXZAdI+8BNvfJ6iwPu9BB5jUmb+AIhL8w9XKm4RF5dXSSz5joP8WIJB6jz7ohiRdMD1fojOloor7RrpVw==;EndpointSuffix=core.windows.net";
        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            return Ok("hello");
        }

        /// <summary>
        ///  Expects a Message, sends that JSON to an Azure Queue
        /// </summary>
        [HttpPost]
        [APIBodyAuth("User")]
        [Route("Send")]
        public IActionResult Send(Message MessageObjectJSON)
        {
            AzureWrapper.ProcessResult p = new AzureWrapper.ProcessResult();
            try
            {
                string sQ = "kenzen-message-queue";
                AzureWrapper.IO.QueueIO q = new AzureWrapper.IO.QueueIO(Config, Logger);
                q.CreateQueueClient(Cnxn);
                string sM = JsonConvert.SerializeObject(MessageObjectJSON);
                p = q.Insert(Cnxn, sQ, sM).Result;
                if (p.Exception == null)
                    return Ok("sent");
                else
                    return BadRequest(p.Exception.Message);
            }
            catch(Exception e)
            {
                p.Exception = e;
            }

            return Ok(p);
        }
    }
}
