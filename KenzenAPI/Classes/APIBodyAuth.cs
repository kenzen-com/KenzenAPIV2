using KenzenAPI.Classes.Lookup;
using KenzenAPI.DataClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http.Controllers;
namespace KenzenAPI
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class APIBodyAuthAttribute : Attribute, IAuthorizationFilter
    {
        IConfiguration Configuration;
        private string Role = "";
        public APIBodyAuthAttribute(string RoleIn)
        {
            Init(RoleIn);
        }

        void Init(string RoleIn)
        {
            this.Role = RoleIn;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();


        }
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            try
            {
                filterContext.HttpContext.Request.EnableBuffering();
                Stream strm = filterContext.HttpContext.Request.Body;
                string sJSON = "";

                using (var buffer = new MemoryStream())
                {
                    filterContext.HttpContext.Request.Body.CopyToAsync(buffer);

                    // Move the buffer to the beginning of the stream before reading
                    buffer.Position = 0;

                    // Do the same for the original request stream
                    if (filterContext.HttpContext.Request.Body.CanSeek && filterContext.HttpContext.Request.Body.Position != 0)
                    {
                        filterContext.HttpContext.Request.Body.Position = 0;
                    }
                    StreamReader reader = new StreamReader(buffer);
                    sJSON = reader.ReadToEndAsync().Result;
                }
                dynamic d = JsonConvert.DeserializeObject(sJSON);
                int UserID = 0;
                try
                {
                    UserID = Convert.ToInt32(d.UserID);
                }
                catch (Exception e1)
                {
                    JArray jList = (JArray)JsonConvert.DeserializeObject(sJSON);
                    List<dynamic> dList = jList.ToObject<List<dynamic>>();
                    UserID = dList[0].UserID;
                }

                string sToken = filterContext.HttpContext.Request.Headers["Authorization"].ToList()[0].ToString().Replace("Bearer ", "");
                bool bOK = Validate(sToken, UserID);
                bool bAuth = false;
                if (bOK)
                {
                    Role r = QuickCache.Roles.Find(q => q.Name.ToUpper() == this.Role.ToUpper());
                    UserRole u = QuickCache.UserRoles.Find(q => q.UserID == UserID && q.RoleID == r.ID);
                    if (u != null)
                        bAuth = true;
                }


                if (bOK && bAuth)
                {

                }
                else
                {
                    filterContext.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                }

            }
            catch (Exception e1)
            {

                filterContext.HttpContext.Response.WriteAsync(e1.Message);
                filterContext.Result = new StatusCodeResult(StatusCodes.Status417ExpectationFailed);
            }

            return;
        }

        #region Validate
        protected bool Validate(string Token, int UserID)
        {
            if (Token.Length == 0) return false;

            string tokenUsername = TokenManager.ValidateToken(Token);

            if (tokenUsername == null) return false;

            if (UserID.ToString().Equals(tokenUsername))
            {
                return true;
            }
            return false;
        }
        #endregion Validate
    }
}