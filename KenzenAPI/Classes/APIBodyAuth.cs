using KenzenAPI.Classes.Lookup;
using KenzenAPI.DataClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
namespace KenzenAPI
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class APIBodyAuthAttribute : Attribute, Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter
    {
        private string Role = "";
        public APIBodyAuthAttribute(string RoleIn)
        {
            Init(RoleIn);
        }

        void Init(string RoleIn)
        {
            this.Role = RoleIn;

        }
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var httpResponse = new System.Net.Http.HttpResponseMessage();
            try
            {
                Stream strm = filterContext.HttpContext.Request.Body;
                StreamReader reader = new StreamReader(strm);
                string sJSON = reader.ReadToEnd();

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
                filterContext.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
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