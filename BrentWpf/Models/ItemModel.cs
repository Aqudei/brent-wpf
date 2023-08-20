using CsvHelper.Configuration;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Xpf.Core.Native;
using DryIoc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//ERP ID : 2204
//Serial No : K55200041
//Model : OKI.ES9455MFD
//Contract In : 7 / 07 / 2016
//Contract Out : 
//Date Last Billed : 7 / 07 / 2023
//Next Bill Date : 8 / 07 / 2023
//Billing Frequency : 1
//Rental Count : 
//Machine Rental($) : 0
//Request By : GenericCSV
//Customer Name : SA Teen Solicitors
//Location : SA Teen
//Mono CPP ($) : 0.0154
//Colour CPP($) : 0.11
//Current Read Mono : 198621
//Current Read Colour : 20155
//Mono Avg : 1857.4285


namespace BrentWpf.Models
{
    internal class ItemModelMap : ClassMap<ItemModel>
    {
        public ItemModelMap()
        {
            Map(m => m.ERPId).Name("ERP ID");
            Map(m => m.SerialNumber).Name("Serial No");
            Map(m => m.Model).Name("Model");
            Map(m => m.ContractIn).Name("Contract In");
            Map(m => m.ContractOut).Name("Contract Out");
            Map(m => m.DateLastBilled).Name("Date Last Billed");
            Map(m => m.NextBillDate).Name("Next Bill Date");
            Map(m => m.BillingFrequency).Name("Billing Frequency");
            Map(m => m.MachineRentalAmount).Name("Machine Rental ($)");
            Map(m => m.RequestBy).Name("Request By");
            Map(m => m.CustomerName).Name("Customer Name");
            Map(m => m.Location).Name("Location");
            Map(m => m.MonoCPPAmount).Name("Mono CPP ($)");
            Map(m => m.ColorCPPAmount).Name("Colour CPP ($)");
            Map(m => m.CurrentReadMono).Name("Current Read Mono");
            Map(m => m.CurrentReadColour).Name("Current Read Colour");
            Map(m => m.MonoAverage).Name("Mono Avg");
        }
    }
    internal class ItemModel : BindableBase
    {
        private bool _isSelected;

        public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }
        public string ERPId { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public string ContractIn { get; set; }
        public string ContractOut { get; set; }
        public string DateLastBilled { get; set; }
        public string NextBillDate { get; set; }
        public string BillingFrequency { get; set; }
        public string RentalCount { get; set; }
        public string MachineRentalAmount { get; set; }
        public string RequestBy { get; set; }
        public string CustomerName { get; set; }
        public string Location { get; set; }
        public string MonoCPPAmount { get; set; }
        public string ColorCPPAmount { get; set; }
        public string CurrentReadMono { get; set; }
        public string CurrentReadColour { get; set; }
        public string MonoAverage { get; set; }

        public override string ToString()
        {
            return $"{ERPId},{SerialNumber},{Model}";
        }
    }
}
