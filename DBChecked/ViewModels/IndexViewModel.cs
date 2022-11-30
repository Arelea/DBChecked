using System;
using System.Collections.Generic;
using DBChecked.Models;

namespace DBChecked.ViewModels
{
    public class IndexViewModel : BaseViewModel
    {
        public List<DBConnectionData> List { get; set; }

        public List<string> CompletedConnections { get; set; }

        public string Error { get; set; }
    }
}
