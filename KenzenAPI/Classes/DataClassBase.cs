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


        public int ID { get; set; }
        public int ClientID { get; set; }

        [JsonIgnore]
        public string TableName { get; set; }

        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
