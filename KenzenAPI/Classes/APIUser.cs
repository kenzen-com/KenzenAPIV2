using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.Configuration;
using KenzenAPI.Classes;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace KenzenAPI.DataClasses
{

    public class APIUserCollection : Dictionary<int, APIUser>
    {
        public ILogger Logger;
        public IConfiguration Config;

        #region Constructors


        public APIUserCollection()
        {
            string CnxnString = Config["CnxnString"];
            string LogPath = Config["LogPath"];

            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                APIUser oAPIUser = new APIUser(Logger, Config);
                SqlCommand cmd = new SqlCommand("[" + oAPIUser.SchemaName + "].[spUsersFetch]", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    oAPIUser = new APIUser(Logger, Config);
                    oAPIUser.Username = dr["Username"] == DBNull.Value ? "" : dr["Username"].ToString().Trim();
                    oAPIUser.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    oAPIUser.FirstName = dr["FirstName"] == DBNull.Value ? "" : dr["FirstName"].ToString().Trim();
                    oAPIUser.ClientID = dr["ClientID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ClientID"]);
                    oAPIUser.RoleID = dr["RoleID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["RoleID"]);
                    oAPIUser.LastEditDate = dr["LastEditDate"] == DBNull.Value ? "" : Convert.ToDateTime(dr["LastEditDate"]).ToUniversalTime().ToString("o").Trim();
                    oAPIUser.RoleName = dr["Name"] == DBNull.Value ? "" : dr["Name"].ToString().Trim();
                    oAPIUser.CreatedDate = dr["CreatedDate"] == DBNull.Value ? "" : Convert.ToDateTime(dr["CreatedDate"]).ToUniversalTime().ToString("o").Trim();
                    oAPIUser.LastName = dr["LastName"] == DBNull.Value ? "" : dr["LastName"].ToString().Trim();
                    oAPIUser.LastLoginDate = dr["LastLoginDateTime"] == DBNull.Value ? "" : dr["LastLoginDateTime"].ToString().Trim();
                    oAPIUser.LastEditBy = dr["LastEditBy"] == DBNull.Value ? 0 : Convert.ToInt32(dr["LastEditBy"]);

                    if (!this.ContainsKey(oAPIUser.ID))
                        this.Add(oAPIUser.ID, oAPIUser);

                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("APIUserCollectionConstructor", Exc.Message, LogPath);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }
        }

        #endregion Constructors

    }



    public class APIUser : DataClassBase
    {

        #region Vars

        string _FirstName;
        string _LastName;
        string _LastLoginDate;
        string _LastEditDate;
        int _LastEditBy;
        string _CreatedDate;

        string _Username;
        int _ClientID;
        int _RoleID;
        string _RoleName;
        string _token;

        #endregion Vars

        #region Get/Sets

        public string FirstName
        {
            get { return (_FirstName); }
            set { _FirstName = value; }
        }

        public string LastName
        {
            get { return (_LastName); }
            set { _LastName = value; }
        }

        public string LastLoginDate
        {
            get { return (Convert.ToDateTime(_LastLoginDate).ToString("o")); }
            set { _LastLoginDate = value; }
        }

        public string LastEditDate
        {
            get { return (Convert.ToDateTime(_LastEditDate).ToString("o")); }
            set { _LastEditDate = value; }
        }
        public int LastEditBy
        {
            get { return (_LastEditBy); }
            set { _LastEditBy = value; }
        }

        public string CreatedDate
        {
            get { return (Convert.ToDateTime(_CreatedDate).ToString("o")); }
            set { _CreatedDate = value; }
        }
        public string Username
        {
            get { return (_Username); }
            set { _Username = value; }
        }

        public int ClientID
        {
            get { return (_ClientID); }
            set { _ClientID = value; }
        }

        public string RoleName { get => _RoleName; set => _RoleName = value; }
        public int RoleID { get => _RoleID; set => _RoleID = value; }
        public string Token { get => _token; set => _token = value; }

        #endregion Get/Sets

        #region Constructors
        public APIUser()
        {
        }
        public APIUser(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
            TableName = "Users";
        }

        public APIUser(int ID, ILogger logger, IConfiguration config) : this(logger, config)
        {
            string CnxnString = Config["CnxnString"];
            string LogPath = Config["LogPath"];

            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                SqlCommand cmd = new SqlCommand("SELECT * FROM [" + this.SchemaName + "].[" + this.TableName + "] WHERE ID = @ID", Cnxn);

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    this.Username = dr["Username"] == DBNull.Value ? "" : dr["Username"].ToString().Trim();
                    this.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    this.FirstName = dr["FirstName"] == DBNull.Value ? "" : dr["FirstName"].ToString().Trim();
                    this.LastEditBy = dr["LastEditBy"] == DBNull.Value ? 0 : Convert.ToInt32(dr["LastEditBy"]);
                    this.ClientID = dr["ClientID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ClientID"]);
                    this.RoleID = dr["RoleID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["RoleID"]);
                    this.LastLoginDate = dr["LastLoginDate"] == DBNull.Value ? "" : dr["LastLoginDate"].ToString().Trim();
                    this.LastEditDate = dr["LastEditDate"] == DBNull.Value ? "" : Convert.ToDateTime(dr["LastEditDate"]).ToUniversalTime().ToString("o").Trim();
                    this.RoleName = dr["Name"] == DBNull.Value ? "" : dr["Name"].ToString().Trim();
                    this.CreatedDate = dr["CreatedDate"] == DBNull.Value ? "" : Convert.ToDateTime(dr["CreatedDate"]).ToUniversalTime().ToString("o").Trim();
                    this.LastName = dr["LastName"] == DBNull.Value ? "" : dr["LastName"].ToString().Trim();
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("APIUserConstructor", Exc.Message, LogPath);
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

                SqlCommand cmd = new SqlCommand("[" + this.SchemaName + "].[spUserSave]", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for APIUser
                cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.VarChar, 50));
                cmd.Parameters["@Username"].Value = this.Username ?? "";

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = this.ID;

                cmd.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.VarChar, 50));
                cmd.Parameters["@FirstName"].Value = this.FirstName ?? "";

                cmd.Parameters.Add("@LastEditBy", SqlDbType.Int);
                cmd.Parameters["@LastEditBy"].Value = this.LastEditBy > 0 ? this.LastEditBy : 1;

                cmd.Parameters.Add(new SqlParameter("@ClientID", SqlDbType.Int));
                cmd.Parameters["@ClientID"].Value = this.ClientID;

                cmd.Parameters.Add(new SqlParameter("@RoleID", SqlDbType.Int));
                cmd.Parameters["@RoleID"].Value = this.RoleID == 0 ? 2 : this.RoleID;

                cmd.Parameters.Add(new SqlParameter("@LastName", SqlDbType.VarChar, 50));
                cmd.Parameters["@LastName"].Value = this.LastName ?? "";

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
                oPR.Result = this.ID.ToString();
                return (oPR);

            }
            catch (Exception Exc)
            {
                Log.LogErr("APIUserSave", Exc.Message, LogPath);

                oPR.Exception = Exc;
                oPR.Result += "Error";
                return (oPR);
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
                cmd.Parameters["@ID"].Value = this.ID;

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();
                return (true);
            }
            catch (Exception Exc)
            {
                Log.LogErr("APIUserDelete", Exc.Message, LogPath);
                return (false);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }


        }
        #endregion Delete

        #region FetchRoles
        public static ProcessResult FetchRoles(int UserID, int ClientID, string CnxnString, string LogPath)
        {
            ProcessResult oPR = new ProcessResult();
            List<string> Roles = new List<string>();

            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {
                APIUser u = new APIUser(null, null);
                SqlCommand cmd = new SqlCommand("[" + u.SchemaName + "].[spUserRolesFetch]", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int));
                cmd.Parameters["@UserID"].Value = UserID;

                cmd.Parameters.Add(new SqlParameter("@ClientID", SqlDbType.Int));
                cmd.Parameters["@ClientID"].Value = ClientID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Roles.Add(dr[0].ToString());
                }

                dr.Close();
                Cnxn.Close();

                oPR.ObjectProcessed = Roles;
            }
            catch (Exception Exc)
            {
                oPR.Exception = Exc;
                Log.LogErr("FetchRolesStatic", Exc.Message, LogPath);
            }

            return (oPR);

        }
        #endregion FetchRoles

    }
}