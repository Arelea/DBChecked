using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBChecked.Models
{
    public sealed class DBConnectionData
    {
        public string Name { get; set; }

        public string Descr { get; set; }

        public string Host { get; set; }

        public string Port { get; set; }

        public string Status { get; set; }
    }
}
