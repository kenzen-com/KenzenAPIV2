using Microsoft.Extensions.Configuration;
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
        public string SchemaName { get; set; }
        public string TableName { get; set; }
    }
}
