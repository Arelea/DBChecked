using DBChecked.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DBChecked.ViewModels;
using Npgsql;

namespace DBChecked.Controllers
{
    public class HomeController : BaseDataController
    {
        public IActionResult Index()
        {
            var viewModel = this.GetViewModel<IndexViewModel>();
            var listOfConnections = new List<DBConnectionData>();

            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection("Server=10.200.0.17;Port=5433;User Id=laura;Password=2JlyKXxT7P;Database=service;"))
                {
                    conn.Open();
                    NpgsqlCommand command = new NpgsqlCommand("SELECT name, descr, host, port FROM dbases;", conn);
                    NpgsqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var name = reader["name"].ToString();
                        var descr = reader["descr"].ToString();
                        var port = reader["port"].ToString();
                        var host = reader["host"].ToString();

                        listOfConnections.Add(new DBConnectionData()
                        {
                            Name = name,
                            Host = host,
                            Port = port,
                            Descr = descr,
                        });
                    }
                    reader.Close();

                    command.Dispose();
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                viewModel.Error = e.Message;
            }

            viewModel.List = listOfConnections;

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
