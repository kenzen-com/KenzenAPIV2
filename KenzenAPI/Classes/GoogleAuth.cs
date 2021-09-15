using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using KenzenAPI.Classes.Models;
using KenzenAPI.DataClasses;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KenzenAPI.Classes
{
    public class GoogleAuth
    {
        public ILogger Logger;
        public IConfiguration Config;
        protected string googleplus_client_id = "458878619548-khuatamj3qpiccnsm4q6dbulf13jumva.apps.googleusercontent.com";    // Replace this with your Client ID
        protected string googleplus_client_secret = "4hiVJYlomswRd_PV5lyNQlfN";                                                // Replace this with your Client Secret
        protected string googleplus_redirect_url = "http://localhost:2443/Index.aspx";                                         // Replace this with your Redirect URL; Your Redirect URL from your developer.google application should match this URL.
        protected string Parameters;

        static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static string ApplicationName = "Kenzen API";


        public GoogleAuth(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }

        public string Connect()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                UserCredential credential;

                using (var stream =
                    new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    sb.Append("Credential file saved to: " + credPath);
                }

                // Create Gmail API service.
                var service = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                // Define parameters of request.
                UsersResource.LabelsResource.ListRequest request = service.Users.Labels.List("me");

                // List labels.
                IList<Label> labels = request.Execute().Labels;
                sb.Append("Labels:");
                if (labels != null && labels.Count > 0)
                {
                    foreach (var labelItem in labels)
                    {
                        sb.Append(labelItem.Name);
                    }
                }
                else
                {
                    sb.Append("No labels found.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
            return sb.ToString();
        }

        private async Task<ProcessResult> GetGoogleplusUserData(string access_token)
        {
            ProcessResult oPR = new ProcessResult();
            try
            {
                        User oUser = new User();
                HttpClient client = new HttpClient();
                var urlProfile = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + access_token;

                client.CancelPendingRequests();
                HttpResponseMessage output = await client.GetAsync(urlProfile);

                if (output.IsSuccessStatusCode)
                {
                    string outputData = await output.Content.ReadAsStringAsync();
                    GoogleUserOutputData oGoogleUserData = JsonConvert.DeserializeObject<GoogleUserOutputData>(outputData);

                    if (oGoogleUserData != null)
                    {
                        oUser.LastName = oGoogleUserData.name; // etc
                    }
                }
                oPR.ObjectProcessed = oUser;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                oPR.Exception = ex;
            }

            return oPR;
        }
        public async Task<UserCredential> Auth()
        {
            UserCredential credential;
            string[] scopes = new string[] { }; // user basic profile

            //Read client id and client secret from Web config file

            credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                       new ClientSecrets
                       {
                           ClientId = Config["ClientId"],
                           ClientSecret = Config["ClientSecret"]
                       }, scopes,
                "user", CancellationToken.None, new FileDataStore("Auth.Api.Store"));

            return credential;
        }
    }
}
