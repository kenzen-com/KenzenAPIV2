using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.Configuration;
using KenzenAPI.Classes;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Linq;

namespace KenzenAPI.DataClasses
{

    public class UserCollection : Dictionary<int, User>
    {

        #region Constructors
        public UserCollection() { }
        public UserCollection(ILogger Logger, IConfiguration Config, int ClientID = 0)
        {
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                User oUser = new User(Logger, Config);
                string sSQL = "spUsersFetch";
                if (ClientID > 0)
                {
                    Client c = new Client(ClientID, Logger, Config);
                    sSQL = "spUsersFetch";
                }

                SqlCommand cmd = new SqlCommand(sSQL, Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;
                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    oUser = new User(null, null);
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
                    if (!this.ContainsKey(oUser.ID))
                        this.Add(oUser.ID, oUser);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("UserCollectionConstructor", Exc.Message, Config["LogPath"]);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }
        }
  
        #endregion Constructors

    }



    public class User : DataClassBase
    {
        #region Vars

        decimal _Weight;
        string _CountryOfResidence;
        int _Platform;
        string _WorkDayStart;
        decimal _Vest;
        decimal _WorkDayLength;
        string _DateOfBirth;
        string _Gender;
        int _TeamID;
        string _LastName;
        string _LastUpdatedUTC;
        int _GMT;
        decimal _Height;
        string _LastLoginUTC;
        string _Username;
        string _UTC;
        string _EmailAddress;
        string _FirstName;
        string _Measure;

        #endregion Vars

        #region Get/Sets

        public decimal Weight
        {
            get { return (_Weight); }
            set { _Weight = value; }
        }

        public string CountryOfResidence
        {
            get { return (_CountryOfResidence); }
            set { _CountryOfResidence = value; }
        }

        public int Platform
        {
            get { return (_Platform); }
            set { _Platform = value; }
        }

        public string WorkDayStart
        {
            get { return (_WorkDayStart); }
            set { _WorkDayStart = value; }
        }

        public decimal Vest
        {
            get { return (_Vest); }
            set { _Vest = value; }
        }

        public decimal WorkDayLength
        {
            get { return (_WorkDayLength); }
            set { _WorkDayLength = value; }
        }

        public string DateOfBirth
        {
            get { return (_DateOfBirth); }
            set { _DateOfBirth = value; }
        }

        public string Gender
        {
            get { return (_Gender); }
            set { _Gender = value; }
        }

        public int TeamID
        {
            get { return (_TeamID); }
            set { _TeamID = value; }
        }

        public string LastName
        {
            get { return (_LastName); }
            set { _LastName = value; }
        }

        public string LastUpdatedUTC
        {
            get { return (_LastUpdatedUTC); }
            set { _LastUpdatedUTC = value; }
        }

        public int GMT
        {
            get { return (_GMT); }
            set { _GMT = value; }
        }

        public decimal Height
        {
            get { return (_Height); }
            set { _Height = value; }
        }

        public string LastLoginUTC
        {
            get { return (_LastLoginUTC); }
            set { _LastLoginUTC = value; }
        }


        public string Username
        {
            get { return (_Username); }
            set { _Username = value; }
        }

        public string EmailAddress
        {
            get { return (_EmailAddress); }
            set { _EmailAddress = value; }
        }

        public string FirstName
        {
            get { return (_FirstName); }
            set { _FirstName = value; }
        }

        public string Measure
        {
            get { return (_Measure); }
            set { _Measure = value; }
        }

        #endregion Get/Sets

        #region Constructors
        public User()
        {
        }
        public User(ILogger logger, IConfiguration config)
        {
            Logger = logger;
            Config = config;
            TableName = "Users";
        }

