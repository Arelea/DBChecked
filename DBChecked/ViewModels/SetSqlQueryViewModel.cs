using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DBChecked.ViewModels
{
    public sealed class SetSqlQueryViewModel :BaseViewModel
    {
        public List<SelectListItem> ConnectionList { get; set; }

        public SetSqlQueryForm Form { get; set; }
    }

    public sealed class SetSqlQueryForm
    {
        public string Query { get; set; }

        public string Connection { get; set; }
    }
}
