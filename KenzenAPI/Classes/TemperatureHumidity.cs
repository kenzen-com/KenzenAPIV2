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
    public class TemperatureHumidityCollection : Dictionary<int, TemperatureHumidity>
    {

        #region Constructors

        public TemperatureHumidityCollection()
        {
        }

        public TemperatureHumidityCollection(int ClientID, ILogger Logger, IConfiguration Config)
        {
            // fetch all from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spTemperatureHumiditiesFetchByClient", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TemperatureHumidity oTemperatureHumidity = new TemperatureHumidity(null, null);
                    oTemperatureHumidity.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    oTemperatureHumidity.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                    oTemperatureHumidity.SkinRH109_1min = dr["SkinRH109_1min"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SkinRH109_1min"]);
                    oTemperatureHumidity.GMT = dr["GMT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GMT"]);
                    oTemperatureHumidity.AmbientRH110_1min = dr["AmbientRH110_1min"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["AmbientRH110_1min"]);
                    oTemperatureHumidity.AmbientTemp110_1min = dr["AmbientTemp110_1min"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["AmbientTemp110_1min"]);
                    oTemperatureHumidity.SkinTemp109_1min = dr["SkinTemp109_1min"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SkinTemp109_1min"]);
                    oTemperatureHumidity.TeamID = dr["TeamID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TeamID"]);
                    oTemperatureHumidity.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                    oTemperatureHumidity.MaxTotalAcc_1min = dr["MaxTotalAcc_1min"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MaxTotalAcc_1min"]);
                    if (!this.ContainsKey(oTemperatureHumidity.ID))
                        this.Add(oTemperatureHumidity.ID, oTemperatureHumidity);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error("TemperatureHumidityCollectionConstructor", Exc.Message, Config["LogPath"]);
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
                foreach (TemperatureHumidity o in this.Values)
                {
                    oPR = o.Save(Config);
                    if (oPR.Exception != null)
                        throw oPR.Exception;
                }
                oPR.Result += "Collection Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Logger.Error("TemperatureHumidityCollection Save", Exc.Message, Config["LogPath"]);
                oPR.Exception = Exc;
                return (oPR);
            }
        }
        #endregion Save
    }



    public class TemperatureHumidity : DataClassBase
    {

        #region Vars

        decimal _AmbientTemp110_1min;
        decimal _SkinRH109_1min;
        decimal _MaxTotalAcc_1min;

        int _UserID;
        int _TeamID;
        int _ID;
        decimal _AmbientRH110_1min;

        decimal _SkinTemp109_1min;

        #endregion Vars

        #region Get/Sets

        public decimal AmbientTemp110_1min
        {
            get { return (_AmbientTemp110_1min); }
            set { _AmbientTemp110_1min = value; }
        }

        public decimal SkinRH109_1min
        {
            get { return (_SkinRH109_1min); }
            set { _SkinRH109_1min = value; }
        }

        public decimal MaxTotalAcc_1min
        {
            get { return (_MaxTotalAcc_1min); }
            set { _MaxTotalAcc_1min = value; }
        }

        public int UserID
        {
            get { return (_UserID); }
            set { _UserID = value; }
        }

        public int TeamID
        {
            get { return (_TeamID); }
            set { _TeamID = value; }
        }

        public decimal AmbientRH110_1min
        {
            get { return (_AmbientRH110_1min); }
            set { _AmbientRH110_1min = value; }
        }

        public decimal SkinTemp109_1min
        {
            get { return (_SkinTemp109_1min); }
            set { _SkinTemp109_1min = value; }
        }

        #endregion Get/Sets

        #region Constructors
        public TemperatureHumidity() { }

        public TemperatureHumidity(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }

        public TemperatureHumidity(int ID)
        {
            // fill props from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spTemperatureHumidityInfoFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    this.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    this.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                    this.SkinRH109_1min = dr["SkinRH109_1min"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SkinRH109_1min"]);
                    this.GMT = dr["GMT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GMT"]);
                    this.AmbientRH110_1min = dr["AmbientRH110_1min"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["AmbientRH110_1min"]);
                    this.AmbientTemp110_1min = dr["AmbientTemp110_1min"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["AmbientTemp110_1min"]);
                    this.SkinTemp109_1min = dr["SkinTemp109_1min"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SkinTemp109_1min"]);
                    this.TeamID = dr["TeamID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TeamID"]);
                    this.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                    this.MaxTotalAcc_1min = dr["MaxTotalAcc_1min"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MaxTotalAcc_1min"]);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error("TemperatureHumidityConstructor", Exc.Message, Config["LogPath"]);
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

                SqlCommand cmd = new SqlCommand("spTemperatureHumiditySave", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for TemperatureHumidity
                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = this.ID;

                cmd.Parameters.Add(new SqlParameter("@UTC", SqlDbType.VarChar, 50));
                cmd.Parameters["@UTC"].Value = this.UTC ?? DateTime.Now.ToString();

                cmd.Parameters.Add(new SqlParameter("@SkinRH109_1min", SqlDbType.Money));
                cmd.Parameters["@SkinRH109_1min"].Value = this.SkinRH109_1min;

                cmd.Parameters.Add(new SqlParameter("@GMT", SqlDbType.Int));
                cmd.Parameters["@GMT"].Value = this.GMT;

                cmd.Parameters.Add(new SqlParameter("@AmbientRH110_1min", SqlDbType.Money));
                cmd.Parameters["@AmbientRH110_1min"].Value = this.AmbientRH110_1min;

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = this.ID;

                cmd.Parameters.Add(new SqlParameter("@AmbientTemp110_1min", SqlDbType.Money));
                cmd.Parameters["@AmbientTemp110_1min"].Value = this.AmbientTemp110_1min;

                cmd.Parameters.Add(new SqlParameter("@SkinTemp109_1min", SqlDbType.Money));
                cmd.Parameters["@SkinTemp109_1min"].Value = this.SkinTemp109_1min;

                cmd.Parameters.Add(new SqlParameter("@TeamID", SqlDbType.Int));
                cmd.Parameters["@TeamID"].Value = this.TeamID;

                cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int));
                cmd.Parameters["@UserID"].Value = this.UserID;

                cmd.Parameters.Add(new SqlParameter("@MaxTotalAcc_1min", SqlDbType.Money));
                cmd.Parameters["@MaxTotalAcc_1min"].Value = this.MaxTotalAcc_1min;

                // assign output param
                cmd.Parameters.Add(new SqlParameter("@TemperatureHumidityIDOut", SqlDbType.Int));
                cmd.Parameters["@TemperatureHumidityIDOut"].Direction = ParameterDirection.Output;

                #endregion Parameters

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();

                int iTemperatureHumidityID = Convert.ToInt32(cmd.Parameters["@TemperatureHumidityIDOut"].Value);
                this.ID = iTemperatureHumidityID;

                oPR.ObjectProcessed = this;
                oPR.Result += "Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Logger.Error("TemperatureHumiditySave", Exc.Message, Config["LogPath"]);

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


        public static bool Delete(int TemperatureHumidityID, int ClientID, ILogger Logger, IConfiguration Config)
        {
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spTemperatureHumidityDelete", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@TemperatureHumidityID", SqlDbType.Int));
                cmd.Parameters["@TemperatureHumidityID"].Value = TemperatureHumidityID;

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();
                return (true);
            }
            catch (Exception Exc)
            {
                Logger.Error("TemperatureHumidityDelete", Exc.Message, Config["LogPath"]);
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