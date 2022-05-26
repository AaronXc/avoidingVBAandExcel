using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace MotorsAndBatteries.repositories
{
    public class SheetMap : ISheetMap
    {


        public Dictionary<string, string> getSheetToTableMap(IConfiguration conf)
        {
            return conf.GetValue<Dictionary<string, string>>("SheetToTableMap");
        }
        public  Dictionary<string, string> getColumnToColumnMap(IConfiguration conf)
        {
            return conf.GetValue<Dictionary<string, string>>("ColumnToColumnMap");
        }
    }
}
