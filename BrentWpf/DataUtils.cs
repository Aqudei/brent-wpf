using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrentWpf
{
    internal class DataUtils
    {
        public static string ToCsvText(IEnumerable<List<string>> rows)
        {
            var sb = new StringBuilder();

            foreach (var row in rows)
            {
                sb.AppendLine(string.Join(",", row.Select(r => $"\"{r}\"")));
            }

            return sb.ToString();
        }

        public static IEnumerable<List<string>> ReadCsvGrid(string fileName, int rowStartMarker, int columnStartMarker, int columndEndMarker)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };

            using (var reader = new StreamReader(fileName))
            using (var csv = new CsvReader(reader, config))
            {
                var rowCounter = 0;

                while (csv.Read())
                {
                    if (rowCounter < rowStartMarker)
                    {
                        rowCounter++;
                        continue;
                    }

                    var row = new List<string>();
                    for (int i = columnStartMarker; i < columndEndMarker; i++)
                    {
                        row.Add(csv.GetField<string>(i));
                    }

                    if (row.All(string.IsNullOrWhiteSpace))
                    {
                        rowCounter++;
                        continue;
                    }

                    rowCounter++;
                    yield return row;
                }
            }
        }


        public static DataTable ToDataTable(IEnumerable<List<string>> rows)
        {
            // Create a DataTable
            var dt = new DataTable();

            // Get the column names from the first row
            var columnNames = rows.First();

            // Add columns to the DataTable
            foreach (string columnName in columnNames)
            {
                dt.Columns.Add(columnName, typeof(string));
            }

            // Iterate through the IEnumerable of Lists of strings and add each row to the DataTable
            foreach (var row in rows.Skip(1))
            {
                var dr = dt.Rows.Add();
                for (int i = 0; i < row.Count; i++)
                {
                    dr[i] = row[i];
                }
            }

            return dt;
        }
    }
}
