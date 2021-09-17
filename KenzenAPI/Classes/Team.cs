using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Serilog;
using Microsoft.Extensions.Configuration;
using KenzenAPI.Classes;
using KenzenAPI;
using System.Data.SqlClient;

namespace KenzenAPI.DataClasses
{
    
    public class TeamCollection : Dictionary<int, Team>
    {
        #region Constructors

        public TeamCollection()
        {
        }

        public TeamCollection(ILogger Logger, IConfiguration Config)
        {
            // fetch all from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(1, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spTeamsFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Team oTeam = new Team();
                    oTeam.FillProps(dr);
                    if (!this.ContainsKey(oTeam.ID))
                        this.Add(oTeam.ID, oTeam);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error(Exc.Message);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }
        }

        public TeamCollection(ILogger Logger, IConfiguration Config, int ClientID)
        {

            // fetch all from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spTeamsByClient", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ClientID", SqlDbType.Int));
                cmd.Parameters["@ClientID"].Value = ClientID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Team oTeam = new Team();
                    oTeam.FillProps(dr);
                    if (!this.ContainsKey(oTeam.ID))
                        this.Add(oTeam.ID, oTeam);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error(Exc.Message);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }
        }
        #endregion Constructors

        #region Save
        public ProcessResult Save(ILogger Logger, IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            try
            {
                foreach (Team o in this.Values)
                {
                    oPR = o.Save(Logger, Config);
                    if (oPR.Exception != null)
                        throw oPR.Exception;
                }
                oPR.Result += "Collection Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Logger.Error("TeamCollection Save", Exc.Message, Config["LogPath"]);
                
                oPR.Exception = Exc;
                return (oPR);
            }
        }
        #endregion Save
    }


    
    public class Team : DataClassBase
    {

        #region Vars

        decimal _Longitude;
        int _ID;
        string _Name;

        decimal _Latitude;
        int _TeamID;
        string _Location;

        #endregion Vars

        #region Get/Sets

        public decimal Longitude
        {
            get { return (_Longitude); }
            set { _Longitude = value; }
        }

        public int ID
        {
            get { return (_ID); }
            set { _ID = value; }
        }

        public string Name
        {
            get { return (_Name); }
            set { _Name = value; }
        }

        public string UTC
        {
            get { return (_UTC); }
            set { _UTC = value; }
        }

        public decimal Latitude
        {
            get { return (_Latitude); }
            set { _Latitude = value; }
        }

        public int TeamID
        {
            get { return (_TeamID); }
            set { _TeamID = value; }
        }

        public string Location
        {
            get { return (_Location); }
            set { _Location = value; }
        }

        #endregion Get/Sets

        #region Constructors

        public Team()
        {
        }
        public Team(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }
        public Team(int TeamID)
        {
            // fill props from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spTeamInfoFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@TeamID", SqlDbType.Int));
                cmd.Parameters["@TeamID"].Value = TeamID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    FillProps(dr);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error(Exc.Message);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }


        }

        #endregion Constructors

        public void FillProps(SqlDataReader dr)
        {
            this.Longitude = dr["Longitude"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Longitude"]);
            this.Name = dr["Name"] == DBNull.Value ? "" : dr["Name"].ToString().Trim();
            this.TeamID = dr["TeamID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TeamID"]);
            this.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
            this.Location = dr["Location"] == DBNull.Value ? "" : dr["Location"].ToString().Trim();
            this.Latitude = dr["Latitude"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Latitude"]);
            this.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();

        }

        #region Save
        public ProcessResult Save(ILogger logger, IConfiguration config)
        {
            ProcessResult oPR = new ProcessResult();
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spTeamSave", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for Team
                cmd.Parameters.Add(new SqlParameter("@Longitude", SqlDbType.Money, 8));
                cmd.Parameters["@Longitude"].Value = this.Longitude;

                cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar, 255));
                cmd.Parameters["@Name"].Value = this.Name ?? "";

                cmd.Parameters.Add(new SqlParameter("@TeamID", SqlDbType.Int));
                cmd.Parameters["@TeamID"].Value = this.TeamID;

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = this.ID;

                cmd.Parameters.Add(new SqlParameter("@Location", SqlDbType.VarChar, 255));
                cmd.Parameters["@Location"].Value = this.Location ?? "";

                cmd.Parameters.Add(new SqlParameter("@Latitude", SqlDbType.Money, 8));
                cmd.Parameters["@Latitude"].Value = this.Latitude;

                cmd.Parameters.Add(new SqlParameter("@UTC", SqlDbType.VarChar, 50));
                cmd.Parameters["@UTC"].Value = this.UTC ?? DateTime.Now.ToString();

                // assign output param
                cmd.Parameters.Add(new SqlParameter("@TeamIDOut", SqlDbType.Int));
                cmd.Parameters["@TeamIDOut"].Direction = ParameterDirection.Output;

                #endregion Parameters

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();

                int iTeamID = Convert.ToInt32(cmd.Parameters["@TeamIDOut"].Value);
                this.ID = iTeamID;

                oPR.ObjectProcessed = this;
                oPR.Result += "Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Logger.Error(Exc.Message);

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


        public static bool Delete(int ClientID, int TeamID, ILogger Logger, IConfiguration Config)
        {
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spTeamDelete", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@TeamID", SqlDbType.Int));
                cmd.Parameters["@TeamID"].Value = TeamID;

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();
                return (true);
            }
            catch (Exception Exc)
            {
                Logger.Error(Exc.Message);
                return (false);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }


        }
        #endregion Delete

        #region Users
        public static ProcessResult Users(int ClientID, int TeamID, ILogger Logger, IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {
                List<User> oOut = new List<User>();

                User oUser = new User(Logger, Config);
                SqlCommand cmd = new SqlCommand("spUsersFetchByTeam", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TeamID", SqlDbType.Int).Value = TeamID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    oOut.Add(oUser);

                }
                dr.Close();
                Cnxn.Close();
                oPR.ObjectProcessed = oOut;
            }
            catch (Exception Exc)
            {
                oPR.Exception = Exc;
                Logger.Error(Exc.Message);
            }

            return oPR;
        }
        #endregion Users

        #region AssignToTeam
        public static ProcessResult AssignToTeam(int ClientID, int TeamID, int UserID, ILogger Logger, IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spTeamUserAssign", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for Team
                cmd.Parameters.Add(new SqlParameter("@TeamID", SqlDbType.Int));
                cmd.Parameters["@TeamID"].Value = TeamID;

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = UserID;

                #endregion Parameters

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();

                oPR.Result += "Assigned";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Logger.Error(Exc.Message);

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

        #region Managers
        public static ProcessResult Managers(int ClientID, ILogger Logger, IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {
                List<User> oOut = new List<User>();

                User oUser = new User(Logger, Config);
                SqlCommand cmd = new SqlCommand("spManagersFetchByClient", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ClientID", SqlDbType.Int).Value = ClientID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    oOut.Add(oUser);

                }
                dr.Close();
                Cnxn.Close();
                oPR.ObjectProcessed = oOut;
            }
            catch (Exception Exc)
            {
                oPR.Exception = Exc;
                Logger.Error(Exc.Message);
            }

            return oPR;
        }
        #endregion Users

        #region AssignManager
        public static ProcessResult AssignManager(int ClientID, int TeamID, int UserID, ILogger Logger, IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spTeamManagerAssign", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for Team
                cmd.Parameters.Add(new SqlParameter("@TeamID", SqlDbType.Int));
                cmd.Parameters["@TeamID"].Value = TeamID;

                cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int));
                cmd.Parameters["@UserID"].Value = UserID;

                #endregion Parameters

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();

                oPR.Result += "Assigned";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Logger.Error(Exc.Message);

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
    }
}