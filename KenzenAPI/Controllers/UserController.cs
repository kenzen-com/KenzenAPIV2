using KenzenAPI.Classes.Models;
using KenzenAPI.DataClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace KenzenAPI.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(UserController))]
    [Route("[controller]")]
    public class UserController : Controller
    {

        ILogger Logger;
        IConfiguration Config;
        public UserController(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }


        #region Login
        /// <summary>
        ///  Accepts a Login object as JSON | Returns an User object
        /// </summary>
        [HttpPost]
        [Route("Login")]

        public IActionResult Login(Login L)
        {
            Login u = new Login();
            try
            {
                // expects username and password in body
                if (L == null)
                    return BadRequest("Login failed");

                u = (Login)Classes.Models.Login.LogMeIn(L.ClientID, L.Username, L.Password, Config["CnxnString"], Config["LogPath"]).ObjectProcessed;
                if (u != null && u.UserID > 0)
                {
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["JWTKey"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(Config["JWTIssuer"],
                      Config["JWTIssuer"],
                      null,
                      expires: DateTime.Now.AddMinutes(120),
                      signingCredentials: credentials);

                    string sToken = TokenManager.GenerateToken(u.UserID.ToString());
                    u.Token = sToken;

                    return Ok(u);
                }
                else
                    return BadRequest("Login failed");

            }
            catch (Exception Exc)
            {
                return BadRequest(Exc.Message);
            }
        }

        #endregion Login

        #region UpdatePassword
        /// <summary>
        ///  Accepts a UserID in the Route URL | Updates a User Password for the User ID in the JSON object
        /// </summary>
        [HttpPost]
        [Route("UpdatePassword")]
        [APIRouteAuth("User")]
        public IActionResult UpdatePassword(UserPassword oPW)
        {
            try
            {
                ProcessResult oPR = oPW.Save();
                if (oPR.Exception != null)
                    throw oPR.Exception;

                return Ok();

            }
            catch (Exception Exc)
            {
                return (BadRequest(Exc.Message));
            }
        }

        #endregion UpdatePassword

        #region User
        /// <summary>
        /// Accepts a User object as JSON | Returns an ID
        /// </summary>
        [HttpPost]
        [Route("User")]
        [APIBodyAuth("User")]
        public IActionResult User(User oModel)
        {
            try
            {
                ProcessResult oPR = oModel.Save();
                return Ok(oPR.Result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion User

        #region User
        /// <summary>
        ///  Accepts a UserID in the Route URL | Fetches a User
        /// </summary>
        [HttpGet]
        [Route("User/{UserID}")]
        [APIRouteAuth("User")]
        public IActionResult User(int UserID)
        {
            try
            {
                User u = new User(UserID, Logger, Config);
                return Ok(u);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion Users

        #region UserRoles
        /// <summary>
        ///  Accepts a UserID in the Route URL | Fetches a list of UserRoles for the UserID in that Program(ID)
        /// </summary>
        [HttpGet]
        [Route("UserRoles/{ClientID}/{UserID}")]

        [APIRouteAuth("User")]
        public IActionResult UserRoles(int ClientID, int UserID)
        {
            try
            {
                List<UserRole> u = (List<UserRole>)KenzenAPI.DataClasses.User.FetchRoles(UserID, ClientID, Config["CnxnString"], Config["LogPath"]).ObjectProcessed;
                return Ok(u);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion UserRoles

        #region Heartrates
        /// <summary>
        ///  Accepts a List of HeartRates as JSON
        /// </summary>
        [HttpPost]
        [APIRouteAuth("User")]
        [Route("Heartrates/{UserID}")]
        public IActionResult Heartrates(List<HeartRate> l, int UserID)
        {
            try
            {
                User u = new User(UserID, Logger, Config);
                ProcessResult oPR = HeartRate.SaveList(l, u.ClientID, Config);
                if (oPR.Exception == null)
                    return Ok("Saved");
                else
                    return BadRequest(oPR.Exception.Message);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        ///  Accepts a UserID in the Route URL | Fetches a list of HeartRates for the application
        /// </summary>
        [HttpGet]
        [Route("Heartrates/{ClientID}/{UserID}")]
        [APIRouteAuth("User")]
        public IActionResult Heartrates(int ClientID)
        {
            try
            {
                HeartRateCollection u = new HeartRateCollection(Logger, Config, ClientID);
                return Ok(u);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion Heartrates
    }
}
