using DBChecked.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
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
            var completedConnections = new List<string>();

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

                        completedConnections.Add($"Server={host};Port={port};User Id=laura;Password=2JlyKXxT7P;Database={name};Timeout=30;");
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

            viewModel.CompletedConnections = completedConnections;
            viewModel.List = listOfConnections;

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SetSqlQuery(string connections)
        {
            var viewModel = this.GetViewModel<SetSqlQueryViewModel>();
            var parsedConnections = connections.Split(":").ToList();
            var list = new List<SelectListItem>();
            foreach (var parsedConnection in parsedConnections)
            {
                list.Add(new SelectListItem { Value = parsedConnection, Text = parsedConnection.Split(";").First(m => m.Contains("Database")).Remove(0, 9) });
            }

            viewModel.Form = this.CreateForm<SetSqlQueryForm>();
            viewModel.Form.ConnectionsString = connections == null || connections == "" ? TempData["connectionString"].ToString() : connections;
            viewModel.ConnectionList =  list;

            if (TempData["Result"] != null)
            {
                viewModel.Result = TempData["Result"] as List<dynamic>;
            }
            else if (TempData["errorMessage"] != null)
            {
                viewModel.ErrorMessage = TempData["errorMessage"].ToString();
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SetSqlQuery(SetSqlQueryForm form)
        {
            var dns = new List<dynamic>();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(form.Connection))
                {
                    conn.Open();
                    NpgsqlCommand command = new NpgsqlCommand(form.Query, conn);
                    NpgsqlDataReader reader = command.ExecuteReader();

                    var dataTable = new DataTable();
                    dataTable.Load(reader);
                    foreach (var item in dataTable.AsEnumerable())
                    {
                        IDictionary<string, object> dn = new ExpandoObject();

                        foreach (var column in dataTable.Columns.Cast<DataColumn>())
                        {
                            dn[column.ColumnName] = item[column];
                        }

                        dns.Add(dn);
                    }

                    reader.Close();

                    command.Dispose();
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                TempData["errorMessage"] = e.Message;
                TempData["connectionString"] = form.ConnectionsString;

                return RedirectToAction("SetSqlQuery", "Home");
            }

            TempData["connectionString"] = form.ConnectionsString;
            TempData["Result"] = dns;

            return RedirectToAction("SetSqlQuery", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
