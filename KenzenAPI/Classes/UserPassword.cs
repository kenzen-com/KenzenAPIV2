
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Configuration;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Serilog;
using KenzenAPI.Classes;
using AzureWrapper;
using System.Linq;

namespace KenzenAPI.DataClasses
{

    public class UserPasswordCollection : Dictionary<int, UserPassword>
    {
        public ILogger Logger;
        public IConfiguration Config;

        #region Constructors
        public UserPasswordCollection(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }


        public UserPasswordCollection(int ID, ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
            string CnxnString = Config["CnxnString"];
            string LogPath = Config["LogPath"];

            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                UserPassword oUserPassword = new UserPassword(Logger, Config);
                SqlCommand cmd = new SqlCommand("SELECT * FROM " + oUserPassword.TableName + " WHERE UserID = @ID", Cnxn);

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    oUserPassword = new UserPassword(logger, config);
                    oUserPassword.UTC = dr["UTC"] == DBNull.Value ? "" : Convert.ToDateTime(dr["UTC"]).ToUniversalTime().ToString("o").Trim();
                    oUserPassword.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                    oUserPassword.IsCurrent = dr["IsCurrent"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsCurrent"]);
                    oUserPassword.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    oUserPassword.Password = dr["Password"] == DBNull.Value ? "" : dr["Password"].ToString().Trim();
                    if (!this.ContainsKey(oUserPassword.ID))
                        this.Add(oUserPassword.ID, oUserPassword);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error("UserPasswordCollectionConstructor", Exc.Message, LogPath);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }
        }

        #endregion Constructors

    }



    public class UserPassword : DataClassBase
    {

        #region Vars

        int _UserID;


        bool _IsCurrent;
        string _Password;

        #endregion Vars

        #region Get/Sets

        public int UserID
        {
            get { return (_UserID); }
            set { _UserID = value; }
        }
  
        public bool IsCurrent
        {
            get { return (_IsCurrent); }
            set { _IsCurrent = value; }
        }

        public string Password
        {
            get { return (_Password); }
            set { _Password = value; }
        }

        public bool IsExpired
        {
            get
            {
                TimeSpan ts = DateTime.Now - Convert.ToDateTime(this.UTC);
                if (ts.Days > Convert.ToInt32(Config["PasswordExpDays"]))
                    return true;
                else
                    return
                        false;
            }
        }

        #endregion Get/Sets

        #region Constructors
        public UserPassword() { }

        public UserPassword(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
            TableName = "UserPasswords";

        }

        public UserPassword(int ID, ILogger logger, IConfiguration config) : this(logger, config)
        {

            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("SELECT * FROM " + this.TableName + " WHERE UserID = @ID AND IsCurrent = 1", Cnxn);

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    this.UTC = dr["UTC"] == DBNull.Value ? "" : Convert.ToDateTime(dr["UTC"]).ToUniversalTime().ToString("o").Trim();
                    this.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                    this.IsCurrent = dr["IsCurrent"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsCurrent"]);
                    this.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    this.Password = dr["Password"] == DBNull.Value ? "" : dr["Password"].ToString().Trim();
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error("UserPasswordConstructor", Exc.Message, Config["LogPath"]);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }


        }

        #endregion Constructors

        #region Save
        public ProcessResult Save(IConfiguration config)
        {
            Config = config;
            ProcessResult oPR = new ProcessResult();
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {
                List<UserPassword> l = new UserPasswordCollection(ID, Logger, Config).Values.ToList();
                UserPassword p = l.Find(q => q.Password == AzureWrapper.Crypto.HashMe(this.Password).Result);
                if(p != null)
                {
                    oPR.Result = "Cannot re-use password";
                    return (oPR);
                }

                SqlCommand cmd = new SqlCommand("spPasswordSave", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for UserPasswords
                cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int));
                cmd.Parameters["@UserID"].Value = this.UserID;

                cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar, 100));
                cmd.Parameters["@Password"].Value = Crypto.HashMe(this.Password).Result ?? "";

                // assign output param
                cmd.Parameters.Add(new SqlParameter("@IDOut", SqlDbType.Int));
                cmd.Parameters["@IDOut"].Direction = ParameterDirection.Output;

                #endregion Parameters

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();

                int iID = Convert.ToInt32(cmd.Parameters["@IDOut"].Value);
                this.ID = iID;

                oPR.ObjectProcessed = this;
                oPR.Result += "Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Logger.Error("UserPasswordSave", Exc.Message, Config["LogPath"]);

                oPR.Exception = Exc;
                oPR.Result += "Error";
                return (oPR);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }
        }
        #endregion Save

        #region Delete


        public bool Delete()
        {
            string CnxnString = Config["CnxnString"];
            string LogPath = Config["LogPath"];

            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                SqlCommand cmd = new SqlCommand("DELETE FROM " + this.TableName + " WHERE ID = @ID", Cnxn);

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();
                return (true);
            }
            catch (Exception Exc)
            {
                Logger.Error("UserPasswordDelete", Exc.Message, LogPath);
                return (false);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }


        }
        #endregion Delete

    }
}