using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MotorsAndBatteries.repositories
{
    interface ISheetMap
    {
        Dictionary<string, string> getSheetToTableMap(IConfiguration conf);
        Dictionary<string, string> getColumnToColumnMap(IConfiguration conf);


    }
}
