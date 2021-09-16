using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KenzenAPI.Classes.Models
{
    public class TeamUser
    {
        public TeamUser() { }
        public int UserID { get; set; }
        public int TeamID { get; set; }
        public int ClientID { get; set; }
    }
}
