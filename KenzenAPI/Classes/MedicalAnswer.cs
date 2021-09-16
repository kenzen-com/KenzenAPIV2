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

    public class MedicalAnswerCollection : Dictionary<int, MedicalAnswer>
    {

        #region Constructors

        public MedicalAnswerCollection()
        {
        }

        public MedicalAnswerCollection(ILogger Logger, IConfiguration Config)
        {
            // fetch all from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(1, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spMedicalAnswersFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    MedicalAnswer oMedicalAnswer = new MedicalAnswer();
                    oMedicalAnswer.Text = dr["Text"] == DBNull.Value ? "" : dr["Text"].ToString().Trim();
                    oMedicalAnswer.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    oMedicalAnswer.MedicalQuestionnaireID = dr["MedicalQuestionnaireID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MedicalQuestionnaireID"]);
                    if (!this.ContainsKey(oMedicalAnswer.ID))
                        this.Add(oMedicalAnswer.ID, oMedicalAnswer);
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
                foreach (MedicalAnswer o in this.Values)
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



    public class MedicalAnswer : DataClassBase
    {

        #region Vars

        string _Text;
        int _ID;
        int _MedicalQuestionnaireID;

        #endregion Vars

        #region Get/Sets

        public string Text
        {
            get { return (_Text); }
            set { _Text = value; }
        }

        public int ID
        {
            get { return (_ID); }
            set { _ID = value; }
        }

        public int MedicalQuestionnaireID
        {
            get { return (_MedicalQuestionnaireID); }
            set { _MedicalQuestionnaireID = value; }
        }

        #endregion Get/Sets

        #region Constructors

        public MedicalAnswer()
        {
        }

        public MedicalAnswer(int ID, string CnxnString, string LogPath)
        {
            // fill props from db
            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                SqlCommand cmd = new SqlCommand("spMedicalAnswerInfoFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    this.Text = dr["Text"] == DBNull.Value ? "" : dr["Text"].ToString().Trim();
                    this.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    this.MedicalQuestionnaireID = dr["MedicalQuestionnaireID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MedicalQuestionnaireID"]);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("MedicalAnswerConstructor", Exc.Message, LogPath);
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

                SqlCommand cmd = new SqlCommand("spMedicalAnswerSave", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for MedicalAnswer
                cmd.Parameters.Add(new SqlParameter("@Text", SqlDbType.VarChar, 255));
                cmd.Parameters["@Text"].Value = this.Text ?? "";

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = this.ID;

                cmd.Parameters.Add(new SqlParameter("@MedicalQuestionnaireID", SqlDbType.Int));
                cmd.Parameters["@MedicalQuestionnaireID"].Value = this.MedicalQuestionnaireID;

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


        public static bool Delete(int ID, string CnxnString, string LogPath)
        {
            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                SqlCommand cmd = new SqlCommand("spMedicalAnswerDelete", Cnxn);
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
                Log.LogErr("MedicalAnswerDelete", Exc.Message, LogPath);
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