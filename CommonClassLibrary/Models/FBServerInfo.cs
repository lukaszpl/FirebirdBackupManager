using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClassLibrary.Models
{
    public class FBServerInfo
    {
        public int id { get; set; }
        public FBServerInfo(int id, string? name, string? serverIP, int? serverPort, string? user, string? password)
        {
            this.id = id;
            this.name = name;
            this.serverIP = serverIP;
            this.serverPort = serverPort;
            this.user = user;
            this.password = password;
        }

        public string? name { get; set; }
        public string? serverIP { get; set; }
        public int? serverPort { get; set; }
        public string? user {  get; set; }
        public string? password { get; set; }
    }
}
