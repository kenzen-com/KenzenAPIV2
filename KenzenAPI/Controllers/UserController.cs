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
        ///  Accepts a Login object as JSON | Returns an User object
        /// </summary>
        [HttpPost]
        [Route("Login")]

        public IActionResult Login(Login LoginObjectJSON)
        {
            Login u = new Login();
            try
            {
                // expects username and password in body
                if (LoginObjectJSON == null)
                    return BadRequest("Login failed");

                u = (Login)Classes.Models.Login.LogMeIn(LoginObjectJSON.ClientID, LoginObjectJSON.Username, LoginObjectJSON.Password, Logger, Config).ObjectProcessed;
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
        public IActionResult UpdatePassword(UserPassword UserPasswordObjectJSON)
        {
            try
            {
                ProcessResult oPR = UserPasswordObjectJSON.Save(Config);
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
        public IActionResult User(User UserObjectJSON)
        {
            try
            {
                ProcessResult oPR = UserObjectJSON.Save(Config);
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
                List<UserRole> u = (List<UserRole>)KenzenAPI.DataClasses.User.FetchRoles(UserID, ClientID, Logger, Config).ObjectProcessed;
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
        [Route("Heartrates")]
        public IActionResult Heartrates(List<HeartRate> HeartRateListObjectJSON)
        {
            try
            {
                User u = new User(HeartRateListObjectJSON[0].UserID, Logger, Config);
                ProcessResult oPR = HeartRate.SaveList(HeartRateListObjectJSON, u.ClientID, Config);
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

        #region MedicalAnswers

        /// <summary>
        ///  Accepts a UserID in the Route URL | Fetches a list of UserMedicalAnswers 
        /// </summary>
        [HttpGet]
        [Route("MedicalAnswers/{ClientID}/{UserID}")]
        [APIRouteAuth("User")]
        public IActionResult UserMedicalAnswers(int ClientID, int UserID)
        {
            try
            {
                List<UserMedicalAnswer> u = new UserMedicalAnswerCollection(ClientID, UserID, Logger, Config).Values.ToList();
                return Ok(u);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion MedicalAnswers

        #region TeamUsers
        /// <summary>
        ///  Accepts a ClientID and a TeamID in the Route URL | Fetches a list of TeamUsers by Team
        /// </summary>
        [HttpGet]
        [Route("TeamUsers/{ClientID}/{TeamID}")]
        [APIRouteAuth("User")]
        public IActionResult TeamUsers(int ClientID, int TeamID)
        {
            try
            {
                ProcessResult oPR = Team.Users(ClientID, TeamID, Logger, Config);
                List<User> u = (List<User>)oPR.ObjectProcessed;
                return Ok(u);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion TeamUsers

        #region TeamUser
        /// <summary>
        ///  Accepts a TeamUser object | Assigns a User to a Team
        /// </summary>
        [HttpPost]
        [Route("TeamUser")]
        [APIRouteAuth("User")]
        public IActionResult TeamUser(TeamUser TeamUser)
        {
            try
            {
                ProcessResult oPR = Team.AssignToTeam(TeamUser.ClientID, TeamUser.TeamID, TeamUser.UserID, Logger, Config);
                return Ok(oPR.Result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion TeamUsers

    }
}
