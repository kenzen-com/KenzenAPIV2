using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using KenzenAPI.Classes;
using Microsoft.Extensions.Configuration;

namespace KenzenAPI.DataClasses
{
    public class MedicalQuestionnaireCollection : Dictionary<int, MedicalQuestionnaire>
    {

        #region Constructors

        public MedicalQuestionnaireCollection()
        {
        }

        public MedicalQuestionnaireCollection(int ClientID, IConfiguration Config)
        {
            // fetch all from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spMedicalQuestionnairesFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    MedicalQuestionnaire oMedicalQuestionnaire = new MedicalQuestionnaire();
                    oMedicalQuestionnaire.question = dr["question"] == DBNull.Value ? "" : dr["question"].ToString().Trim();
                    oMedicalQuestionnaire.questionId = dr["questionId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["questionId"]);
                    oMedicalQuestionnaire.questionType = dr["questionType"] == DBNull.Value ? "" : dr["questionType"].ToString().Trim();
                    if (!this.ContainsKey(oMedicalQuestionnaire.ID))
                        this.Add(oMedicalQuestionnaire.ID, oMedicalQuestionnaire);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("MedicalQuestionnaireCollectionConstructor", Exc.Message, Config["LogPath"]);
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
                foreach (MedicalQuestionnaire o in this.Values)
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
                Log.LogErr("MedicalQuestionnaireCollection Save", Exc.Message, Config["LogPath"]);
                oPR.Exception = Exc;
                return (oPR);
            }
        }
        #endregion Save
    }



    public class MedicalQuestionnaire : DataClassBase
    {

        #region Vars

        string _question;
        string _questionType;
        int _questionId;

        #endregion Vars

        #region Get/Sets

        public string question
        {
            get { return (_question); }
            set { _question = value; }
        }

        public string questionType
        {
            get { return (_questionType); }
            set { _questionType = value; }
        }

        public int questionId
        {
            get { return (_questionId); }
            set { _questionId = value; }
        }

        #endregion Get/Sets

        #region Constructors

        public MedicalQuestionnaire()
        {
        }

        public MedicalQuestionnaire(int MedicalQuestionnaireID)
        {
            // fill props from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spMedicalQuestionnaireInfoFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@MedicalQuestionnaireID", SqlDbType.Int));
                cmd.Parameters["@MedicalQuestionnaireID"].Value = MedicalQuestionnaireID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    this.question = dr["question"] == DBNull.Value ? "" : dr["question"].ToString().Trim();
                    this.questionId = dr["questionId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["questionId"]);
                    this.questionType = dr["questionType"] == DBNull.Value ? "" : dr["questionType"].ToString().Trim();
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("MedicalQuestionnaireConstructor", Exc.Message, Config["LogPath"]);
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

                SqlCommand cmd = new SqlCommand("spMedicalQuestionnaireSave", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for MedicalQuestionnaire
                cmd.Parameters.Add(new SqlParameter("@question", SqlDbType.VarChar, 255));
                cmd.Parameters["@question"].Value = this.question ?? "";

                cmd.Parameters.Add(new SqlParameter("@questionId", SqlDbType.Int));
                cmd.Parameters["@questionId"].Value = this.questionId;

                cmd.Parameters.Add(new SqlParameter("@questionType", SqlDbType.VarChar, 255));
                cmd.Parameters["@questionType"].Value = this.questionType ?? "";

                // assign output param
                cmd.Parameters.Add(new SqlParameter("@MedicalQuestionnaireIDOut", SqlDbType.Int));
                cmd.Parameters["@MedicalQuestionnaireIDOut"].Direction = ParameterDirection.Output;

                #endregion Parameters

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();

                int iMedicalQuestionnaireID = Convert.ToInt32(cmd.Parameters["@MedicalQuestionnaireIDOut"].Value);
                this.ID = iMedicalQuestionnaireID;

                oPR.ObjectProcessed = this;
                oPR.Result += "Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Log.LogErr("MedicalQuestionnaireSave", Exc.Message, Config["LogPath"]);

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


        public static bool Delete(int MedicalQuestionnaireID, int ClientID, IConfiguration Config)
        {
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spMedicalQuestionnaireDelete", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@MedicalQuestionnaireID", SqlDbType.Int));
                cmd.Parameters["@MedicalQuestionnaireID"].Value = MedicalQuestionnaireID;

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();
                return (true);
            }
            catch (Exception Exc)
            {
                Log.LogErr("MedicalQuestionnaireDelete", Exc.Message, Config["LogPath"]);
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