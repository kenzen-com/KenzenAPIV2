using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using KenzenAPI.Classes;
using Serilog;
using Microsoft.Extensions.Configuration;

namespace KenzenAPI.DataClasses
{
    public class WorkRestCollection : Dictionary<int, WorkRest>
    {

        #region Constructors

        public WorkRestCollection()
        {
        }

        public WorkRestCollection(int ClientID, ILogger Logger, IConfiguration Config)
        {
            // fetch all from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spWorkRestsFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    WorkRest oWorkRest = new WorkRest(null, null);
                    oWorkRest.userId = dr["userId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["userId"]);
                    oWorkRest.sunExposureId = dr["sunExposureId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["sunExposureId"]);
                    oWorkRest.latitude = dr["latitude"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["latitude"]);
                    oWorkRest.wrRisklevelId = dr["wrRisklevelId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["wrRisklevelId"]);
                    oWorkRest.clothingId = dr["clothingId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["clothingId"]);
                    oWorkRest.GMT = dr["GMT"] == DBNull.Value ? "" : dr["GMT"].ToString().Trim();
                    oWorkRest.workLevelId = dr["workLevelId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["workLevelId"]);
                    oWorkRest.utcTs = dr["utcTs"] == DBNull.Value ? "" : dr["utcTs"].ToString().Trim();
                    oWorkRest.locationKey = dr["locationKey"] == DBNull.Value ? 0 : Convert.ToInt32(dr["locationKey"]);
                    oWorkRest.teamId = dr["teamId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["teamId"]);
                    oWorkRest.longitude = dr["longitude"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["longitude"]);
                    oWorkRest.temperature = dr["temperature"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["temperature"]);
                    oWorkRest.location = dr["location"] == DBNull.Value ? "" : dr["location"].ToString().Trim();
                    oWorkRest.wrRecActionId = dr["wrRecActionId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["wrRecActionId"]);
                    oWorkRest.humidity = dr["humidity"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["humidity"]);
                    oWorkRest.environmentId = dr["environmentId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["environmentId"]);
                    if (!this.ContainsKey(oWorkRest.ID))
                        this.Add(oWorkRest.ID, oWorkRest);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error("WorkRestCollectionConstructor", Exc.Message, Config["LogPath"]);
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
                foreach (WorkRest o in this.Values)
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
                Logger.Error("WorkRestCollection Save", Exc.Message, Config["LogPath"]);
                oPR.Exception = Exc;
                return (oPR);
            }
        }
        #endregion Save
    }



    public class WorkRest : DataClassBase
    {

        #region Vars

        int _sunExposureId;
        string _utcTs;
        Decimal _latitude;
        int _locationKey;
        int _workLevelId;
        int _wrRecActionId;
        int _teamId;
        int _environmentId;
        int _wrRisklevelId;
        Decimal _temperature;
        string _location;
        int _userId;
        int _clothingId;
        Decimal _humidity;
        Decimal _longitude;
        string _GMT;

        #endregion Vars

        #region Get/Sets

        public int sunExposureId
        {
            get { return (_sunExposureId); }
            set { _sunExposureId = value; }
        }

        public string utcTs
        {
            get { return (_utcTs); }
            set { _utcTs = value; }
        }

        public Decimal latitude
        {
            get { return (_latitude); }
            set { _latitude = value; }
        }

        public int locationKey
        {
            get { return (_locationKey); }
            set { _locationKey = value; }
        }

        public int workLevelId
        {
            get { return (_workLevelId); }
            set { _workLevelId = value; }
        }

        public int wrRecActionId
        {
            get { return (_wrRecActionId); }
            set { _wrRecActionId = value; }
        }

        public int teamId
        {
            get { return (_teamId); }
            set { _teamId = value; }
        }

        public int environmentId
        {
            get { return (_environmentId); }
            set { _environmentId = value; }
        }

        public int wrRisklevelId
        {
            get { return (_wrRisklevelId); }
            set { _wrRisklevelId = value; }
        }

        public Decimal temperature
        {
            get { return (_temperature); }
            set { _temperature = value; }
        }

        public string location
        {
            get { return (_location); }
            set { _location = value; }
        }

        public int userId
        {
            get { return (_userId); }
            set { _userId = value; }
        }

        public int clothingId
        {
            get { return (_clothingId); }
            set { _clothingId = value; }
        }

        public Decimal humidity
        {
            get { return (_humidity); }
            set { _humidity = value; }
        }

        public Decimal longitude
        {
            get { return (_longitude); }
            set { _longitude = value; }
        }

        public string GMT
        {
            get { return (_GMT); }
            set { _GMT = value; }
        }

        #endregion Get/Sets

        #region Constructors

        public WorkRest(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }

        public WorkRest(int WorkRestID)
        {
            // fill props from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spWorkRestInfoFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@WorkRestID", SqlDbType.Int));
                cmd.Parameters["@WorkRestID"].Value = WorkRestID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    this.userId = dr["userId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["userId"]);
                    this.sunExposureId = dr["sunExposureId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["sunExposureId"]);
                    this.latitude = dr["latitude"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["latitude"]);
                    this.wrRisklevelId = dr["wrRisklevelId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["wrRisklevelId"]);
                    this.clothingId = dr["clothingId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["clothingId"]);
                    this.GMT = dr["GMT"] == DBNull.Value ? "" : dr["GMT"].ToString().Trim();
                    this.workLevelId = dr["workLevelId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["workLevelId"]);
                    this.utcTs = dr["utcTs"] == DBNull.Value ? "" : dr["utcTs"].ToString().Trim();
                    this.locationKey = dr["locationKey"] == DBNull.Value ? 0 : Convert.ToInt32(dr["locationKey"]);
                    this.teamId = dr["teamId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["teamId"]);
                    this.longitude = dr["longitude"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["longitude"]);
                    this.temperature = dr["temperature"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["temperature"]);
                    this.location = dr["location"] == DBNull.Value ? "" : dr["location"].ToString().Trim();
                    this.wrRecActionId = dr["wrRecActionId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["wrRecActionId"]);
                    this.humidity = dr["humidity"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["humidity"]);
                    this.environmentId = dr["environmentId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["environmentId"]);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error("WorkRestConstructor", Exc.Message, Config["LogPath"]);
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

                SqlCommand cmd = new SqlCommand("spWorkRestSave", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for WorkRest
                cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int));
                cmd.Parameters["@userId"].Value = this.userId;

                cmd.Parameters.Add(new SqlParameter("@sunExposureId", SqlDbType.Int));
                cmd.Parameters["@sunExposureId"].Value = this.sunExposureId;

                cmd.Parameters.Add(new SqlParameter("@latitude", SqlDbType.Money));
                cmd.Parameters["@latitude"].Value = this.latitude;

                cmd.Parameters.Add(new SqlParameter("@wrRisklevelId", SqlDbType.Int));
                cmd.Parameters["@wrRisklevelId"].Value = this.wrRisklevelId;

                cmd.Parameters.Add(new SqlParameter("@clothingId", SqlDbType.Int));
                cmd.Parameters["@clothingId"].Value = this.clothingId;

                cmd.Parameters.Add(new SqlParameter("@GMT", SqlDbType.VarChar, 255));
                cmd.Parameters["@GMT"].Value = this.GMT ?? "";

                cmd.Parameters.Add(new SqlParameter("@workLevelId", SqlDbType.Int));
                cmd.Parameters["@workLevelId"].Value = this.workLevelId;

                cmd.Parameters.Add(new SqlParameter("@utcTs", SqlDbType.VarChar, 8));
                cmd.Parameters["@utcTs"].Value = this.utcTs ?? "";

                cmd.Parameters.Add(new SqlParameter("@locationKey", SqlDbType.Int));
                cmd.Parameters["@locationKey"].Value = this.locationKey;

                cmd.Parameters.Add(new SqlParameter("@teamId", SqlDbType.Int));
                cmd.Parameters["@teamId"].Value = this.teamId;

                cmd.Parameters.Add(new SqlParameter("@longitude", SqlDbType.Money));
                cmd.Parameters["@longitude"].Value = this.longitude;

                cmd.Parameters.Add(new SqlParameter("@temperature", SqlDbType.Money));
                cmd.Parameters["@temperature"].Value = this.temperature;

                cmd.Parameters.Add(new SqlParameter("@location", SqlDbType.VarChar, 255));
                cmd.Parameters["@location"].Value = this.location ?? "";

                cmd.Parameters.Add(new SqlParameter("@wrRecActionId", SqlDbType.Int));
                cmd.Parameters["@wrRecActionId"].Value = this.wrRecActionId;

                cmd.Parameters.Add(new SqlParameter("@humidity", SqlDbType.Money));
                cmd.Parameters["@humidity"].Value = this.humidity;

                cmd.Parameters.Add(new SqlParameter("@environmentId", SqlDbType.Int));
                cmd.Parameters["@environmentId"].Value = this.environmentId;

                // assign output param
                cmd.Parameters.Add(new SqlParameter("@WorkRestIDOut", SqlDbType.Int));
                cmd.Parameters["@WorkRestIDOut"].Direction = ParameterDirection.Output;

                #endregion Parameters

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();

                int iWorkRestID = Convert.ToInt32(cmd.Parameters["@WorkRestIDOut"].Value);
                this.ID = iWorkRestID;

                oPR.ObjectProcessed = this;
                oPR.Result += "Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Logger.Error("WorkRestSave", Exc.Message, Config["LogPath"]);

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


        public static bool Delete(int WorkRestID, int ClientID, ILogger Logger, IConfiguration Config)
        {
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spWorkRestDelete", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@WorkRestID", SqlDbType.Int));
                cmd.Parameters["@WorkRestID"].Value = WorkRestID;

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();
                return (true);
            }
            catch (Exception Exc)
            {
                Logger.Error("WorkRestDelete", Exc.Message, Config["LogPath"]);
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