using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSwag.Annotations;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KenzenAPI.Classes
{
    public abstract class DataClassBase
    {
        string _UTC;
        public ILogger Logger;
        public IConfiguration Config;
        [OpenApiIgnore]
        internal string TableName { get; set; }
        public string UTC
        {
            get { return (Convert.ToDateTime(_UTC).ToString("o")); }
            set { _UTC = value; }
        }

        public int ID { get; set; }
        public int ClientID { get; set; }

        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
