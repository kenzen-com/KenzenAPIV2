using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace KenzenAPI.Controllers
{
    [ApiController]
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
        [Route("DB")]
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
    }
}
