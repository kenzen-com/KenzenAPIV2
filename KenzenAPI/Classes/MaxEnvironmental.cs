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

    public class MaxEnvironmentalCollection : Dictionary<int, MaxEnvironmental>
    {

        #region Constructors

        public MaxEnvironmentalCollection()
        {
        }

        public MaxEnvironmentalCollection(int ClientID, IConfiguration Config)
        {
            // fetch all from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spMaxEnvironmentalsFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    MaxEnvironmental oMaxEnvironmental = new MaxEnvironmental(null, null);
                    oMaxEnvironmental.LocationID = dr["LocationID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["LocationID"]);
                    oMaxEnvironmental.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                    oMaxEnvironmental.PredictedMaxTemp = dr["PredictedMaxTemp"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PredictedMaxTemp"]);
                    oMaxEnvironmental.PredictedMaxRH = dr["PredictedMaxRH"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PredictedMaxRH"]);
                    oMaxEnvironmental.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    oMaxEnvironmental.GMT = dr["GMT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GMT"]);
                    oMaxEnvironmental.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                    if (!this.ContainsKey(oMaxEnvironmental.ID))
                        this.Add(oMaxEnvironmental.ID, oMaxEnvironmental);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("MaxEnvironmentalCollectionConstructor", Exc.Message, Config["LogPath"]);
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
                foreach (MaxEnvironmental o in this.Values)
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
                Log.LogErr("MaxEnvironmentalCollection Save", Exc.Message, Config["LogPath"]);
                oPR.Exception = Exc;
                return (oPR);
            }
        }
        #endregion Save
    }



    public class MaxEnvironmental : DataClassBase
    {

        #region Vars

        int _UserID;
        int _ID;
        int _LocationID;
        decimal _PredictedMaxRH;
        string _UTC;
        decimal _PredictedMaxTemp;
        int _GMT;

        #endregion Vars

        #region Get/Sets

        public int UserID
        {
            get { return (_UserID); }
            set { _UserID = value; }
        }

        public int ID
        {
            get { return (_ID); }
            set { _ID = value; }
        }

        public int LocationID
        {
            get { return (_LocationID); }
            set { _LocationID = value; }
        }

        public decimal PredictedMaxRH
        {
            get { return (_PredictedMaxRH); }
            set { _PredictedMaxRH = value; }
        }

        public string UTC
        {
            get { return (_UTC); }
            set { _UTC = value; }
        }

        public decimal PredictedMaxTemp
        {
            get { return (_PredictedMaxTemp); }
            set { _PredictedMaxTemp = value; }
        }

        public int GMT
        {
            get { return (_GMT); }
            set { _GMT = value; }
        }

        #endregion Get/Sets

        #region Constructors
        public MaxEnvironmental() { }

        public MaxEnvironmental(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }

        #endregion Constructors

        public MaxEnvironmental(int MaxEnvironmentalID)
        {
            // fill props from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spMaxEnvironmentalInfoFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@MaxEnvironmentalID", SqlDbType.Int));
                cmd.Parameters["@MaxEnvironmentalID"].Value = MaxEnvironmentalID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    this.LocationID = dr["LocationID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["LocationID"]);
                    this.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                    this.PredictedMaxTemp = dr["PredictedMaxTemp"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PredictedMaxTemp"]);
                    this.PredictedMaxRH = dr["PredictedMaxRH"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PredictedMaxRH"]);
                    this.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    this.GMT = dr["GMT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GMT"]);
                    this.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("MaxEnvironmentalConstructor", Exc.Message, Config["LogPath"]);
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

                SqlCommand cmd = new SqlCommand("spMaxEnvironmentalSave", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for MaxEnvironmental
                cmd.Parameters.Add(new SqlParameter("@LocationID", SqlDbType.Int));
                cmd.Parameters["@LocationID"].Value = this.LocationID;

                cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int));
                cmd.Parameters["@UserID"].Value = this.UserID;

                cmd.Parameters.Add(new SqlParameter("@PredictedMaxTemp", SqlDbType.Money, 8));
                cmd.Parameters["@PredictedMaxTemp"].Value = this.PredictedMaxTemp;

                cmd.Parameters.Add(new SqlParameter("@PredictedMaxRH", SqlDbType.Money, 8));
                cmd.Parameters["@PredictedMaxRH"].Value = this.PredictedMaxRH;

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = this.ID;

                cmd.Parameters.Add(new SqlParameter("@GMT", SqlDbType.Int));
                cmd.Parameters["@GMT"].Value = this.GMT;

                cmd.Parameters.Add(new SqlParameter("@UTC", SqlDbType.VarChar, 50));
                cmd.Parameters["@UTC"].Value = this.UTC ?? "";

                // assign output param
                cmd.Parameters.Add(new SqlParameter("@MaxEnvironmentalIDOut", SqlDbType.Int));
                cmd.Parameters["@MaxEnvironmentalIDOut"].Direction = ParameterDirection.Output;

                #endregion Parameters

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();

                int iMaxEnvironmentalID = Convert.ToInt32(cmd.Parameters["@MaxEnvironmentalIDOut"].Value);
                this.ID = iMaxEnvironmentalID;

                oPR.ObjectProcessed = this;
                oPR.Result += "Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Log.LogErr("MaxEnvironmentalSave", Exc.Message, Config["LogPath"]);

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


        public static bool Delete(int MaxEnvironmentalID, int ClientID, IConfiguration Config)
        {
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spMaxEnvironmentalDelete", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@MaxEnvironmentalID", SqlDbType.Int));
                cmd.Parameters["@MaxEnvironmentalID"].Value = MaxEnvironmentalID;

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();
                return (true);
            }
            catch (Exception Exc)
            {
                Log.LogErr("MaxEnvironmentalDelete", Exc.Message, Config["LogPath"]);
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