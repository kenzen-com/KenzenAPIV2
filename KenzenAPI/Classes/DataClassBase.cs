using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KenzenAPI.Classes
{
    public abstract class DataClassBase
    {
        [JsonIgnore]
        public ILogger Logger;
        [JsonIgnore]
        public IConfiguration Config;
        [JsonIgnore]
        internal string TableName { get; set; }
        public string _UTC;
        public string UTC
        {
            get { return (Convert.ToDateTime(_UTC).ToString("o")); }
            set { _UTC = value; }
        }
        public string UserUTC
        {
            get { return (Convert.ToDateTime(_UTC).AddHours(GMT).ToString("o")); }
        }
        public int GMT { get; set; }
        public int ID { get; set; }
        public int UserID { get; set; }
        public int ClientID { get; set; }

        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
