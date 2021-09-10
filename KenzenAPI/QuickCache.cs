using KenzenAPI.Classes.Lookup;
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
        public static List<Role> Roles = new List<Role>();
        public static List<int> CustomClients = new List<int>();
        public static List<UserRole> UserRoles = new List<UserRole>();

    }
}
