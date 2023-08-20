using BrentWpf.Models;
using ControlzEx.Standard;
using CsvHelper;
using CsvHelper.Configuration;
using DevExpress.Mvvm.Native;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public ICollectionView Items { get => items; set => SetProperty(ref items, value); }
        private ObservableCollection<ItemModel> _items = new();

        public MainViewModel()
        {
            _dispatcher = Application.Current.Dispatcher;
            Items = CollectionViewSource.GetDefaultView(_items);
        }

        private DelegateCommand _loadCsvCommand;
        private ICollectionView items;

        public DelegateCommand LoadCsvCommand
        {
            get { return _loadCsvCommand ??= new DelegateCommand(HandleLoadCsv); }
        }

        private async void HandleLoadCsv()
        {
            var dlg = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog
            {
                Multiselect = false,
                Filters = { new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter("Comma Separated Values", ".csv;*.CSV") }
            };

            var rslt = dlg.ShowDialog();
            if (rslt == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
            {
                await _dispatcher.InvokeAsync(_items.Clear);

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    IgnoreBlankLines = true,
                };
                var items = ReadCsv(dlg.FileName);

                var csvData = string.Join(Environment.NewLine, items);
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvData)))
                using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Context.RegisterClassMap<ItemModelMap>();

                    var itemModels = csv.GetRecords<ItemModel>();
                    foreach (var itemModel in itemModels)
                    {
                        await _dispatcher.InvokeAsync(new Action(() =>
                        {
                            _items.Add(itemModel);
                        }));
                    }
                }
            }
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
