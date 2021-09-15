using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Serilog;
using Microsoft.Extensions.Configuration;
using KenzenAPI.Classes;

namespace KenzenAPI.DataClasses
{

    public class AlertCollection : Dictionary<int, Alert>
    {
        ILogger Logger;
        IConfiguration Config;

        #region Constructors

        public AlertCollection()
        {
        }

        public AlertCollection(int ClientID, ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;

            // fetch all from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spAlertsFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Alert oAlert = new Alert(null, null);
                    oAlert.AlertStageID = dr["AlertStageID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["AlertStageID"]);
                    oAlert.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                    oAlert.TeamID = dr["TeamID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TeamID"]);
                    oAlert.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    oAlert.GMT = dr["GMT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GMT"]);
                    oAlert.AlertCounter = dr["AlertCounter"] == DBNull.Value ? 0 : Convert.ToInt32(dr["AlertCounter"]);
                    oAlert.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                    if (!this.ContainsKey(oAlert.ID))
                        this.Add(oAlert.ID, oAlert);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error("AlertCollectionConstructor", Exc.Message, Config["LogPath"]);
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
                foreach (Alert o in this.Values)
                {
                    oPR = o.Save();
                    if (oPR.Exception != null)
                        throw oPR.Exception;
                }
                oPR.Result += "Collection Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Logger.Error("AlertCollection Save", Exc.Message, Config["LogPath"]);
                oPR.Exception = Exc;
                return (oPR);
            }
        }
        #endregion Save
    }


    [Serializable]
    public class Alert : DataClassBase
    {

        #region Vars

        int _UserID;
        int _AlertCounter;
        int _ID;
        string _UTC;
        int _GMT;
        int _TeamID;
        int _AlertStageID;

        #endregion Vars

        #region Get/Sets

        public int UserID
        {
            get { return (_UserID); }
            set { _UserID = value; }
        }

        public int AlertCounter
        {
            get { return (_AlertCounter); }
            set { _AlertCounter = value; }
        }

        public int GMT
        {
            get { return (_GMT); }
            set { _GMT = value; }
        }

        public int TeamID
        {
            get { return (_TeamID); }
            set { _TeamID = value; }
        }

        public int AlertStageID
        {
            get { return (_AlertStageID); }
            set { _AlertStageID = value; }
        }

        #endregion Get/Sets

        #region Constructors
        public Alert(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }

        #endregion Constructors

        public Alert(int AlertID, IConfiguration Config)
        {
            // fill props from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spAlertInfoFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@AlertID", SqlDbType.Int));
                cmd.Parameters["@AlertID"].Value = AlertID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    this.AlertStageID = dr["AlertStageID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["AlertStageID"]);
                    this.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                    this.TeamID = dr["TeamID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TeamID"]);
                    this.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    this.GMT = dr["GMT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GMT"]);
                    this.AlertCounter = dr["AlertCounter"] == DBNull.Value ? 0 : Convert.ToInt32(dr["AlertCounter"]);
                    this.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error("AlertConstructor", Exc.Message, Config["LogPath"]);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }


        }


        #region Save
        public ProcessResult Save()
        {
            ProcessResult oPR = new ProcessResult();
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spAlertSave", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for Alerts
                cmd.Parameters.Add(new SqlParameter("@AlertStageID", SqlDbType.Int));
                cmd.Parameters["@AlertStageID"].Value = this.AlertStageID;

                cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int));
                cmd.Parameters["@UserID"].Value = this.UserID;

                cmd.Parameters.Add(new SqlParameter("@TeamID", SqlDbType.Int));
                cmd.Parameters["@TeamID"].Value = this.TeamID;

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = this.ID;

                cmd.Parameters.Add(new SqlParameter("@GMT", SqlDbType.Int));
                cmd.Parameters["@GMT"].Value = this.GMT;

                cmd.Parameters.Add(new SqlParameter("@AlertCounter", SqlDbType.Int));
                cmd.Parameters["@AlertCounter"].Value = this.AlertCounter;

                cmd.Parameters.Add(new SqlParameter("@UTC", SqlDbType.VarChar, 50));
                cmd.Parameters["@UTC"].Value = this.UTC ?? DateTime.Now.ToString();

                // assign output param
                cmd.Parameters.Add(new SqlParameter("@AlertIDOut", SqlDbType.Int));
                cmd.Parameters["@AlertIDOut"].Direction = ParameterDirection.Output;

                #endregion Parameters

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();

                int iAlertID = Convert.ToInt32(cmd.Parameters["@AlertIDOut"].Value);
                this.ID = iAlertID;

                oPR.ObjectProcessed = this;
                oPR.Result += "Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Logger.Error("AlertSave", Exc.Message, Config["LogPath"]);

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


        public static bool Delete(int AlertID, int ClientID, ILogger Logger, IConfiguration Config)
        {
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spAlertDelete", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@AlertID", SqlDbType.Int));
                cmd.Parameters["@AlertID"].Value = AlertID;

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();
                return (true);
            }
            catch (Exception Exc)
            {
                Logger.Error("AlertDelete", Exc.Message, Config["LogPath"]);
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