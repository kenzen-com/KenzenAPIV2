
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

namespace KenzenAPI.DataClasses
{

    public class APIUserPasswordCollection : Dictionary<int, APIUserPassword>
    {
        public ILogger Logger;
        public IConfiguration Config;

        #region Constructors
        public APIUserPasswordCollection(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }


        public APIUserPasswordCollection(int ID, ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
            string CnxnString = Config["CnxnString"];
            string LogPath = Config["LogPath"];

            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                APIUserPassword oAPIUserPassword = new APIUserPassword(Logger, Config);
                SqlCommand cmd = new SqlCommand("SELECT * FROM [" + oAPIUserPassword.SchemaName + "].[" + oAPIUserPassword.TableName + "] WHERE UserID = @ID", Cnxn);

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    oAPIUserPassword = new APIUserPassword(logger, config);
                    oAPIUserPassword.CreatedDate = dr["CreatedDate"] == DBNull.Value ? "" : Convert.ToDateTime(dr["CreatedDate"]).ToUniversalTime().ToString("o").Trim();
                    oAPIUserPassword.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                    oAPIUserPassword.IsCurrent = dr["IsCurrent"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsCurrent"]);
                    oAPIUserPassword.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    oAPIUserPassword.Password = dr["Password"] == DBNull.Value ? "" : dr["Password"].ToString().Trim();
                    if (!this.ContainsKey(oAPIUserPassword.ID))
                        this.Add(oAPIUserPassword.ID, oAPIUserPassword);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("APIUserPasswordCollectionConstructor", Exc.Message, LogPath);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }
        }

        #endregion Constructors

    }



    public class APIUserPassword : DataClassBase
    {

        #region Vars

        int _UserID;

        string _CreatedDate;
        bool _IsCurrent;
        string _Password;

        #endregion Vars

        #region Get/Sets

        public int UserID
        {
            get { return (_UserID); }
            set { _UserID = value; }
        }
        public string CreatedDate
        {
            get { return (Convert.ToDateTime(_CreatedDate).ToString("o")); }
            set { _CreatedDate = value; }
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

        #endregion Get/Sets

        #region Constructors

        public APIUserPassword(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
            TableName = "APIUserPasswords";

        }

        public APIUserPassword(int ID, ILogger logger, IConfiguration config) : this(logger, config)
        {
            string CnxnString = Config["CnxnString"];
            string LogPath = Config["LogPath"];

            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                SqlCommand cmd = new SqlCommand("SELECT * FROM [" + this.SchemaName + "].[" + this.TableName + "] WHERE UserID = @ID AND IsCurrent = 1", Cnxn);

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    this.CreatedDate = dr["CreatedDate"] == DBNull.Value ? "" : Convert.ToDateTime(dr["CreatedDate"]).ToUniversalTime().ToString("o").Trim();
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
                Log.LogErr("APIUserPasswordConstructor", Exc.Message, LogPath);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }


        }

        #endregion Constructors

        #region Save
        public ProcessResult Save()
        {
            string CnxnString = Config["CnxnString"];
            string LogPath = Config["LogPath"];

            ProcessResult oPR = new ProcessResult();
            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                SqlCommand cmd = new SqlCommand("[" + this.SchemaName + "].[spPasswordSave]", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for APIUserPasswords
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
                Log.LogErr("APIUserPasswordSave", Exc.Message, LogPath);

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

                SqlCommand cmd = new SqlCommand("DELETE FROM [" + this.SchemaName + "].[" + this.TableName + "] WHERE ID = @ID", Cnxn);

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();
                return (true);
            }
            catch (Exception Exc)
            {
                Log.LogErr("APIUserPasswordDelete", Exc.Message, LogPath);
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