using KenzenAPI.DataClasses;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KenzenAPI
{
    public class QuickCache
    {
        public ILogger Logger;
        public IConfiguration Config;
        public static List<APIRole> Roles = new List<APIRole>();
        public static List<APIUserRole> UserRoles = new List<APIUserRole>();

    }
}
