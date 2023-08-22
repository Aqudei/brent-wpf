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
        private readonly Dispatcher _dispatcher;
        private readonly IDialogCoordinator _dialogCoordinator;

        public MainViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dispatcher = Application.Current.Dispatcher;
            _dialogCoordinator = dialogCoordinator;
        }

        private DelegateCommand _loadMasterCsvCommand;
        private DataView _items;
        private DelegateCommand _loadDataCsvCommand;

        public DelegateCommand LoadMasterCsvCommand
        {
            get { return _loadMasterCsvCommand ??= new DelegateCommand(HandleLoadMasterCsv); }
        }

        public DelegateCommand LoadDataCsvCommand { get => _loadDataCsvCommand ??= new DelegateCommand(HandleLoadCsvData); }

        private void HandleLoadCsvData()
        {

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
            var csvData = DataUtils.ReadCsvGrid(fileName,
                Properties.Settings.Default.ROW_START_MARKER,
                Properties.Settings.Default.COLUMN_START_MARKER,
                Properties.Settings.Default.COLUMN_END_MARKER);


            var dataTable = DataUtils.ToDataTable(csvData);

            await _dispatcher.InvokeAsync(() =>
            {
                Items = dataTable.AsDataView();
            });
        }
    }
}
