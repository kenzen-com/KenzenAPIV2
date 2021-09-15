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
    public class DailyFeedbackCollection : Dictionary<int, DailyFeedback>
    {

        #region Constructors

        public DailyFeedbackCollection()
        {
        }

        public DailyFeedbackCollection(ILogger Logger, IConfiguration Config, int ClientID)
        {
            // fetch all from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spDailyFeedbacksFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    DailyFeedback oDailyFeedback = new DailyFeedback(null, null);
                    oDailyFeedback.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                    oDailyFeedback.QuestionID = dr["QuestionID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["QuestionID"]);
                    oDailyFeedback.GMT = dr["GMT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GMT"]);
                    oDailyFeedback.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    oDailyFeedback.Response = dr["Response"] == DBNull.Value ? "" : dr["Response"].ToString().Trim();
                    oDailyFeedback.AnswerID = dr["AnswerID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["AnswerID"]);
                    oDailyFeedback.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                    if (!this.ContainsKey(oDailyFeedback.ID))
                        this.Add(oDailyFeedback.ID, oDailyFeedback);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error("DailyFeedbackCollectionConstructor", Exc.Message, Config["LogPath"]);
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
                foreach (DailyFeedback o in this.Values)
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
                Logger.Error("DailyFeedbackCollection Save", Exc.Message, Config["LogPath"]);
                oPR.Exception = Exc;
                return (oPR);
            }
        }
        #endregion Save
    }



    public class DailyFeedback : DataClassBase
    {

        #region Vars

        int _UserID;
        int _QuestionID;
        int _ID;
        int _AnswerID;
        string _UTC;
        int _GMT;
        string _Response;

        #endregion Vars

        #region Get/Sets

        public int UserID
        {
            get { return (_UserID); }
            set { _UserID = value; }
        }

        public int QuestionID
        {
            get { return (_QuestionID); }
            set { _QuestionID = value; }
        }

        public int AnswerID
        {
            get { return (_AnswerID); }
            set { _AnswerID = value; }
        }

        public int GMT
        {
            get { return (_GMT); }
            set { _GMT = value; }
        }

        public string Response
        {
            get { return (_Response); }
            set { _Response = value; }
        }

        #endregion Get/Sets

        #region Constructors

        public DailyFeedback(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
        }

        public DailyFeedback(int DailyFeedbackID)
        {
            // fill props from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spDailyFeedbackInfoFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@DailyFeedbackID", SqlDbType.Int));
                cmd.Parameters["@DailyFeedbackID"].Value = DailyFeedbackID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    this.UserID = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]);
                    this.QuestionID = dr["QuestionID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["QuestionID"]);
                    this.GMT = dr["GMT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GMT"]);
                    this.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    this.Response = dr["Response"] == DBNull.Value ? "" : dr["Response"].ToString().Trim();
                    this.AnswerID = dr["AnswerID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["AnswerID"]);
                    this.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error("DailyFeedbackConstructor", Exc.Message, Config["LogPath"]);
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

                SqlCommand cmd = new SqlCommand("spDailyFeedbackSave", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for DailyFeedback
                cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int));
                cmd.Parameters["@UserID"].Value = this.UserID;

                cmd.Parameters.Add(new SqlParameter("@QuestionID", SqlDbType.Int));
                cmd.Parameters["@QuestionID"].Value = this.QuestionID;

                cmd.Parameters.Add(new SqlParameter("@GMT", SqlDbType.Int));
                cmd.Parameters["@GMT"].Value = this.GMT;

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = this.ID;

                cmd.Parameters.Add(new SqlParameter("@Response", SqlDbType.VarChar, 255));
                cmd.Parameters["@Response"].Value = this.Response ?? "";

                cmd.Parameters.Add(new SqlParameter("@AnswerID", SqlDbType.Int));
                cmd.Parameters["@AnswerID"].Value = this.AnswerID;

                cmd.Parameters.Add(new SqlParameter("@UTC", SqlDbType.VarChar, 50));
                cmd.Parameters["@UTC"].Value = this.UTC ?? DateTime.Now.ToString();

                // assign output param
                cmd.Parameters.Add(new SqlParameter("@DailyFeedbackIDOut", SqlDbType.Int));
                cmd.Parameters["@DailyFeedbackIDOut"].Direction = ParameterDirection.Output;

                #endregion Parameters

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();

                int iDailyFeedbackID = Convert.ToInt32(cmd.Parameters["@DailyFeedbackIDOut"].Value);
                this.ID = iDailyFeedbackID;

                oPR.ObjectProcessed = this;
                oPR.Result += "Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Logger.Error("DailyFeedbackSave", Exc.Message, Config["LogPath"]);

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


        public static bool Delete(int DailyFeedbackID, int ClientID, ILogger Logger, IConfiguration Config)
        {
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spDailyFeedbackDelete", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@DailyFeedbackID", SqlDbType.Int));
                cmd.Parameters["@DailyFeedbackID"].Value = DailyFeedbackID;

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();
                return (true);
            }
            catch (Exception Exc)
            {
                Logger.Error("DailyFeedbackDelete", Exc.Message, Config["LogPath"]);
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