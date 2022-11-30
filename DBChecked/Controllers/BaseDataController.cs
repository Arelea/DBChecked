using System;
using DBChecked.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DBChecked.Controllers
{
    public class BaseDataController : Controller
    {
        protected T1 GetViewModel<T1>()
            where T1 : BaseViewModel, new()
        {
            var result = new T1();

            return result;
        }

        protected T1 CreateForm<T1>()
            where T1 : BaseForm, new()
        {
            var result = new T1();

            return result;
        }
    }
}
