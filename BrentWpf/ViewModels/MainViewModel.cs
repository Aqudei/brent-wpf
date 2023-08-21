using BrentWpf.Models;
using ControlzEx.Standard;
using CsvHelper;
using CsvHelper.Configuration;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace BrentWpf.ViewModels
{
    internal class MainViewModel : BindableBase
    {
        private string _lastColumn = "Colour Avg";
        public string LastColumn { get => _lastColumn; set => SetProperty(ref _lastColumn, value); }
        private readonly Dispatcher _dispatcher;
        private readonly IDialogCoordinator _dialogCoordinator;

        public MainViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dispatcher = Application.Current.Dispatcher;
            _dialogCoordinator = dialogCoordinator;
        }

        private DelegateCommand _loadCsvCommand;
        private DataView _items;

        public DelegateCommand LoadCsvCommand
        {
            get { return _loadCsvCommand ??= new DelegateCommand(HandleLoadMasterCsv); }
        }

        public DataView Items { get => _items; set => SetProperty(ref _items, value); }

        private async void HandleLoadMasterCsv()
        {
            var dlg = new CommonOpenFileDialog
            {
                Multiselect = false,
                Filters = { new CommonFileDialogFilter("Comma Separated Values", ".csv;*.CSV") }
            };

            var rslt = dlg.ShowDialog();
            if (rslt == CommonFileDialogResult.Ok)
            {
                var progress = await _dialogCoordinator.ShowProgressAsync(this, "Please wait", "Processing Csv file");

                try
                {
                    progress.SetIndeterminate();

                    await ProcessCsvAsync(dlg.FileName);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"{e.Message}\n\n{e.StackTrace}");
                }
                finally
                {
                    await progress.CloseAsync();
                }
            }
        }

        private async Task ProcessCsvAsync(string fileName)
        {
            var rows = ReadCsvGrid(fileName, "ERP ID", LastColumn);
            var dataTable = ToDataTable(rows);

            await _dispatcher.InvokeAsync(() =>
            {
                Items = dataTable.AsDataView();
            });



            //var csvText = ToCsvText(rows);

            //var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            //{

            //};

            //using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvText)))
            //using (var reader = new StreamReader(stream))
            //using (var csv = new CsvHelper.CsvReader(reader, config))
            //{
            //    csv.Context.RegisterClassMap<ItemModelMap>();

            //    foreach (var item in csv.GetRecords<ItemModel>())
            //    {
            //        await _dispatcher.InvokeAsync(() =>
            //        {
            //            _items.Add(item);
            //        });
            //    }
            //}
        }

        public static DataTable ToDataTable(IEnumerable<List<string>> rows)
        {
            // Create a DataTable
            DataTable dt = new DataTable();

            // Get the column names from the first row
            List<string> columnNames = rows.First();

            // Add columns to the DataTable
            foreach (string columnName in columnNames)
            {
                dt.Columns.Add(columnName, typeof(string));
            }

            // Iterate through the IEnumerable of Lists of strings and add each row to the DataTable
            foreach (List<string> row in rows.Skip(1))
            {
                DataRow dr = dt.Rows.Add();
                for (int i = 0; i < row.Count; i++)
                {
                    dr[i] = row[i];
                }
            }

            return dt;
        }

        private IEnumerable<List<string>> ReadCsvGrid(string fileName, string startMarker, string endMarker)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };
            var rowCount = 0;
            var lastIndex = 0;

            using (var reader = new StreamReader(fileName))
            using (var csv = new CsvReader(reader, config))
            {
                var markerFound = false;

                while (csv.Read())
                {
                    if (!markerFound)
                    {
                        markerFound = csv.GetField(0) == startMarker;
                        if (!markerFound)
                            continue;

                    }

                    var row = new List<string>();
                    for (int i = 0; i < csv.Parser.Count; i++)
                    {
                        row.Add(csv.GetField<string>(i));
                    }

                    if (row.All(string.IsNullOrWhiteSpace))
                    {
                        continue;
                    }

                    if (rowCount == 0)
                    {
                        lastIndex = row.IndexOf(endMarker);
                    }

                    rowCount++;

                    yield return row.GetRange(0, lastIndex + 1);
                }
            }
        }

        private string ToCsvText(IEnumerable<List<string>> rows)
        {
            var sb = new StringBuilder();

            foreach (var row in rows)
            {
                sb.AppendLine(string.Join(",", row.Select(r => $"\"{r}\"")));
            }

            return sb.ToString();
        }

        private IEnumerable<string> ReadCsv(string fileName)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };

            using (var reader = new StreamReader(fileName))
            using (var csv = new CsvReader(reader, config))
            {
                var header = new List<string>();

                while (csv.Read())
                {
                    var row = new List<string>();
                    for (int i = 0; i < csv.Parser.Count; i++)
                    {
                        row.Add(csv.GetField<string>(i));
                    }

                    if (!header.Any())
                    {
                        if (!string.IsNullOrWhiteSpace(row.FirstOrDefault()))
                        {
                            var extra_idx = row.IndexOf(LastColumn);

                            for (int i = 0; i < extra_idx; i++)
                            {
                                header.Add($"\"{row[i]}\"");
                            }

                            yield return string.Join(",", header);

                        }
                        continue;
                    }

                    var data = new List<string>();
                    for (var i = 0; i < header.Count; i++)
                    {
                        data.Add($"\"{row[i]}\"");
                    }

                    yield return string.Join(",", data);
                }
            }
        }
    }
}
