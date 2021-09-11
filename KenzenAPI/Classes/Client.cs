using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KenzenAPI.Classes
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Serilog;
    using Microsoft.Extensions.Configuration;
    using KenzenAPI.DataClasses;
    using AzureWrapper;

    public class ClientCollection : Dictionary<int, Client>
    {
        public ILogger Logger;
        public IConfiguration Config;

        #region Constructors
        public ClientCollection() { }
        public ClientCollection(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;

            // fetch all from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(0, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spClientsFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Client oClient = new Client(Logger, Config);
                    oClient.FillProps(dr);
                    if (!this.ContainsKey(oClient.ID))
                        this.Add(oClient.ID, oClient);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                KenzenAPI.Log.LogErr("ClientCollectionConstructor", Exc.Message, Config["LogPath"]);
                Logger.Error("ClientCollectionConstructor:" + Exc.Message);
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
                foreach (Client o in this.Values)
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
                KenzenAPI.Log.LogErr("ClientCollection Save", Exc.Message, Config["LogPath"]);
                Logger.Error("ClientCollection Save:" + Exc.Message);
                oPR.Exception = Exc;
                return (oPR);
            }
        }
        #endregion Save
    }



    public class Client : DataClassBase
    {

        #region Vars

        string _SchemaName;
        string _Name;
        string _APIKey;
        string _LastEditUTC;
        string _Zip;
        string _Address;
        string _ContactPhone;
        int _ID;
        string _UTC;
        int _LastEditBy;
        string _State;
        string _City;
        string _ContactName;
        string _ContactEmail;
        bool _IsEncrypted;
        bool _IsPrivate;

        #endregion Vars

        #region Get/Sets

        public string SchemaName
        {
            get { return (_SchemaName); }
            set { _SchemaName = value; }
        }

        public string Name
        {
            get { return (_Name); }
            set { _Name = value; }
        }

        public string APIKey
        {
            get { return (_APIKey); }
            set { _APIKey = value; }
        }

        public string LastEditUTC
        {
            get { return (_LastEditUTC); }
            set { _LastEditUTC = value; }
        }

        public string Zip
        {
            get { return (_Zip); }
            set { _Zip = value; }
        }

        public string Address
        {
            get { return (_Address); }
            set { _Address = value; }
        }

        public string ContactPhone
        {
            get { return (_ContactPhone); }
            set { _ContactPhone = value; }
        }

        public int ID
        {
            get { return (_ID); }
            set { _ID = value; }
        }

        public string UTC
        {
            get { return (_UTC); }
            set { _UTC = value; }
        }

        public int LastEditBy
        {
            get { return (_LastEditBy); }
            set { _LastEditBy = value; }
        }

        public string State
        {
            get { return (_State); }
            set { _State = value; }
        }

        public string City
        {
            get { return (_City); }
            set { _City = value; }
        }

        public string ContactName
        {
            get { return (_ContactName); }
            set { _ContactName = value; }
        }

        public string ContactEmail
        {
            get { return (_ContactEmail); }
            set { _ContactEmail = value; }
        }

        public bool IsEncrypted { get => _IsEncrypted; set => _IsEncrypted = value; }
        public bool IsPrivate { get => _IsPrivate; set => _IsPrivate = value; }

        #endregion Get/Sets

        #region Constructors
        ILogger Logger;
        IConfiguration Config;
        public Client(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
            TableName = "Clients";

        }

        public Client(int ClientID, ILogger logger, IConfiguration config) : this(logger, config)
        {
            // fill props from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            if (!QuickCache.CustomClients.Contains(ClientID))
                Cnxn = new SqlConnection(Config["CnxnString" + ClientID]);
            try
            {

                SqlCommand cmd = new SqlCommand("spClientInfoFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ClientID", SqlDbType.Int));
                cmd.Parameters["@ClientID"].Value = ClientID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                FillProps(dr);
                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                KenzenAPI.Log.LogErr("ClientConstructor", Exc.Message, Config["LogPath"]);
                Logger.Error("ClientConstructor:" + Exc.Message);
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

                SqlCommand cmd = new SqlCommand("spClientSave", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for Clients
                cmd.Parameters.Add(new SqlParameter("@Address", SqlDbType.VarChar, 50));
                cmd.Parameters["@Address"].Value = this.Address ?? "";

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = this.ID;

                cmd.Parameters.Add(new SqlParameter("@LastEditBy", SqlDbType.Int));
                cmd.Parameters["@LastEditBy"].Value = this.LastEditBy;

                cmd.Parameters.Add(new SqlParameter("@IsEncrypted", SqlDbType.Bit));
                cmd.Parameters["@IsEncrypted"].Value = this.IsEncrypted;

                cmd.Parameters.Add(new SqlParameter("@Name", SqlDbType.VarChar, 50));
                cmd.Parameters["@Name"].Value = this.Name ?? "";

                cmd.Parameters.Add(new SqlParameter("@City", SqlDbType.VarChar, 50));
                cmd.Parameters["@City"].Value = this.City ?? "";

                cmd.Parameters.Add(new SqlParameter("@State", SqlDbType.VarChar, 50));
                cmd.Parameters["@State"].Value = this.State ?? "";

                cmd.Parameters.Add(new SqlParameter("@ContactPhone", SqlDbType.VarChar, 50));
                cmd.Parameters["@ContactPhone"].Value = this.ContactPhone ?? "";

                cmd.Parameters.Add(new SqlParameter("@ContactEmail", SqlDbType.VarChar, 50));
                cmd.Parameters["@ContactEmail"].Value = this.ContactEmail ?? "";

                cmd.Parameters.Add(new SqlParameter("@ContactName", SqlDbType.VarChar, 50));
                cmd.Parameters["@ContactName"].Value = this.ContactName ?? "";

                cmd.Parameters.Add(new SqlParameter("@LastEditUTC", SqlDbType.VarChar, 50));
                cmd.Parameters["@LastEditUTC"].Value = DateTime.Now;

                cmd.Parameters.Add(new SqlParameter("@APIKey", SqlDbType.VarChar, 100));
                cmd.Parameters["@APIKey"].Value = this.APIKey ?? "";

                cmd.Parameters.Add(new SqlParameter("@Zip", SqlDbType.VarChar, 50));
                cmd.Parameters["@Zip"].Value = this.Zip ?? "";

                cmd.Parameters.Add(new SqlParameter("@SchemaName", SqlDbType.VarChar, 50));
                cmd.Parameters["@SchemaName"].Value = this.SchemaName ?? "";

                // assign output param
                cmd.Parameters.Add(new SqlParameter("@ClientIDOut", SqlDbType.Int));
                cmd.Parameters["@ClientIDOut"].Direction = ParameterDirection.Output;

                #endregion Parameters
                if (this.IsEncrypted)
                {
                    foreach (SqlParameter p in cmd.Parameters)
                    {
                        p.Value = Crypto.EncryptValue(p.Value.ToString(), Config["LogPath"]);
                    }
                }

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();

                int iClientID = Convert.ToInt32(cmd.Parameters["@ClientIDOut"].Value);
                this.ID = iClientID;

                oPR.ObjectProcessed = this;
                oPR.Result += "Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                KenzenAPI.Log.LogErr("ClientSave", Exc.Message, Config["LogPath"]);
                Logger.Error("ClientSave:" + Exc.Message);

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

        #region FillProps
        public void FillProps(SqlDataReader dr)
        {
            while (dr.Read())
            {
                if (!this.IsEncrypted)
                {
                    this.Address = dr["Address"] == DBNull.Value ? "" : dr["Address"].ToString().Trim();
                    this.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    this.LastEditBy = dr["LastEditBy"] == DBNull.Value ? 0 : Convert.ToInt32(dr["LastEditBy"]);
                    this.Name = dr["Name"] == DBNull.Value ? "" : dr["Name"].ToString().Trim();
                    this.City = dr["City"] == DBNull.Value ? "" : dr["City"].ToString().Trim();
                    this.State = dr["State"] == DBNull.Value ? "" : dr["State"].ToString().Trim();
                    this.ContactPhone = dr["ContactPhone"] == DBNull.Value ? "" : dr["ContactPhone"].ToString().Trim();
                    this.ContactEmail = dr["ContactEmail"] == DBNull.Value ? "" : dr["ContactEmail"].ToString().Trim();
                    this.ContactName = dr["ContactName"] == DBNull.Value ? "" : dr["ContactName"].ToString().Trim();
                    this.LastEditUTC = dr["LastEditUTC"] == DBNull.Value ? "" : dr["LastEditUTC"].ToString().Trim();
                    this.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                    this.APIKey = dr["APIKey"] == DBNull.Value ? "" : dr["APIKey"].ToString().Trim();
                    this.Zip = dr["Zip"] == DBNull.Value ? "" : dr["Zip"].ToString().Trim();
                    this.SchemaName = dr["SchemaName"] == DBNull.Value ? "" : dr["SchemaName"].ToString().Trim();
                    this.IsEncrypted = dr["IsEncrypted"] == DBNull.Value ? false : true;
                    this.IsPrivate = dr["IsPrivate"] == DBNull.Value ? false : true;
                }
                else
                {
                    this.Address = dr["Address"] == DBNull.Value ? "" : Crypto.DecryptValue(dr["Address"].ToString().Trim(), Config["LogPath"]);
                    this.Name = dr["Name"] == DBNull.Value ? "" : Crypto.DecryptValue(dr["Name"].ToString().Trim(), Config["LogPath"]);
                    this.City = dr["City"] == DBNull.Value ? "" : Crypto.DecryptValue(dr["City"].ToString().Trim(), Config["LogPath"]);
                    this.State = dr["State"] == DBNull.Value ? "" : Crypto.DecryptValue(dr["State"].ToString().Trim(), Config["LogPath"]);
                    this.ContactPhone = dr["ContactPhone"] == DBNull.Value ? "" : Crypto.DecryptValue(dr["ContactPhone"].ToString().Trim(), Config["LogPath"]);
                    this.ContactEmail = dr["ContactEmail"] == DBNull.Value ? "" : Crypto.DecryptValue(dr["ContactEmail"].ToString().Trim(), Config["LogPath"]);
                    this.ContactName = dr["ContactName"] == DBNull.Value ? "" : Crypto.DecryptValue(dr["ContactName"].ToString().Trim(), Config["LogPath"]);
                    this.LastEditUTC = dr["LastEditUTC"] == DBNull.Value ? "" : Crypto.DecryptValue(dr["LastEditUTC"].ToString().Trim(), Config["LogPath"]);
                    this.UTC = dr["UTC"] == DBNull.Value ? "" : Crypto.DecryptValue(dr["UTC"].ToString().Trim(), Config["LogPath"]);
                    this.APIKey = dr["APIKey"] == DBNull.Value ? "" : Crypto.DecryptValue(dr["APIKey"].ToString().Trim(), Config["LogPath"]);
                    this.Zip = dr["Zip"] == DBNull.Value ? "" : Crypto.DecryptValue(dr["Zip"].ToString().Trim(), Config["LogPath"]);
                    this.SchemaName = dr["SchemaName"] == DBNull.Value ? "" : Crypto.DecryptValue(dr["SchemaName"].ToString().Trim(), Config["LogPath"]);
                    this.IsEncrypted = dr["IsEncrypted"] == DBNull.Value ? false : true;
                    this.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    this.LastEditBy = dr["LastEditBy"] == DBNull.Value ? 0 : Convert.ToInt32(dr["LastEditBy"]);
                }
            }
        }
        #endregion FillProps

        #region Delete


        public bool Delete()
        {
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spClientDelete", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ClientID", SqlDbType.Int));
                cmd.Parameters["@ClientID"].Value = ID;

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();
                return (true);
            }
            catch (Exception Exc)
            {
                KenzenAPI.Log.LogErr("ClientDelete", Exc.Message, Config["LogPath"]);
                Logger.Error("ClientDelete:" + Exc.Message);
                return (false);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }


        }
        #endregion Delete

        public static ProcessResult Users(int ClientID, ILogger Logger, IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(0, Config));
            try
            {
                List<User> oOut = new List<User>();

                User oUser = new User(Logger, Config);
                SqlCommand cmd = new SqlCommand("spUsersFetchByClient", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ClientID", SqlDbType.Int).Value = ClientID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    oUser = new User(Logger, Config);
                    oUser.DateOfBirth = dr["DateOfBirth"] == DBNull.Value ? "" : dr["DateOfBirth"].ToString().Trim();
                    oUser.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    oUser.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                    oUser.Username = dr["Username"] == DBNull.Value ? "" : dr["Username"].ToString().Trim();
                    oUser.LastLoginUTC = dr["LastLoginUTC"] == DBNull.Value ? "" : dr["LastLoginUTC"].ToString().Trim();
                    oUser.TeamID = dr["TeamID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TeamID"]);
                    oUser.CountryOfResidence = dr["CountryOfResidence"] == DBNull.Value ? "" : dr["CountryOfResidence"].ToString().Trim();
                    oUser.GMT = dr["GMT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GMT"]);
                    oUser.EmailAddress = dr["EmailAddress"] == DBNull.Value ? "" : dr["EmailAddress"].ToString().Trim();
                    oUser.LastName = dr["LastName"] == DBNull.Value ? "" : dr["LastName"].ToString().Trim();
                    oUser.FirstName = dr["FirstName"] == DBNull.Value ? "" : dr["FirstName"].ToString().Trim();
                    oUser.Vest = dr["Vest"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Vest"]);
                    oUser.Measure = dr["Measure"] == DBNull.Value ? "" : dr["Measure"].ToString().Trim();
                    oUser.WorkDayLength = dr["WorkDayLength"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["WorkDayLength"]);
                    oUser.Height = dr["Height"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Height"]);
                    oUser.ClientID = dr["ClientID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ClientID"]);
                    oUser.Gender = dr["Gender"] == DBNull.Value ? "" : dr["Gender"].ToString().Trim();
                    oUser.Weight = dr["Weight"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Weight"]);
                    oUser.WorkDayStart = dr["WorkDayStart"] == DBNull.Value ? "" : dr["WorkDayStart"].ToString().Trim();
                    oUser.Platform = dr["Platform"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Platform"]);
                    oUser.LastUpdatedUTC = dr["LastUpdatedUTC"] == DBNull.Value ? "" : dr["LastUpdatedUTC"].ToString().Trim();

                    oOut.Add(oUser);

                }
                dr.Close();
                Cnxn.Close();
                oPR.ObjectProcessed = oOut;
            }
            catch (Exception Exc)
            {
                oPR.Exception = Exc;
                Logger.Error(Exc.Message);
            }

            return oPR;
        }

        public static string GetCnxnString(int ClientID, IConfiguration Config)
        {
            if (ClientID > 0)
                return Config["CnxnString" + ClientID];
            else
                return Config["CnxnString"];


        }
    }

}
