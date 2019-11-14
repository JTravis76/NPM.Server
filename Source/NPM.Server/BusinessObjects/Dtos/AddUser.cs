using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPM.BusinessObjects.Dtos
{
    public class AddUser
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string type { get; set; }
        public string[] roles { get; set; }
        public DateTime date { get; set; }
    }
}
