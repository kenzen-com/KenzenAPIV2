using AzureWrapper;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace KenzenAPI.Classes.Models
{
    public class Login
    {
        public ILogger Logger;
        public IConfiguration Config;
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string LastLoginDateTime { get; set; }
        public string PasswordCreatedDateTime { get; set; }
        public int ClientID { get; set; }
        public int UserID { get; set; }
        public Login()
        { }
        public Login(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }
        public static ProcessResult LogMeIn(int ClientID, string Username, string Password, string CnxnString, string LogPath)
        {

            Login oUser = new Login(null, null);
            ProcessResult oPR = new ProcessResult();
            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                SqlCommand cmd = new SqlCommand("spLogin", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for CPIUser
                cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.VarChar, 50));
                cmd.Parameters["@Username"].Value = Username ?? "";

                cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar, 100));
                cmd.Parameters["@Password"].Value = Crypto.HashMe(Password).Result ?? "";

                // assign output params
                cmd.Parameters.Add(new SqlParameter("@IDOut", SqlDbType.Int));
                cmd.Parameters["@IDOut"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add(new SqlParameter("@POut", SqlDbType.VarChar, 100));
                cmd.Parameters["@POut"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add(new SqlParameter("@DOut", SqlDbType.VarChar, 50));
                cmd.Parameters["@DOut"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add(new SqlParameter("@LOut", SqlDbType.VarChar, 50));
                cmd.Parameters["@LOut"].Direction = ParameterDirection.Output;

                cmd.Parameters.Add(new SqlParameter("@COut", SqlDbType.Int));
                cmd.Parameters["@COut"].Direction = ParameterDirection.Output;

                #endregion Parameters


                Cnxn.Open();
                cmd.ExecuteNonQuery();

                oUser.UserID = (int)cmd.Parameters["@IDOut"].Value;
                oUser.Password = cmd.Parameters["@POut"].Value.ToString();
                oUser.PasswordCreatedDateTime = cmd.Parameters["@DOut"].Value.ToString();
                oUser.LastLoginDateTime = cmd.Parameters["@LOut"].Value.ToString();
                oUser.ClientID = (int)cmd.Parameters["@COut"].Value;
                oUser.Username = Username;

                Cnxn.Close();
                oPR.ObjectProcessed = oUser;

                return (oPR);
            }
            catch (Exception Exc)
            {
                Log.LogErr("Login.LogMeIn", Exc.Message, LogPath);

                oPR.Exception = Exc;
                oPR.Result += "Error";
                return (oPR);
            }
        }
    }
}