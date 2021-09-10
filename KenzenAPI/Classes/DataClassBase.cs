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
        public ILogger Logger;
        public IConfiguration Config;


        public int ID { get; set; }
        public int ClientID { get; set; }
        public string SchemaName { get; set; } = "dbo";
        public string TableName { get; set; }

        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
