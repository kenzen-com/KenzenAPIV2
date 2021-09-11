using KenzenAPI.DataClasses;
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
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(MessageController))]
    [Route("[controller]")]
    public class StatsController : Controller
    {
        ILogger Logger;
        IConfiguration Config;
        public StatsController(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }
        string Cnxn = "DefaultEndpointsProtocol=https;AccountName=kenzenstorage1;AccountKey=O+C4dkXZAdI+8BNvfJ6iwPu9BB5jUmb+AIhL8w9XKm4RF5dXSSz5joP8WIJB6jz7ohiRdMD1fojOloor7RrpVw==;EndpointSuffix=core.windows.net";
        public IActionResult Index()
        {
            return Ok("hello");
        }
        [APIBodyAuth("User")]
        [Route("HR")]
        [HttpPost]
        public IActionResult HR(HeartRate r)
        {
            ProcessResult p = r.Save();
            if (p.Exception == null)
                return Ok(r.ID);
            else
                return BadRequest(p.Exception.Message);
        }
        /// <summary>
        ///  Expects a list of HeartRates
        /// </summary>
        [APIBodyAuth("User")]
        [Route("HRList")]
        [HttpPost]
        public IActionResult HRList(List<HeartRate> r)
        {

            ProcessResult p = HeartRate.SaveList(r, r[0].ClientID, Config);
            if (p.Exception == null)
                return Ok("Saved");
            else
                return BadRequest(p.Exception.Message);
        }
        /// <summary>
        ///  Expects a Client ID in the URI and returns a list of HeartRates
        /// </summary>
        [APIBodyAuth("User")]
        [Route("HR/{ClientID}")]
        [HttpGet]
        public IActionResult HRFetch(int ClientID)
        {
            try
            {
                List<HeartRate> r = new List<HeartRate>();
                r.AddRange(new HeartRateCollection(Logger, Config, ClientID).Values.ToList());
                return Ok(r);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        ///  Expects a Client ID and a UserID in the URI and returns a list of HeartRates
        /// </summary>
        [APIBodyAuth("User")]
        [Route("HR/{ClientID}/{UserID}")]
        [HttpGet]
        public IActionResult HRFetch(int ClientID, int UserID)
        {
            try
            {
                List<HeartRate> r = new List<HeartRate>();
                r.AddRange(new HeartRateCollection(Logger, Config, ClientID).Values.ToList());
                r = r.FindAll(h => h.UserID == UserID);
                return Ok(r);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        /// <summary>
        ///  Expects a TemperatureHumidity object
        /// </summary>
        [APIBodyAuth("User")]
        [Route("TH")]
        [HttpPost]
        public IActionResult TH(TemperatureHumidity t)
        {
            ProcessResult p = t.Save();
            if (p.Exception == null)
                return Ok(t.ID);
            else
                return BadRequest(p.Exception.Message);
        }

        /// <summary>
        ///  Expects a MaxEnvironmental object
        /// </summary>
        [APIBodyAuth("User")]
        [Route("ME")]
        [HttpPost]
        public IActionResult ME(MaxEnvironmental m)
        {
            ProcessResult p = m.Save();
            if (p.Exception == null)
                return Ok(m.ID);
            else
                return BadRequest(p.Exception.Message);
        }
    }
}
