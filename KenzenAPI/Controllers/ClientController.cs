using KenzenAPI.Classes;
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
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(ClientController))]
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
                AzureWrapper.ProcessResult oPR = c.Save(Client.GetCnxnString(c.ClientID, Config), Config["LogPath"]);
                if (oPR.Exception != null)
                    throw oPR.Exception;

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        #region Users
        /// <summary>
        ///  Accepts a ClientID and a UserID in the Route URL | Fetches a list of Users by Client
        /// </summary>
        [HttpGet]
        [Route("Users/{ClientID}/{UserID}")]
        [APIRouteAuth("User")]
        public IActionResult Users(int ClientID)
        {
            try
            {
               AzureWrapper.ProcessResult oPR = Client.Users(ClientID, Logger, Config);
                List<User> u = (List<User>)oPR.ObjectProcessed;
                return Ok(u);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion Users
    }
}
