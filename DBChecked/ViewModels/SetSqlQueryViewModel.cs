using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DBChecked.ViewModels
{
    public sealed class SetSqlQueryViewModel :BaseViewModel
    {
        public List<SelectListItem> ConnectionList { get; set; }

        public SetSqlQueryForm Form { get; set; }

        public string ErrorMessage { get; set; }

        public List<dynamic> Result { get; set; }
    }

    public sealed class SetSqlQueryForm : BaseForm
    {
        public string Query { get; set; }

        public string Connection { get; set; }

        public string ConnectionsString { get; set; }
}
}
