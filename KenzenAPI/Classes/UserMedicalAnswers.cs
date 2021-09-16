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

    public class UserMedicalAnswerCollection : Dictionary<int, UserMedicalAnswer>
    {

        #region Constructors

        public UserMedicalAnswerCollection()
        {
        }

        public UserMedicalAnswerCollection(int ClientID, int UserID, ILogger Logger, IConfiguration Config)
        {
            // fetch all from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(1, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spUserMedicalAnswersFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;


                cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int));
                cmd.Parameters["@UserID"].Value = UserID;


                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    UserMedicalAnswer oUserMedicalAnswer = new UserMedicalAnswer();
                    oUserMedicalAnswer.answerId = dr["answerId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["answerId"]);
                    oUserMedicalAnswer.userId = dr["userId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["userId"]);
                    oUserMedicalAnswer.utcTs = dr["utcTs"] == DBNull.Value ? "" : dr["utcTs"].ToString().Trim();
                    oUserMedicalAnswer.GMT = dr["GMT"] == DBNull.Value ? "" : dr["GMT"].ToString().Trim();
                    oUserMedicalAnswer.questionId = dr["questionId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["questionId"]);
                    oUserMedicalAnswer.response = dr["response"] == DBNull.Value ? "" : dr["response"].ToString().Trim();
                    if (!this.ContainsKey(oUserMedicalAnswer.ID))
                        this.Add(oUserMedicalAnswer.ID, oUserMedicalAnswer);
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
        public ProcessResult Save(string CnxnString, string LogPath)
        {
            ProcessResult oPR = new ProcessResult();
            try
            {
                foreach (UserMedicalAnswer o in this.Values)
                {
                    oPR = o.Save(CnxnString, LogPath);
                    if (oPR.Exception != null)
                        throw oPR.Exception;
                }
                oPR.Result += "Collection Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Log.LogErr("UserMedicalAnswerCollection Save", Exc.Message, LogPath);
                oPR.Exception = Exc;
                return (oPR);
            }
        }
        #endregion Save
    }



    public class UserMedicalAnswer : DataClassBase
    {

        #region Vars

        int _answerId;
        int _userId;
        string _GMT;
        string _utcTs;
        string _response;
        int _questionId;

        #endregion Vars

        #region Get/Sets

        public int answerId
        {
            get { return (_answerId); }
            set { _answerId = value; }
        }

        public int userId
        {
            get { return (_userId); }
            set { _userId = value; }
        }

        public string GMT
        {
            get { return (_GMT); }
            set { _GMT = value; }
        }

        public string utcTs
        {
            get { return (_utcTs); }
            set { _utcTs = value; }
        }

        public string response
        {
            get { return (_response); }
            set { _response = value; }
        }

        public int questionId
        {
            get { return (_questionId); }
            set { _questionId = value; }
        }

        #endregion Get/Sets

        #region Constructors

        public UserMedicalAnswer()
        {
        }

        public UserMedicalAnswer(int ID, string CnxnString, string LogPath)
        {
            // fill props from db
            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                SqlCommand cmd = new SqlCommand("spUserMedicalAnswerInfoFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    this.answerId = dr["answerId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["answerId"]);
                    this.userId = dr["userId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["userId"]);
                    this.utcTs = dr["utcTs"] == DBNull.Value ? "" : dr["utcTs"].ToString().Trim();
                    this.GMT = dr["GMT"] == DBNull.Value ? "" : dr["GMT"].ToString().Trim();
                    this.questionId = dr["questionId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["questionId"]);
                    this.response = dr["response"] == DBNull.Value ? "" : dr["response"].ToString().Trim();
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("UserMedicalAnswerConstructor", Exc.Message, LogPath);
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
            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                SqlCommand cmd = new SqlCommand("spUserMedicalAnswerSave", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for UserMedicalAnswers
                cmd.Parameters.Add(new SqlParameter("@answerId", SqlDbType.Int));
                cmd.Parameters["@answerId"].Value = this.answerId;

                cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int));
                cmd.Parameters["@userId"].Value = this.userId;

                cmd.Parameters.Add(new SqlParameter("@utcTs", SqlDbType.VarChar, 8));
                cmd.Parameters["@utcTs"].Value = this.utcTs ?? "";

                cmd.Parameters.Add(new SqlParameter("@GMT", SqlDbType.VarChar, 255));
                cmd.Parameters["@GMT"].Value = this.GMT ?? "";

                cmd.Parameters.Add(new SqlParameter("@questionId", SqlDbType.Int));
                cmd.Parameters["@questionId"].Value = this.questionId;

                cmd.Parameters.Add(new SqlParameter("@response", SqlDbType.VarChar, 255));
                cmd.Parameters["@response"].Value = this.response ?? "";

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
                Log.LogErr("UserMedicalAnswerSave", Exc.Message, LogPath);

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


        public static bool Delete(int ID, string CnxnString, string LogPath)
        {
            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                SqlCommand cmd = new SqlCommand("spUserMedicalAnswerDelete", Cnxn);
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
                Log.LogErr("UserMedicalAnswerDelete", Exc.Message, LogPath);
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