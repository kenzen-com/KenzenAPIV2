using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Extensions.Configuration;
using Serilog;
using KenzenAPI.Classes;

namespace KenzenAPI.DataClasses
{

    public class HeartRateCollection : Dictionary<int, HeartRate>
    {

        #region Constructors
        ILogger Logger;
        IConfiguration Config;
        public HeartRateCollection(ILogger logger, IConfiguration config, int ClientID = 0)
        {
            Logger = logger;
            Config = config;
  
            // fetch all from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spHeartRatesFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    HeartRate oHeartRate = new HeartRate(null, null);
                    oHeartRate.CBTPostRateLim_1min = dr["CBTPostRateLim_1min"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["CBTPostRateLim_1min"]);
                    oHeartRate.StepRate_1min = dr["StepRate_1min"] == DBNull.Value ? 0 : Convert.ToInt32(dr["StepRate_1min"]);
                    oHeartRate.StepCount_1min = dr["StepCount_1min"] == DBNull.Value ? 0 : Convert.ToInt32(dr["StepCount_1min"]);
                    oHeartRate.HeartRateAvg5_1min = dr["HeartRateAvg5_1min"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["HeartRateAvg5_1min"]);
                    oHeartRate.GMT = dr["GMT"] == DBNull.Value ? "" : dr["GMT"].ToString().Trim();
                    oHeartRate.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                    oHeartRate.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                    oHeartRate.TeamID = dr["TeamID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TeamID"]);
                    if (!this.ContainsKey(oHeartRate.ID))
                        this.Add(oHeartRate.ID, oHeartRate);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("HeartRateCollectionConstructor", Exc.Message, Config["LogPath"]);
                Logger.Error(Exc.Message);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }
        }

        #endregion Constructors


        #region Save
        public ProcessResult Save(string CnxnString, string LogPath)
        {
            ProcessResult oPR = new ProcessResult();
            try
            {
                foreach (HeartRate o in this.Values)
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
                Log.LogErr("HeartRateCollection Save", Exc.Message, LogPath);
                Logger.Error(Exc.Message);
                oPR.Exception = Exc;
                return (oPR);
            }
        }
        #endregion Save
    }



    public class HeartRate
    {

        #region Vars

        int _TeamID;
        int _StepRate_1min;
        int _UserID;
        int _ID;
        string _UTC;
        decimal _CBTPostRateLim_1min;
        decimal _HeartRateAvg5_1min;
        string _GMT;
        int _StepCount_1min;

        #endregion Vars

        #region Get/Sets

        public int TeamID
        {
            get { return (_TeamID); }
            set { _TeamID = value; }
        }
        public int StepRate_1min
        {
            get { return (_StepRate_1min); }
            set { _StepRate_1min = value; }
        }

        public int UserID
        {
            get { return (_UserID); }
            set { _UserID = value; }
        }

        public string UTC
        {
            get { return (_UTC); }
            set { _UTC = value; }
        }

        public decimal CBTPostRateLim_1min
        {
            get { return (_CBTPostRateLim_1min); }
            set { _CBTPostRateLim_1min = value; }
        }

        public decimal HeartRateAvg5_1min
        {
            get { return (_HeartRateAvg5_1min); }
            set { _HeartRateAvg5_1min = value; }
        }

        public string GMT
        {
            get { return (_GMT); }
            set { _GMT = value; }
        }

        public int StepCount_1min
        {
            get { return (_StepCount_1min); }
            set { _StepCount_1min = value; }
        }

        public int ID { get => _ID; set => _ID = value; }

        #endregion Get/Sets

        #region Constructors
        ILogger Logger;
        IConfiguration Config;
        public HeartRate(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }

        #endregion Constructors

        #region Save
        public ProcessResult Save(int ClientID = 0)
        {
            ProcessResult oPR = new ProcessResult();
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spHeartRateSave", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for HeartRate
                cmd.Parameters.Add(new SqlParameter("@CBTPostRateLim_1min", SqlDbType.Money));
                cmd.Parameters["@CBTPostRateLim_1min"].Value = this.CBTPostRateLim_1min;

                cmd.Parameters.Add(new SqlParameter("@StepRate_1min", SqlDbType.Int));
                cmd.Parameters["@StepRate_1min"].Value = this.StepRate_1min;

                cmd.Parameters.Add(new SqlParameter("@StepCount_1min", SqlDbType.Int));
                cmd.Parameters["@StepCount_1min"].Value = this.StepCount_1min;

                cmd.Parameters.Add(new SqlParameter("@HeartRateAvg5_1min", SqlDbType.Money));
                cmd.Parameters["@HeartRateAvg5_1min"].Value = this.HeartRateAvg5_1min;

                cmd.Parameters.Add(new SqlParameter("@GMT", SqlDbType.VarChar, 255));
                cmd.Parameters["@GMT"].Value = this.GMT ?? "";

                cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int));
                cmd.Parameters["@UserID"].Value = this.UserID;

                cmd.Parameters.Add(new SqlParameter("@UTC", SqlDbType.VarChar, 50));
                cmd.Parameters["@UTC"].Value = this.UTC ?? "";

                cmd.Parameters.Add(new SqlParameter("@TeamID", SqlDbType.Int));
                cmd.Parameters["@TeamID"].Value = this.TeamID;

                // assign output param
                cmd.Parameters.Add(new SqlParameter("@IDOut", SqlDbType.Int));
                cmd.Parameters["@IDOut"].Direction = ParameterDirection.Output;

                #endregion Parameters

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();

                int iHeartRateID = Convert.ToInt32(cmd.Parameters["@IDOut"].Value);
                this.ID = iHeartRateID;

                oPR.ObjectProcessed = this;
                oPR.Result += "Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Log.LogErr("HeartRateSave", Exc.Message, Config["LogPath"]);
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


        public static bool Delete(int HeartRateID, string CnxnString, string LogPath)
        {
            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                SqlCommand cmd = new SqlCommand("spHeartRateDelete", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@HeartRateID", SqlDbType.Int));
                cmd.Parameters["@HeartRateID"].Value = HeartRateID;

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();
                return (true);
            }
            catch (Exception Exc)
            {
                Log.LogErr("HeartRateDelete", Exc.Message, LogPath);
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