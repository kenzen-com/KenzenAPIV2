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
        ///  Accepts a Login object as JSON | Returns an APIUser object
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
        public IActionResult UpdatePassword(HttpRequestMessage RJSON)
        {
            int iUserID = 0;
            try
            {
                string s = RJSON.Content.ReadAsStringAsync().Result;
                JObject JSON = (JObject)JsonConvert.DeserializeObject(s);
                APIUserPassword oPW = JSON.ToObject<APIUserPassword>();
                iUserID = oPW.UserID;

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
        /// Accepts a APIUser object as JSON | Returns an ID
        /// </summary>
        [HttpPost]
        [Route("APIUser")]
        [APIBodyAuth("User")]
        public IActionResult APIUser(HttpRequestMessage MJSON)
        {
            int iID = -1;
            try
            {
                string s = MJSON.Content.ReadAsStringAsync().Result;
                JObject JSON = (JObject)JsonConvert.DeserializeObject(s);
                APIUser oModel = JSON.ToObject<APIUser>();

                ProcessResult oPR = oModel.Save();
                iID = Convert.ToInt32(oPR.Result);
                return Ok(oPR.Result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion User

        #region Users
        /// <summary>
        ///  Accepts a UserID in the Route URL | Fetches a list of Users
        /// </summary>
        [HttpGet]
        [Route("Users")]

        [APIRouteAuth("User")]
        public IActionResult APIUser(int UserID)
        {
            try
            {
                APIUserCollection u = new APIUserCollection();
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
        [Route("UserRoles/{ClientID}")]

        [APIRouteAuth("User")]
        public IActionResult UserRoles(int ClientID, int UserID)
        {
            try
            {
                List<APIUserRole> u = (List<APIUserRole>)KenzenAPI.DataClasses.APIUser.FetchRoles(UserID, ClientID, Config["CnxnString"], Config["LogPath"]).ObjectProcessed;
                return Ok(u);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion UserRoles

        #region Roles
        /// <summary>
        ///  Accepts a UserID in the Route URL | Fetches a list of Roles for the application
        /// </summary>
        [HttpGet]
        [Route("Roles")]

        [APIRouteAuth("User")]
        public IActionResult Roles(int UserID)
        {
            try
            {
                APIRoleCollection u = new APIRoleCollection(Logger, Config);
                return Ok(u);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion Roles
    }
}
