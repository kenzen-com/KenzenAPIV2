using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using KenzenAPI.Classes;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace KenzenAPI.DataClasses
{
    public class ConnectionStatusCollection : Dictionary<int, ConnectionStatus>
    {
        ILogger Logger;
        IConfiguration Config;
        #region Constructors

        public ConnectionStatusCollection()
        {
        }

        public ConnectionStatusCollection(ILogger logger, IConfiguration config, int ClientID)
        {
            Logger = logger;
            Config = config;

            // fetch all from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spConnectionStatusFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ConnectionStatus oConnectionStatus = new ConnectionStatus(null, null);
                    oConnectionStatus.DeviceStatus = dr["DeviceStatus"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DeviceStatus"]);
                    oConnectionStatus.FirmwareVersion = dr["FirmwareVersion"] == DBNull.Value ? "" : dr["FirmwareVersion"].ToString().Trim();
                    oConnectionStatus.BatteryPercent = dr["BatteryPercent"] == DBNull.Value ? 0 : Convert.ToInt32(dr["BatteryPercent"]);
                    oConnectionStatus.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                    oConnectionStatus.GMT = dr["GMT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GMT"]);
                    oConnectionStatus.MacAddress = dr["MacAddress"] == DBNull.Value ? "" : dr["MacAddress"].ToString().Trim();
                    oConnectionStatus.IsCharging = dr["IsCharging"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsCharging"]);
                    oConnectionStatus.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                    if (!this.ContainsKey(oConnectionStatus.ID))
                        this.Add(oConnectionStatus.ID, oConnectionStatus);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error("ConnectionStatuCollectionConstructor", Exc.Message, Config["LogPath"]);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }
        }

        #endregion Constructors


        #region Save
        public ProcessResult Save(IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            try
            {
                foreach (ConnectionStatus o in this.Values)
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
                Logger.Error("ConnectionStatusCollection Save", Exc.Message, Config["LogPath"]);
                oPR.Exception = Exc;
                return (oPR);
            }
        }
        #endregion Save
    }



    public class ConnectionStatus : DataClassBase
    {

        #region Vars

        string _FirmwareVersion;
        int _UserID;
        int _DeviceStatus;
        int _BatteryPercent;
        string _MacAddress;
        bool _IsCharging;



        #endregion Vars

        #region Get/Sets

        public string FirmwareVersion
        {
            get { return (_FirmwareVersion); }
            set { _FirmwareVersion = value; }
        }

        public int UserID
        {
            get { return (_UserID); }
            set { _UserID = value; }
        }

        public int DeviceStatus
        {
            get { return (_DeviceStatus); }
            set { _DeviceStatus = value; }
        }

        public int BatteryPercent
        {
            get { return (_BatteryPercent); }
            set { _BatteryPercent = value; }
        }

        public string MacAddress
        {
            get { return (_MacAddress); }
            set { _MacAddress = value; }
        }

        public bool IsCharging
        {
            get { return (_IsCharging); }
            set { _IsCharging = value; }
        }

   
        #endregion Get/Sets

        #region Constructors
        public ConnectionStatus() { }
        public ConnectionStatus(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }

        public ConnectionStatus(int ConnectionStatuID)
        {
            // fill props from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spConnectionStatuInfoFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ConnectionStatuID", SqlDbType.Int));
                cmd.Parameters["@ConnectionStatuID"].Value = ConnectionStatuID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    this.DeviceStatus = dr["DeviceStatus"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DeviceStatus"]);
                    this.FirmwareVersion = dr["FirmwareVersion"] == DBNull.Value ? "" : dr["FirmwareVersion"].ToString().Trim();
                    this.BatteryPercent = dr["BatteryPercent"] == DBNull.Value ? 0 : Convert.ToInt32(dr["BatteryPercent"]);
                    this.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                    this.GMT = dr["GMT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GMT"]);
                    this.MacAddress = dr["MacAddress"] == DBNull.Value ? "" : dr["MacAddress"].ToString().Trim();
                    this.IsCharging = dr["IsCharging"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsCharging"]);
                    this.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error("ConnectionStatuConstructor", Exc.Message, Config["LogPath"]);
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
            ProcessResult oPR = new ProcessResult();
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spConnectionStatuSave", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for ConnectionStatus
                cmd.Parameters.Add(new SqlParameter("@DeviceStatus", SqlDbType.Int));
                cmd.Parameters["@DeviceStatus"].Value = this.DeviceStatus;

                cmd.Parameters.Add(new SqlParameter("@FirmwareVersion", SqlDbType.VarChar, 50));
                cmd.Parameters["@FirmwareVersion"].Value = this.FirmwareVersion ?? "";

                cmd.Parameters.Add(new SqlParameter("@BatteryPercent", SqlDbType.Int));
                cmd.Parameters["@BatteryPercent"].Value = this.BatteryPercent;

                cmd.Parameters.Add(new SqlParameter("@UTC", SqlDbType.VarChar, 50));
                cmd.Parameters["@UTC"].Value = this.UTC ?? DateTime.Now.ToString();

                cmd.Parameters.Add(new SqlParameter("@GMT", SqlDbType.Int));
                cmd.Parameters["@GMT"].Value = this.GMT;

                cmd.Parameters.Add(new SqlParameter("@MacAddress", SqlDbType.VarChar, 50));
                cmd.Parameters["@MacAddress"].Value = this.MacAddress ?? "";

                cmd.Parameters.Add(new SqlParameter("@IsCharging", SqlDbType.Bit));
                cmd.Parameters["@IsCharging"].Value = this.IsCharging;

                cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int));
                cmd.Parameters["@UserID"].Value = this.UserID;

                // assign output param
                cmd.Parameters.Add(new SqlParameter("@ConnectionStatuIDOut", SqlDbType.Int));
                cmd.Parameters["@ConnectionStatuIDOut"].Direction = ParameterDirection.Output;

                #endregion Parameters

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();

                int iConnectionStatuID = Convert.ToInt32(cmd.Parameters["@ConnectionStatuIDOut"].Value);
                this.ID = iConnectionStatuID;

                oPR.ObjectProcessed = this;
                oPR.Result += "Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Logger.Error("ConnectionStatuSave", Exc.Message, Config["LogPath"]);

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


        public static bool Delete(int ConnectionStatusID, int ClientID, ILogger Logger, IConfiguration Config)
        {
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spConnectionStatusDelete", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ConnectionStatusID", SqlDbType.Int));
                cmd.Parameters["@ConnectionStatusID"].Value = ConnectionStatusID;

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();
                return (true);
            }
            catch (Exception Exc)
            {
                Logger.Error("ConnectionStatuDelete", Exc.Message, Config["LogPath"]);
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