using KenzenAPI.Classes.Lookup;
using KenzenAPI.DataClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using KenzenAPI.Classes;
using System.IO;

namespace KenzenAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SystemController : Controller
    {
        ILogger Logger;
        IConfiguration Config;
        public SystemController(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }
        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        ///  Possible Lookup (list) types fetched are AlertResponse, AlertStage, Clothing, DailuyFeedbackAnwser, Environment, MedicalAnswer
        ///  NavigationSource, RecAction, RiskLevel, SunExposure, SystemCheckerAnswer and WorkLevel
        /// </summary>
        [HttpGet]
        [APIRouteAuth("User")]
        [Route("LookupList/{LookupType}/{UserID}")]
        public IActionResult LookupList(string LookupType)
        {
            try
            {
                switch (LookupType.ToUpper())
                {
                    case "ALERTREPONSE":
                        AlertResponse a = new AlertResponse();
                        return Ok(a.FetchAll(Config).ObjectProcessed);
                    case "ALERTSTAGE":
                        AlertStage b = new AlertStage();
                        return Ok(b.FetchAll(Config).ObjectProcessed);
                    case "CLOTHING":
                        Clothing c = new Clothing();
                        return Ok(c.FetchAll(Config).ObjectProcessed);
                    case "DAILYFEEDBACKANSWER":
                        DailyFeedbackAnswer d = new DailyFeedbackAnswer();
                        return Ok(d.FetchAll(Config).ObjectProcessed);
                    case "ENVIRONMENT":
                        Classes.Lookup.Environment e = new Classes.Lookup.Environment();
                        return Ok(e.FetchAll(Config).ObjectProcessed);
                    case "MEDICALANSWER":
                        MedicalAnswer f = new MedicalAnswer();
                        return Ok(f.FetchAll(Config).ObjectProcessed);
                    case "NAVIGATIONSOURCE":
                        NavigationSource g = new NavigationSource();
                        return Ok(g.FetchAll(Config).ObjectProcessed);
                    case "RECACTION":
                        RecAction h = new RecAction();
                        return Ok(h.FetchAll(Config).ObjectProcessed);
                    case "RISKLEVEL":
                        RiskLevel j = new RiskLevel();
                        return Ok(j.FetchAll(Config).ObjectProcessed);
                    case "SUNEXPOSURE":
                        SunExposure k = new SunExposure();
                        return Ok(k.FetchAll(Config).ObjectProcessed);
                    case "SYSTEMCHECKERANSWER":
                        SystemCheckerAnswer l = new SystemCheckerAnswer();
                        return Ok(l.FetchAll(Config).ObjectProcessed);
                    case "WORKLEVEL":
                        WorkLevel m = new WorkLevel();
                        return Ok(m.FetchAll(Config).ObjectProcessed);
                    case "ROLE":
                        Role n = new Role();
                        return Ok(n.FetchAll(Config).ObjectProcessed);

                }

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [APIRouteAuth("User")]
        [Route("JSON/{UserID}")]
        public IActionResult JSON()
        {
            string sOut = "Client" + System.Environment.NewLine;
            sOut += new Client(null, null).ToJson() + System.Environment.NewLine;
            sOut += "User" + System.Environment.NewLine;
            sOut += new User().ToJson() + System.Environment.NewLine;
            sOut += "Message" + System.Environment.NewLine;
            sOut += new Message(null, null).ToJson() + System.Environment.NewLine;
            sOut += "HeartRate" + System.Environment.NewLine;
            sOut += new HeartRate(null, null).ToJson() + System.Environment.NewLine;
            sOut += "MaxEnvironmental" + System.Environment.NewLine;
            sOut += new MaxEnvironmental(null, null).ToJson() + System.Environment.NewLine;
            sOut += "TemperatureHumidity" + System.Environment.NewLine;
            sOut += new TemperatureHumidity(null, null).ToJson() + System.Environment.NewLine;
            sOut += "ConnectionStatus" + System.Environment.NewLine;
            sOut += new ConnectionStatus(null, null).ToJson() + System.Environment.NewLine;
            sOut += "WorkRest" + System.Environment.NewLine;
            sOut += new WorkRest(null, null).ToJson() + System.Environment.NewLine;

            return Ok(sOut);
        }

        [HttpGet]
        [APIRouteAuth("User")]
        [Route("DownloadApplication/{UserID}")]
        public async Task<FileStreamResult> DownloadApplication()
        {
            var path = "~/Files/Kenzen.zip";
            var stream = System.IO.File.OpenRead(path);
            return new FileStreamResult(stream, "application/octet-stream");
        }
    }
}
