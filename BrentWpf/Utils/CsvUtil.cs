using CsvHelper.Configuration;
using CsvHelper;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm.Native;
using ImTools;

namespace BrentWpf.Utils
{
    internal class CsvUtil
    {
        private readonly string _lastColumnHeader;

        public CsvUtil(string lastColumn)
        {
            _lastColumnHeader = lastColumn;
        }

        private IEnumerable<string[]> ReadCsvGrid(string fileName)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };

            using (var reader = new StreamReader(fileName))
            using (var csv = new CsvReader(reader, config))
            {
                while (csv.Read())
                {
                    var row = new List<string>();
                    for (int i = 0; i < csv.Parser.Count; i++)
                    {
                        row.Add(csv.GetField<string>(i));
                    }

                    yield return row.ToArray();
                }
            }
        }

        private IEnumerable<string> ReadCsvMain(List<string[]> csvGrid)
        {
            var lastColIndex = 0;
            for (int row = 0; row < csvGrid.Count(); row++)
            {
                if (row == 0)
                {
                    lastColIndex = csvGrid[row].IndexOf(_lastColumnHeader);
                }

                for (int i = 0; i < lastColIndex; i++)
                {

                }
            }
        }

        private IEnumerable<string> ReadCsvExtra(IEnumerable<string[]> csvGrid)
        {
            for (int i = 0; i < csvGrid.Count(); i++)
            {

            }
        }
    }
}
