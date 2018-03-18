using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace synology_test.Models
{
    public class SynologyConfig
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string BaseHost { get; set; }
        public int Port { get; set; }
        public bool Ssl { get; set; }
        public int SslPort { get; set; }
    }
}
