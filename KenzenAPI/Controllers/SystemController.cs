﻿using KenzenAPI.Classes.Lookup;
using KenzenAPI.DataClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace KenzenAPI.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(SystemController))]
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
        public IActionResult Index()
        {
            return View();
        }

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
  
    }
}