        public User(int ID, ILogger logger, IConfiguration config) : this(logger, config)
        {
            string CnxnString = Config["CnxnString"];
            string LogPath = Config["LogPath"];

            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                SqlCommand cmd = new SqlCommand("SELECT * FROM " + this.TableName + " WHERE ID = @ID", Cnxn);

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = ID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    this.DateOfBirth = dr["DateOfBirth"] == DBNull.Value ? "" : dr["DateOfBirth"].ToString().Trim();
                    this.ID = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]);
                    this.UTC = dr["UTC"] == DBNull.Value ? "" : dr["UTC"].ToString().Trim();
                    this.Username = dr["Username"] == DBNull.Value ? "" : dr["Username"].ToString().Trim();
                    this.LastLoginUTC = dr["LastLoginUTC"] == DBNull.Value ? "" : dr["LastLoginUTC"].ToString().Trim();
                    this.TeamID = dr["TeamID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["TeamID"]);
                    this.CountryOfResidence = dr["CountryOfResidence"] == DBNull.Value ? "" : dr["CountryOfResidence"].ToString().Trim();
                    this.GMT = dr["GMT"] == DBNull.Value ? 0 : Convert.ToInt32(dr["GMT"]);
                    this.EmailAddress = dr["EmailAddress"] == DBNull.Value ? "" : dr["EmailAddress"].ToString().Trim();
                    this.LastName = dr["LastName"] == DBNull.Value ? "" : dr["LastName"].ToString().Trim();
                    this.FirstName = dr["FirstName"] == DBNull.Value ? "" : dr["FirstName"].ToString().Trim();
                    this.Vest = dr["Vest"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Vest"]);
                    this.Measure = dr["Measure"] == DBNull.Value ? "" : dr["Measure"].ToString().Trim();
                    this.WorkDayLength = dr["WorkDayLength"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["WorkDayLength"]);
                    this.Height = dr["Height"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Height"]);
                    this.ClientID = dr["ClientID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ClientID"]);
                    this.Gender = dr["Gender"] == DBNull.Value ? "" : dr["Gender"].ToString().Trim();
                    this.Weight = dr["Weight"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Weight"]);
                    this.WorkDayStart = dr["WorkDayStart"] == DBNull.Value ? "" : dr["WorkDayStart"].ToString().Trim();
                    this.Platform = dr["Platform"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Platform"]);
                    this.LastUpdatedUTC = dr["LastUpdatedUTC"] == DBNull.Value ? "" : dr["LastUpdatedUTC"].ToString().Trim();
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("UserConstructor", Exc.Message, LogPath);
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
            string CnxnString = Config["CnxnString"];
            string LogPath = Config["LogPath"];

            ProcessResult oPR = new ProcessResult();
            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {
                if (this.ID == 0)
                {
                    List<User> o = new UserCollection(Logger, Config).Values.ToList();
                    User u = o.Find(q => q.Username == this.Username);
                    if(u != null)
                    {
                        oPR.Result = "User already exists.";
                        return (oPR);
                    }
                }

                SqlCommand cmd = new SqlCommand("spUserSave", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                #region Parameters
                // parameters for Users
                cmd.Parameters.Add(new SqlParameter("@DateOfBirth", SqlDbType.VarChar, 50));
                cmd.Parameters["@DateOfBirth"].Value = this.DateOfBirth ?? "";

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = this.ID;

                cmd.Parameters.Add(new SqlParameter("@UTC", SqlDbType.VarChar, 50));
                cmd.Parameters["@UTC"].Value = this.UTC ?? DateTime.Now.ToString();

                cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.VarChar, 50));
                cmd.Parameters["@Username"].Value = this.Username ?? "";

                cmd.Parameters.Add(new SqlParameter("@LastLoginUTC", SqlDbType.VarChar, 50));
                cmd.Parameters["@LastLoginUTC"].Value = this.LastLoginUTC ?? "";

                cmd.Parameters.Add(new SqlParameter("@TeamID", SqlDbType.Int));
                cmd.Parameters["@TeamID"].Value = this.TeamID;

                cmd.Parameters.Add(new SqlParameter("@CountryOfResidence", SqlDbType.VarChar, 50));
                cmd.Parameters["@CountryOfResidence"].Value = this.CountryOfResidence ?? "";

                cmd.Parameters.Add(new SqlParameter("@GMT", SqlDbType.Int));
                cmd.Parameters["@GMT"].Value = this.GMT;

                cmd.Parameters.Add(new SqlParameter("@EmailAddress", SqlDbType.VarChar, 255));
                cmd.Parameters["@EmailAddress"].Value = this.EmailAddress ?? "";

                cmd.Parameters.Add(new SqlParameter("@LastName", SqlDbType.VarChar, 50));
                cmd.Parameters["@LastName"].Value = this.LastName ?? "";

                cmd.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.VarChar, 50));
                cmd.Parameters["@FirstName"].Value = this.FirstName ?? "";

                cmd.Parameters.Add(new SqlParameter("@Vest", SqlDbType.Money));
                cmd.Parameters["@Vest"].Value = this.Vest;

                cmd.Parameters.Add(new SqlParameter("@Measure", SqlDbType.VarChar, 255));
                cmd.Parameters["@Measure"].Value = this.Measure ?? "";

                cmd.Parameters.Add(new SqlParameter("@WorkDayLength", SqlDbType.Money));
                cmd.Parameters["@WorkDayLength"].Value = this.WorkDayLength;

                cmd.Parameters.Add(new SqlParameter("@Height", SqlDbType.Money));
                cmd.Parameters["@Height"].Value = this.Height;

                cmd.Parameters.Add(new SqlParameter("@ClientID", SqlDbType.Int));
                cmd.Parameters["@ClientID"].Value = this.ClientID;

                cmd.Parameters.Add(new SqlParameter("@Gender", SqlDbType.VarChar, 50));
                cmd.Parameters["@Gender"].Value = this.Gender ?? "";

                cmd.Parameters.Add(new SqlParameter("@Weight", SqlDbType.Money));
                cmd.Parameters["@Weight"].Value = this.Weight;

                cmd.Parameters.Add(new SqlParameter("@WorkDayStart", SqlDbType.VarChar, 50));
                cmd.Parameters["@WorkDayStart"].Value = this.WorkDayStart ?? "";

                cmd.Parameters.Add(new SqlParameter("@Platform", SqlDbType.Int));
                cmd.Parameters["@Platform"].Value = this.Platform;

                cmd.Parameters.Add(new SqlParameter("@LastUpdatedUTC", SqlDbType.VarChar, 50));
                cmd.Parameters["@LastUpdatedUTC"].Value = this.LastUpdatedUTC ?? "";

                // assign output param
                cmd.Parameters.Add(new SqlParameter("@UserIDOut", SqlDbType.Int));
                cmd.Parameters["@UserIDOut"].Direction = ParameterDirection.Output;

                #endregion Parameters

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();

                int iUserID = Convert.ToInt32(cmd.Parameters["@UserIDOut"].Value);
                this.ID = iUserID;

                oPR.ObjectProcessed = this;
                oPR.Result += "Saved";
                return (oPR);

            }
            catch (Exception Exc)
            {
                Log.LogErr("UserSave", Exc.Message, LogPath);

                oPR.Exception = Exc;
                oPR.Result += "Error";
                return (oPR);
            }
        }
        #endregion Save

