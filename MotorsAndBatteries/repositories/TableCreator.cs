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
        public DataTable createTable(DataSet excelSheet)
        {

            DataTable newTable = new DataTable();
            newTable.Columns.Add("motor");
            newTable.Columns.Add("battery");
            newTable.Columns.Add("battery source");
            newTable.Columns.Add("parallel batteries");
            newTable.Columns.Add("hover mass (g)");
            newTable.Columns.Add("battery mass (g)");
            newTable.Columns.Add("motor mass (g)");
            newTable.Columns.Add("remaining payload (g)");
            newTable.Columns.Add("battery capacity");
            newTable.Columns.Add("number of cells");
            newTable.Columns.Add("battery energy surplus");
            newTable.Columns.Add("battery life surplus");

            foreach (DataRow motorRow in excelSheet.Tables["motors$"].Rows)
            {
                foreach (DataRow batteryRow in excelSheet.Tables["batteries$"].Rows)
                {

                    int cellsFactor = 2;
                    int energyFactor = 2;
                    
                    if ((double)batteryRow["number of cells"] == (double)motorRow["number of cells"])
                    {
                        while ( (double)motorRow["required energy, 13 minute hover (W*h)"] > ((double)batteryRow["theoretical energy (W*h)"]) *energyFactor) 
                        {
                            energyFactor += 1;
                        }
                        if ((double)motorRow["required energy, 13 minute hover (W*h)"] < ((double)batteryRow["theoretical energy (W*h)"]) * energyFactor && ((double)motorRow["hover mass (g)"] - (double)motorRow["mass of 4 motors (g)"] - energyFactor * 1000 * (double)batteryRow["mass (kg)"]) > 0 )
                        {


                            DataRow newRow = newTable.NewRow();

                            newRow["motor"] = motorRow["model"];
                            newRow["battery"] = batteryRow["model"];
                            newRow["battery source"] = batteryRow["source"];
                            newRow["parallel batteries"] = energyFactor;
                            newRow["hover mass (g)"] = motorRow["hover mass (g)"];
                            newRow["battery mass (g)"] = batteryRow["mass (kg)"];
                            newRow["motor mass (g)"] = motorRow["mass of 4 motors (g)"];
                            newRow["remaining payload (g)"] = (double)motorRow["hover mass (g)"] - (double)motorRow["mass of 4 motors (g)"] - energyFactor*1000*(double)batteryRow["mass (kg)"];
                            newRow["battery capacity"] = batteryRow["capacity (Ah)"];
                            newRow["number of cells"] = batteryRow["number of cells"];
                            newRow["battery energy surplus"] = energyFactor*(double)batteryRow["theoretical energy (W*h)"] - (double)motorRow["required energy, 13 minute hover (W*h)"] ;
                            newRow["battery life surplus"] = (energyFactor * (double)batteryRow["theoretical energy (W*h)"] - (double)motorRow["required energy, 13 minute hover (W*h)"])/((double)motorRow["Voltage (V)"]*(double)motorRow["current (A)"]);
                            newTable.Rows.Add(newRow);
                        }
                        //create parallel battery entry
                    }
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
            return newTable;
        }
    }
}
