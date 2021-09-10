
using KenzenAPI.Classes.Lookup;
using KenzenAPI.DataClasses;
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
    public class APIRouteAuthAttribute : AuthorizationFilterAttribute
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
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var httpResponse = new System.Net.Http.HttpResponseMessage();
            try
            {
                string sURL = actionContext.Request.RequestUri.ToString();
                int iLast = sURL.LastIndexOf("/");
                int UserID = Convert.ToInt32(sURL.Substring(iLast).Trim().Replace("/", ""));
                string sToken = actionContext.Request.Headers.GetValues("Authorization").ToList()[0].ToString().Replace("Bearer ", "");
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
                    httpResponse.StatusCode = HttpStatusCode.OK;
                }
                else
                {
                    httpResponse.StatusCode = HttpStatusCode.Unauthorized;
                    actionContext.Response = httpResponse;
                }

            }
            catch (Exception e1)
            {
                httpResponse.StatusCode = HttpStatusCode.BadRequest;
                actionContext.Response = httpResponse;
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