        #region Delete


        public bool Delete()
        {
            string CnxnString = Config["CnxnString"];
            string LogPath = Config["LogPath"];

            SqlConnection Cnxn = new SqlConnection(CnxnString);
            try
            {

                SqlCommand cmd = new SqlCommand("DELETE FROM " + this.TableName + " WHERE ID = @ID", Cnxn);

                cmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.Int));
                cmd.Parameters["@ID"].Value = this.ID;

                Cnxn.Open();
                cmd.ExecuteNonQuery();
                Cnxn.Close();
                return (true);
            }
            catch (Exception Exc)
            {
                Log.LogErr("UserDelete", Exc.Message, LogPath);
                return (false);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }


        }
        #endregion Delete

        #region FetchRoles
        public static ProcessResult FetchRoles(int UserID, int ClientID, IConfiguration Config)
        {
            ProcessResult oPR = new ProcessResult();
            List<string> Roles = new List<string>();

            SqlConnection Cnxn = new SqlConnection(Config["CnxnString"]);
            try
            {
                User u = new User(null, null);
                SqlCommand cmd = new SqlCommand("spUserRolesFetch", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int));
                cmd.Parameters["@UserID"].Value = UserID;

                cmd.Parameters.Add(new SqlParameter("@ClientID", SqlDbType.Int));
                cmd.Parameters["@ClientID"].Value = ClientID;

                Cnxn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Roles.Add(dr[0].ToString());
                }

                dr.Close();
                Cnxn.Close();

                oPR.ObjectProcessed = Roles;
            }
            catch (Exception Exc)
            {
                oPR.Exception = Exc;
                Log.LogErr("FetchRolesStatic", Exc.Message, Config["LogPath"]);
            }

            return (oPR);

        }
        #endregion FetchRoles

        public static List<TemperatureHumidity> FetchTemperatureHumidities(int UserID, int ClientID, IConfiguration Config)
        {
            List<TemperatureHumidity> oTHs = new List<TemperatureHumidity>();
            // fetch all from db
            SqlConnection Cnxn = new SqlConnection(Client.GetCnxnString(ClientID, Config));
            try
            {

                SqlCommand cmd = new SqlCommand("spTemperatureHumiditiesFetchByUser", Cnxn);
                cmd.CommandType = CommandType.StoredProcedure;


                cmd.Parameters.Add(new SqlParameter("@UserID", SqlDbType.Int));
                cmd.Parameters["@UserID"].Value = UserID;


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
                    oTHs.Add(oTemperatureHumidity);
                }

                dr.Close();
                Cnxn.Close();
            }
            catch (Exception Exc)
            {
                Log.LogErr("User.TemperatureHumidityList", Exc.Message, Config["LogPath"]);
            }
            finally
            {
                if (Cnxn.State == ConnectionState.Open) Cnxn.Close();
            }

            return oTHs;
        }


    }
}