using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;

namespace MotorsAndBatteries.repositories
{
    public class TableCreator
    {
        
        // Logic:
        //
        // select batteries that have the same number of cells as the motor required voltage
        // select number of batteries that has enough stored energy
        // calculate battery life based on Ah and motor current draw
        // 

        private readonly DataTable newSheet;
        public static Dictionary<string, DataTable> createTable(DataSet excelSheet)
        {

            DataTable seriesSchema = new DataTable();
            seriesSchema.Columns.Add("motor");
            seriesSchema.Columns.Add("battery");
            seriesSchema.Columns.Add("battery source");
            seriesSchema.Columns.Add("series batteries");
            seriesSchema.Columns.Add("hover mass (g)");
            seriesSchema.Columns.Add("battery mass (g)");
            seriesSchema.Columns.Add("motor mass (g)");
            seriesSchema.Columns.Add("remaining payload (g)");
            seriesSchema.Columns.Add("battery capacity");
            seriesSchema.Columns.Add("number of cells");
            seriesSchema.Columns.Add("battery energy surplus");
            seriesSchema.Columns.Add("battery life surplus");

            DataTable parallelSchema = new DataTable();
            parallelSchema.Columns.Add("motor");
            parallelSchema.Columns.Add("battery");
            parallelSchema.Columns.Add("battery source");
            parallelSchema.Columns.Add("parallel batteries");
            parallelSchema.Columns.Add("hover mass (g)");
            parallelSchema.Columns.Add("battery mass (g)");
            parallelSchema.Columns.Add("motor mass (g)");
            parallelSchema.Columns.Add("remaining payload (g)");
            parallelSchema.Columns.Add("battery capacity");
            parallelSchema.Columns.Add("number of cells");
            parallelSchema.Columns.Add("battery energy surplus");
            parallelSchema.Columns.Add("battery life surplus");


            foreach (DataRow motorRow in excelSheet.Tables["motors$"].Rows)
            {
                foreach (DataRow batteryRow in excelSheet.Tables["batteries$"].Rows)
                {



                    addSeriesBatteries(excelSheet, seriesSchema, motorRow, batteryRow);
                    addParallelBatteries(excelSheet, parallelSchema, motorRow, batteryRow);
                    //else
                    //{
                    //    while ((double)motorRow["number of cells"] > (double)batteryRow["number of cells"]*cellsFactor)
                    //    {
                    //        cellsFactor+=1;
                    //    }
                    //    if ((double)batteryRow["number of cells"]* cellsFactor == (double)motorRow["number of cells"] && (double)motorRow["required energy, 13 minute hover (W*h)"] < ((double)batteryRow["theoretical energy (W*h)"])*energyFactor)
                    //    {
                    //        //create series battery entry
                    //    }
                    //}

                }
            }
            return new Dictionary<string, DataTable>() { { "series_batteries$", seriesSchema},{"parallel_batteries$", parallelSchema } };
        }

        public static void addParallelBatteries(DataSet ExcelSheet, DataTable parallelSchema, DataRow motorRow, DataRow batteryRow)
        {            
            int energyFactor = 2;
            DataRow newRow = parallelSchema.NewRow();

            if ((double)batteryRow["number of cells"] == (double)motorRow["number of cells"])
            {
                while ((double)motorRow["required energy, 13 minute hover (W*h)"] > ((double)batteryRow["theoretical energy (W*h)"]) * energyFactor)
                {
                    energyFactor += 1;
                }
                if ((double)motorRow["required energy, 13 minute hover (W*h)"] < ((double)batteryRow["theoretical energy (W*h)"]) * energyFactor && ((double)motorRow["hover mass (g)"] - (double)motorRow["mass of 4 motors (g)"] - energyFactor * 1000 * (double)batteryRow["mass (kg)"]) > 0)
                {
                    newRow["motor"] = motorRow["model"];
                    newRow["battery"] = batteryRow["model"];
                    newRow["battery source"] = batteryRow["source"];
                    newRow["parallel batteries"] = energyFactor;
                    newRow["hover mass (g)"] = motorRow["hover mass (g)"];
                    newRow["battery mass (g)"] = batteryRow["mass (kg)"];
                    newRow["motor mass (g)"] = motorRow["mass of 4 motors (g)"];
                    newRow["remaining payload (g)"] = (double)motorRow["hover mass (g)"] - (double)motorRow["mass of 4 motors (g)"] - energyFactor * 1000 * (double)batteryRow["mass (kg)"];
                    newRow["battery capacity"] = batteryRow["capacity (Ah)"];
                    newRow["number of cells"] = batteryRow["number of cells"];
                    newRow["battery energy surplus"] = energyFactor * (double)batteryRow["theoretical energy (W*h)"] - (double)motorRow["required energy, 13 minute hover (W*h)"];
                    newRow["battery life surplus"] = (energyFactor * (double)batteryRow["theoretical energy (W*h)"] - (double)motorRow["required energy, 13 minute hover (W*h)"]) / ((double)motorRow["Voltage (V)"] * (double)motorRow["current (A)"]);
                    parallelSchema.Rows.Add(newRow);
                }
                //create parallel battery entry
            }

        }

        public static void addSeriesBatteries(DataSet ExcelSheet, DataTable seriesSchema, DataRow motorRow, DataRow batteryRow)
        {


            int cellsFactor = 2;
            
            DataRow newRow = seriesSchema.NewRow();
            double cellsNumber = (double)batteryRow["number of cells"];

            while( cellsNumber < (double)motorRow["number of cells"])
            {
                cellsNumber = cellsFactor*cellsNumber;
                cellsFactor += 1;
            }
            if ((double)motorRow["required energy, 13 minute hover (W*h)"] > ((double)batteryRow["theoretical energy (W*h)"]) * cellsFactor && 
                (double)batteryRow["capacity (Ah)"]/ ((double)motorRow["Current (A)"] * 4) > 0.216666666666666667)
            {
                    
                
                if (((double)motorRow["hover mass (g)"] - (double)motorRow["mass of 4 motors (g)"] - cellsFactor * 1000 * (double)batteryRow["mass (kg)"]) > 0)
                {




                    newRow["motor"] = motorRow["model"];
                    newRow["battery"] = batteryRow["model"];
                    newRow["battery source"] = batteryRow["source"];
                    newRow["parallel batteries"] = cellsFactor;
                    newRow["hover mass (g)"] = motorRow["hover mass (g)"];
                    newRow["battery mass (g)"] = batteryRow["mass (kg)"];
                    newRow["motor mass (g)"] = motorRow["mass of 4 motors (g)"];
                    newRow["remaining payload (g)"] = (double)motorRow["hover mass (g)"] - (double)motorRow["mass of 4 motors (g)"] - cellsFactor * 1000 * (double)batteryRow["mass (kg)"];
                    newRow["battery capacity"] = batteryRow["capacity (Ah)"];
                    newRow["number of cells"] = batteryRow["number of cells"];
                    newRow["battery energy surplus"] = cellsFactor * (double)batteryRow["theoretical energy (W*h)"] - (double)motorRow["required energy, 13 minute hover (W*h)"];
                    newRow["battery life surplus"] = (cellsFactor * (double)batteryRow["theoretical energy (W*h)"] - (double)motorRow["required energy, 13 minute hover (W*h)"]) / ((double)motorRow["Voltage (V)"] * (double)motorRow["current (A)"]);
                    seriesSchema.Rows.Add(newRow);
                }
                //create parallel battery entry
            }

        }
    }
}
