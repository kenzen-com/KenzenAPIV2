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

                SqlCommand cmd = new SqlCommand("spWorkRestsByClient", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;


                cmd.Parameters.Add(new SqlParameter("@ClientID", SqlDbType.Int));
                cmd.Parameters["@ClientID"].Value = ClientID;


                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    WorkRest oWorkRest = new WorkRest();
                    oWorkRest.ClothingID = dr["ClothingID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ClothingID"]);
                    oWorkRest.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    oWorkRest.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                    oWorkRest.WorkLevelID = dr["WorkLevelID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["WorkLevelID"]);
                    oWorkRest.TeamID = dr["TeamID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TeamID"]);
                    oWorkRest.GMT = dr["GMT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GMT"]);
                    oWorkRest.Temperature = dr["Temperature"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Temperature"]);
                    oWorkRest.SunExposureID = dr["SunExposureID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["SunExposureID"]);
                    oWorkRest.Latitude = dr["Latitude"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Latitude"]);
                    oWorkRest.RecActionIdD = dr["RecActionIdD"] == DBNull.Value ? 0 : Convert.ToInt32(dr["RecActionIdD"]);
                    oWorkRest.RiskLevelID = dr["RiskLevelID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["RiskLevelID"]);
                    oWorkRest.LocationID = dr["LocationID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["LocationID"]);
                    oWorkRest.Longitude = dr["Longitude"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Longitude"]);
                    oWorkRest.Humidity = dr["Humidity"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Humidity"]);
                    oWorkRest.EnvironmentID = dr["EnvironmentID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["EnvironmentID"]);
                    oWorkRest.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                    if (!this.ContainsKey(oWorkRest.ID))
                        this.Add(oWorkRest.ID, oWorkRest);
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
                foreach (WorkRest o in this.Values)
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
                Logger.Error(Exc.Message);
                oPR.Exception = Exc;
                return (oPR);
            }
        }
        #endregion Save
    }


    public class WorkRest : DataClassBase
    {

        #region Vars

        int _ClothingID;

        int _RiskLevelID;
        int _LocationID;
        decimal _Temperature;
        decimal _Humidity;
        decimal _Longitude;
        int _TeamID;
        int _ID;
        int _RecActionIdD;
        int _EnvironmentID;
        decimal _Latitude;
        int _WorkLevelID;

        int _SunExposureID;

        #endregion Vars

        #region Get/Sets

        public int ClothingID
        {
            get { return (_ClothingID); }
            set { _ClothingID = value; }
        }

        public int RiskLevelID
        {
            get { return (_RiskLevelID); }
            set { _RiskLevelID = value; }
        }

        public int LocationID
        {
            get { return (_LocationID); }
            set { _LocationID = value; }
        }

        public decimal Temperature
        {
            get { return (_Temperature); }
            set { _Temperature = value; }
        }

        public decimal Humidity
        {
            get { return (_Humidity); }
            set { _Humidity = value; }
        }
  
        public decimal Longitude
        {
            get { return (_Longitude); }
            set { _Longitude = value; }
        }

        public int TeamID
        {
            get { return (_TeamID); }
            set { _TeamID = value; }
        }

        public int ID
        {
            get { return (_ID); }
            set { _ID = value; }
        }

        public int RecActionIdD
        {
            get { return (_RecActionIdD); }
            set { _RecActionIdD = value; }
        }

        public int EnvironmentID
        {
            get { return (_EnvironmentID); }
            set { _EnvironmentID = value; }
        }

        public decimal Latitude
        {
            get { return (_Latitude); }
            set { _Latitude = value; }
        }

        public int WorkLevelID
        {
            get { return (_WorkLevelID); }
            set { _WorkLevelID = value; }
        }

        public int SunExposureID
        {
            get { return (_SunExposureID); }
            set { _SunExposureID = value; }
        }

        #endregion Get/Sets

        #region Constructors

        public WorkRest()
        {
        }

        public WorkRest(int ID, ILogger Logger, IConfiguration Config)
        {
            // fill props from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spWorkRestInfoFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    this.ClothingID = dr["ClothingID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ClothingID"]);
                    this.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    this.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                    this.WorkLevelID = dr["WorkLevelID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["WorkLevelID"]);
                    this.TeamID = dr["TeamID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TeamID"]);
                    this.GMT = dr["GMT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GMT"]);
                    this.Temperature = dr["Temperature"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Temperature"]);
                    this.SunExposureID = dr["SunExposureID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["SunExposureID"]);
                    this.Latitude = dr["Latitude"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Latitude"]);
                    this.RecActionIdD = dr["RecActionIdD"] == DBNull.Value ? 0 : Convert.ToInt32(dr["RecActionIdD"]);
                    this.RiskLevelID = dr["RiskLevelID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["RiskLevelID"]);
                    this.LocationID = dr["LocationID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["LocationID"]);
                    this.Longitude = dr["Longitude"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Longitude"]);
                    this.Humidity = dr["Humidity"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Humidity"]);
                    this.EnvironmentID = dr["EnvironmentID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["EnvironmentID"]);
                    this.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
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
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spWorkRestSave", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for WorkRest
                cmd.Parameters.Add(new SqlParameter("@ClothingID", SqlDbType.Int));
                cmd.Parameters["@ClothingID"].Value = this.ClothingID;

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = this.ID;

                cmd.Parameters.Add(new SqlParameter("@UTC", SqlDbType.VarChar, 50));
                cmd.Parameters["@UTC"].Value = this.UTC ?? DateTime.Now.ToString();

                cmd.Parameters.Add(new SqlParameter("@WorkLevelID", SqlDbType.Int));
                cmd.Parameters["@WorkLevelID"].Value = this.WorkLevelID;

                cmd.Parameters.Add(new SqlParameter("@TeamID", SqlDbType.Int));
                cmd.Parameters["@TeamID"].Value = this.TeamID;

                cmd.Parameters.Add(new SqlParameter("@GMT", SqlDbType.Int));
                cmd.Parameters["@GMT"].Value = this.GMT;

                cmd.Parameters.Add(new SqlParameter("@Temperature", SqlDbType.Money, 8));
                cmd.Parameters["@Temperature"].Value = this.Temperature;

                cmd.Parameters.Add(new SqlParameter("@SunExposureID", SqlDbType.Int));
                cmd.Parameters["@SunExposureID"].Value = this.SunExposureID;

                cmd.Parameters.Add(new SqlParameter("@Latitude", SqlDbType.Money, 8));
                cmd.Parameters["@Latitude"].Value = this.Latitude;

                cmd.Parameters.Add(new SqlParameter("@RecActionIdD", SqlDbType.Int));
                cmd.Parameters["@RecActionIdD"].Value = this.RecActionIdD;

                cmd.Parameters.Add(new SqlParameter("@RiskLevelID", SqlDbType.Int));
                cmd.Parameters["@RiskLevelID"].Value = this.RiskLevelID;

                cmd.Parameters.Add(new SqlParameter("@LocationID", SqlDbType.Int));
                cmd.Parameters["@LocationID"].Value = this.LocationID;

                cmd.Parameters.Add(new SqlParameter("@Longitude", SqlDbType.Money, 8));
                cmd.Parameters["@Longitude"].Value = this.Longitude;

                cmd.Parameters.Add(new SqlParameter("@Humidity", SqlDbType.Money, 8));
                cmd.Parameters["@Humidity"].Value = this.Humidity;

                cmd.Parameters.Add(new SqlParameter("@EnvironmentID", SqlDbType.Int));
                cmd.Parameters["@EnvironmentID"].Value = this.EnvironmentID;

                cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int));
                cmd.Parameters["@UserID"].Value = this.UserID;

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


        public static bool Delete(int ID, int ClientID, ILogger Logger, IConfiguration Config)
        {
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spWorkRestDelete", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

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

    }
}