using Microsoft.AspNetCore.Mvc;
using AzureWrapper;
using Serilog;
using Microsoft.Extensions.Configuration;
using KenzenAPI.Classes;
using Newtonsoft.Json;

namespace KenzenAPI.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(MessageController))]
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
        public IActionResult Index()
        {
            return Ok("hello");
        }

        /// <summary>
        ///  Expects a Message, sends that JSON to an Azure Queue
        /// </summary>
        [HttpGet]
        [APIBodyAuth("User")]
        [Route("Send")]
        public IActionResult Send(Message M)
        {
            string sQ = "kenzen-message-queue";
            AzureWrapper.IO.QueueIO q = new AzureWrapper.IO.QueueIO(Config, Logger);
            q.CreateQueueClient(Cnxn);
            string sM = JsonConvert.SerializeObject(M);
            AzureWrapper.ProcessResult p = q.Insert(Cnxn, sQ, sM).Result;
            if (p.Exception == null)
                return Ok("sent");
            else
                return BadRequest(p.Exception.Message);
        }
    }
}
