using DBChecked.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DBChecked.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
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

                if (listOfConnections.Any())
                {

                    foreach (var connection in listOfConnections)
                    {
                        try
                        {
                            using (NpgsqlConnection conn = new NpgsqlConnection($"Server={connection.Host};Port={connection.Port};User Id=laura;Password=2JlyKXxT7P;Database={connection.Name};Timeout=1;"))
                            {
                                conn.Open();
                                NpgsqlCommand command = new NpgsqlCommand("SELECT 12;", conn);
                                NpgsqlDataReader reader = command.ExecuteReader();

                                while (reader.Read())
                                {
                                    var val = (int)reader[0];
                                    if (val == 12)
                                    {
                                        connection.Status = "Работает";
                                    }
                                }
                                reader.Close();

                                command.Dispose();
                                conn.Close();
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }
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

        [HttpGet]
        public IActionResult SetSqlQuery(List<DBConnectionData> connections)
        {
            var viewModel = this.GetViewModel<SetSqlQueryViewModel>();

            var list = new List<SelectListItem>();
            foreach (var item in connections)
            {
                list.Add(new SelectListItem { Value = $"Server={item.Host};Port={item.Port};User Id=laura;Password=2JlyKXxT7P;Database={item.Name};Timeout=30;", Text = item.Name });
            }

            viewModel.ConnectionList =  list;

            return View();
        }

        [HttpPost]
        public IActionResult SetSqlQuery(SetSqlQueryForm form)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(form.Connection))
                {
                    conn.Open();
                    NpgsqlCommand command = new NpgsqlCommand(form.Query, conn);
                    NpgsqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                    }
                    reader.Close();

                    command.Dispose();
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                return RedirectToAction("SetSqlQuery", "Home");
            }

            return RedirectToAction("SetSqlQuery", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
