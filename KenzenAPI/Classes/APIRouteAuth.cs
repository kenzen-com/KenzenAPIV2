
using KenzenAPI.Classes.Lookup;
using KenzenAPI.DataClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
namespace KenzenAPI
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class APIRouteAuthAttribute : Attribute, Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter
    {
        private string Role = "";
        public APIRouteAuthAttribute(string RoleIn)
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
                string sURL = filterContext.HttpContext.Request.Path.ToString();
                int iLast = sURL.LastIndexOf("/");
                int UserID = Convert.ToInt32(sURL.Substring(iLast).Trim().Replace("/", ""));
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