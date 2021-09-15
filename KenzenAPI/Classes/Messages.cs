using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Serilog;
using Microsoft.Extensions.Configuration;

namespace KenzenAPI.Classes
{

    public class MessageCollection : Dictionary<int, Message>
    {

        #region Constructors
        ILogger Logger;
        IConfiguration Config;
        public MessageCollection(ILogger logger, IConfiguration config, int ClientID)
        {
            Logger = logger;
            Config = config;
            // fetch all from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spMessagesFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Message oMessage = new Message(Logger, Config);
                    oMessage.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                    oMessage.ClientID = dr["ClientID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ClientID"]);
                    oMessage.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    oMessage.MessageBody = dr["Message"] == DBNull.Value ? "" : dr["Message"].ToString().Trim();
                    oMessage.DeviceID = dr["DeviceID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DeviceID"]);
                    if (!this.ContainsKey(oMessage.ID))
                        this.Add(oMessage.ID, oMessage);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error("MessageCollectionConstructor", Exc.Message, Config["LogPath"]);
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
            try
            {
                foreach (Message o in this.Values)
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
                Logger.Error("MessageCollection Save", Exc.Message, Config["LogPath"]);
                oPR.Exception = Exc;
                return (oPR);
            }
        }
        #endregion Save
    }



    public class Message : DataClassBase
    {

        #region Vars

        string _Message;
        string _MessageType;
        int _DeviceID;

        #endregion Vars

        #region Get/Sets

        public string MessageBody
        {
            get { return (_Message); }
            set { _Message = value; }
        }

        public int DeviceID
        {
            get { return (_DeviceID); }
            set { _DeviceID = value; }
        }


        public string MessageType { get => _MessageType; set => _MessageType = value; }

        #endregion Get/Sets

        #region Constructors
        public Message(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
            TableName = "Messages";
        }
        public Message() { }

        public Message(int ID)
        {
            // fill props from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(this.ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spMessageInfoFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    this.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                    this.ClientID = dr["ClientID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ClientID"]);
                    this.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    this.MessageBody = dr["Message"] == DBNull.Value ? "" : dr["Message"].ToString().Trim();
                    this.DeviceID = dr["DeviceID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DeviceID"]);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Logger.Error("MessageConstructor", Exc.Message, Config["LogPath"]);
                Logger.Error(Exc.Message);
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
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(this.ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spMessageSave", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for Messages
                cmd.Parameters.Add(new SqlParameter("@UTC", SqlDbType.VarChar, 50));
                cmd.Parameters["@UTC"].Value = this.UTC ?? DateTime.Now.ToString();

                cmd.Parameters.Add(new SqlParameter("@ClientID", SqlDbType.Int));
                cmd.Parameters["@ClientID"].Value = this.ClientID;

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = this.ID;

                cmd.Parameters.Add(new SqlParameter("@Message", SqlDbType.VarChar, 2147483647));
                cmd.Parameters["@Message"].Value = this.MessageBody ?? "";

                cmd.Parameters.Add(new SqlParameter("@DeviceID", SqlDbType.Int));
                cmd.Parameters["@DeviceID"].Value = this.DeviceID;

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
                Logger.Error("MessageSave", Exc.Message, Config["LogPath"]);

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

                SqlCommand cmd = new SqlCommand("spMessageDelete", Cnxn);
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
                Logger.Error("MessageDelete", Exc.Message, Config["LogPath"]);
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