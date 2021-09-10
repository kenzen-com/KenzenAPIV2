using KenzenAPI.Classes.Lookup;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using KenzenAPI.DataClasses;

namespace KenzenAPI.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(AdminController))]
    [Route("[controller]")]
    public class AdminController : Controller
    {
        ILogger Logger;
        IConfiguration Config;
        public AdminController(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }

        [HttpGet]
        [APIRouteAuth("Admin")]
        [Route("DB/{UserID}")]
        public IActionResult DB()
        {
            SqlConnection x = new SqlConnection();
            try
            {
                x.ConnectionString = Config["CnxnString"];
                x.Open();
                if (x.State == System.Data.ConnectionState.Open)
                    return Ok("Open");
                else
                    return Ok("Nope");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                if (x.State == System.Data.ConnectionState.Open)
                    x.Close();
            }
        }

        #region Users
        /// <summary>
        ///  Accepts a UserID in the Route URL | Fetches a list of Users
        /// </summary>
        [HttpGet]
        [Route("Users/{UserID}")]
        [APIRouteAuth("User")]
        public IActionResult Users()
        {
            try
            {
                List<User> u = new UserCollection(Logger, Config).Values.ToList();
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
