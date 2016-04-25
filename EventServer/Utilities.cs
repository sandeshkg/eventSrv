using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace EventServer
{
    public class DataTableProcesses
    {
        public string GetThisColumnValue(string columnName, DataRow row)
        {
            return (row.Table.Columns.Contains(columnName)) ? row[columnName].ToString() : string.Empty;
        }


    }
}