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
	public class DailyFeedbackQuestionnaireCollection : Dictionary<int, DailyFeedbackQuestionnaire>
	{

		#region Constructors

		public DailyFeedbackQuestionnaireCollection()
		{
		}

		public DailyFeedbackQuestionnaireCollection(int ClientID, ILogger Logger, IConfiguration Config)
		{
			// fetch all from db
			SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
			try
			{

				SqlCommand cmd = new SqlCommand("spDailyFeedbackQuestionnairesFetch", Cnxn);
				cmd.CommandType = CommandType.StoredProcedure;

				Cnxn.Open();
				SqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					DailyFeedbackQuestionnaire oDailyFeedbackQuestionnaire = new DailyFeedbackQuestionnaire(null, null);
					oDailyFeedbackQuestionnaire.QuestionType = dr["QuestionType"] == DBNull.Value ? "" : dr["QuestionType"].ToString().Trim();
					oDailyFeedbackQuestionnaire.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
					oDailyFeedbackQuestionnaire.QuestionText = dr["QuestionText"] == DBNull.Value ? "" : dr["QuestionText"].ToString().Trim();
					if (!this.ContainsKey(oDailyFeedbackQuestionnaire.ID))
						this.Add(oDailyFeedbackQuestionnaire.ID, oDailyFeedbackQuestionnaire);
				}

				dr.Close();
				Cnxn.Close();
			}
			catch (Exception Exc)
			{
				Logger.Error("DailyFeedbackQuestionnaireCollectionConstructor", Exc.Message, Config["LogPath"]);
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
				foreach (DailyFeedbackQuestionnaire o in this.Values)
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
				Logger.Error("DailyFeedbackQuestionnaireCollection Save", Exc.Message, Config["LogPath"]);
				oPR.Exception = Exc;
				return (oPR);
			}
		}
		#endregion Save
	}



	public class DailyFeedbackQuestionnaire : DataClassBase
	{

		#region Vars

		int _ID;
		string _QuestionType;
		string _QuestionText;

		#endregion Vars

		#region Get/Sets
		public string QuestionType
		{
			get { return (_QuestionType); }
			set { _QuestionType = value; }
		}

		public string QuestionText
		{
			get { return (_QuestionText); }
			set { _QuestionText = value; }
		}

		#endregion Get/Sets

		#region Constructors

		public DailyFeedbackQuestionnaire(ILogger logger, IConfiguration config)
		{
			Logger = logger;
			Config = config;
		}

		public DailyFeedbackQuestionnaire(int DailyFeedbackQuestionnaireID)
		{
			// fill props from db
			SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
			try
			{

				SqlCommand cmd = new SqlCommand("spDailyFeedbackQuestionnaireInfoFetch", Cnxn);
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new SqlParameter("@DailyFeedbackQuestionnaireID", SqlDbType.Int));
				cmd.Parameters["@DailyFeedbackQuestionnaireID"].Value = DailyFeedbackQuestionnaireID;

				Cnxn.Open();
				SqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{

					this.QuestionType = dr["QuestionType"] == DBNull.Value ? "" : dr["QuestionType"].ToString().Trim();
					this.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
					this.QuestionText = dr["QuestionText"] == DBNull.Value ? "" : dr["QuestionText"].ToString().Trim();
				}

				dr.Close();
				Cnxn.Close();
			}
			catch (Exception Exc)
			{
				Logger.Error("DailyFeedbackQuestionnaireConstructor", Exc.Message, Config["LogPath"]);
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

				SqlCommand cmd = new SqlCommand("spDailyFeedbackQuestionnaireSave", Cnxn);
				cmd.CommandType = CommandType.StoredProcedure;

				#region Parameters
				// parameters for DailyFeedbackQuestionnaire
				cmd.Parameters.Add(new SqlParameter("@QuestionType", SqlDbType.VarChar, 255));
				cmd.Parameters["@QuestionType"].Value = this.QuestionType ?? "";

				cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
				cmd.Parameters["@ID"].Value = this.ID;

				cmd.Parameters.Add(new SqlParameter("@QuestionText", SqlDbType.VarChar, 255));
				cmd.Parameters["@QuestionText"].Value = this.QuestionText ?? "";

				// assign output param
				cmd.Parameters.Add(new SqlParameter("@DailyFeedbackQuestionnaireIDOut", SqlDbType.Int));
				cmd.Parameters["@DailyFeedbackQuestionnaireIDOut"].Direction = ParameterDirection.Output;

				#endregion Parameters

				Cnxn.Open();
				cmd.ExecuteNonQuery();
				Cnxn.Close();

				int iDailyFeedbackQuestionnaireID = Convert.ToInt32(cmd.Parameters["@DailyFeedbackQuestionnaireIDOut"].Value);
				this.ID = iDailyFeedbackQuestionnaireID;

				oPR.ObjectProcessed = this;
				oPR.Result += "Saved";
				return (oPR);

			}
			catch (Exception Exc)
			{
				Logger.Error("DailyFeedbackQuestionnaireSave", Exc.Message, Config["LogPath"]);

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


		public static bool Delete(int DailyFeedbackQuestionnaireID, int ClientID, ILogger Logger, IConfiguration Config)
		{
			SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
			try
			{

				SqlCommand cmd = new SqlCommand("spDailyFeedbackQuestionnaireDelete", Cnxn);
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.Add(new SqlParameter("@DailyFeedbackQuestionnaireID", SqlDbType.Int));
				cmd.Parameters["@DailyFeedbackQuestionnaireID"].Value = DailyFeedbackQuestionnaireID;

				Cnxn.Open();
				cmd.ExecuteNonQuery();
				Cnxn.Close();
				return (true);
			}
			catch (Exception Exc)
			{
				Logger.Error("DailyFeedbackQuestionnaireDelete", Exc.Message, Config["LogPath"]);
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