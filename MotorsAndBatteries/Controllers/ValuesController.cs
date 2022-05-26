using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using MotorsAndBatteries.data;
using MotorsAndBatteries.repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MotorsAndBatteries.models;
using System.Text;

namespace MotorsAndBatteries.controllers
{

    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class ValuesController : Controller
    {

        private ILogger<ValuesController> _logger;

        private readonly IConfiguration configuration;
        private readonly ISheetMap sheetMap;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult respond()
        {
            //var result = new SingleBattery();
            //result.battery_model = 123;
            //result.battery_total_mass = 385.2F;
            //return Ok(JsonConvert.SerializeObject(result));

            return new JsonResult("ok");
        }

        [HttpGet]
        public IActionResult calculate_batteries()
        {
            return new JsonResult("ok");
        }


        public void migrateData()
        {
            // access excel sheet and put data in sql server
        }

        [HttpPost]
        //[Route("[controller]/[action]/{filepath}")]
        //[HttpPost]
        //public async Task<IActionResult> ImportExcelFile(IFormFile FormFile)
        //public IActionResult ImportExcelFile(string filePath)
        //public string ImportExcelFile(excelPostPathClass fP)
        public async Task<IActionResult> ImportExcelFile()
        {
            //excelPostPathClass excelPath = JsonConvert.DeserializeObject<excelPostPathClass>(fP);
            string filePath = "";
            excelPostPathClass postedExcelPath = new excelPostPathClass();
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                Task<string> body = reader.ReadToEndAsync();

                await body;

                if(body.Status == TaskStatus.RanToCompletion)
                {
                    var jsonResp = body.ContinueWith(task => { return getExcelData(body.Result); });
                    await jsonResp;
                    return jsonResp.Result;
                }
                else
                {
                    return new JsonResult("ok");
                }
            }
 
        }

        public  IActionResult getExcelData(string body)
        {
            string filePath = "";
            excelPostPathClass postedExcelPath = new excelPostPathClass();
            postedExcelPath = JsonConvert.DeserializeObject<excelPostPathClass>(body);
            filePath = postedExcelPath.pathToData;

            #region fileupload
            ////get file name
            //var filename = ContentDispositionHeaderValue.Parse(FormFile.ContentDisposition).FileName.Substring(0, FormFile.ContentDisposition.Length);

            ////get path
            //var MainPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

            ////create directory "Uploads" if it doesn't exists
            //if (!Directory.Exists(MainPath))
            //{
            //    Directory.CreateDirectory(MainPath);
            //}

            ////get file path 
            //var filePath = Path.Combine(MainPath, FormFile.FileName);
            //using (System.IO.Stream stream = new FileStream(filePath, FileMode.Create))
            //{
            //    await FormFile.CopyToAsync(stream);
            //}

            ////get extension
            //string extension = Path.GetExtension(filename);
            #endregion
            string extension = Path.GetExtension(filePath);

            string conString = string.Empty;

            switch (extension)
            {
                case ".xls": //Excel 97-03.
                    conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                    break;
                case ".xlsx": //Excel 07 and above.
                    conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;HDR=YES'";
                    break;
            }

            Dictionary<string, DataTable> dtd = new Dictionary<string, DataTable>();
            //Dictionary<string, DataColumnCollection> dtd = new Dictionary<string, DataColumnCollection>();
            conString = string.Format(conString, filePath);

            using (OleDbConnection connExcel = new OleDbConnection(conString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        cmdExcel.Connection = connExcel;

                        //Get the name of First Sheet.
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        String[] excelSheets = new String[dtExcelSchema.Rows.Count];
                        int i = 0;

                        // Add the sheet name to the string array.
                        foreach (DataRow row in dtExcelSchema.Rows)
                        {
                            excelSheets[i] = row["TABLE_NAME"].ToString();
                            i++;
                        }
                        connExcel.Close();
                        // Loop through all of the sheets if you want too...
                        for (int j = 0; j < excelSheets.Length; j++)
                        {
                            var dt = new DataTable();
                            // Query each excel sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + excelSheets[j] + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();

                            dtd.Add(excelSheets[j], dt);
                        }
                        


                    }
                }
            }

            //using (ComponentContext ctx = new ComponentContext(configuration))
            //{
            //    foreach (string sheet in dtd.Keys)
            //    {
            //        string table = sheetMap.getSheetToTableMap(configuration)[sheet];
            //
            //        switch (table)
            //        {
            //            case ("Motors"):
            //
            //                break;
            //            case ("Batteries"):
            //                break;
            //            case ("ParallelBatteries"):
            //                break;
            //            case ("SeriesBatteries"):
            //                break;
            //            case ("SingleBattery"):
            //                break;
            //            case ("SeriesBatteriesPE"):
            //                break;
            //            case ("ParallelBatteriesPE"):
            //                break;
            //
            //        }
            //    }
            //}
            //
            // use the data in the data table to instantiate classes that are in the models folder

            // use these class instances to update the db

            //JsonConvert.SerializeObject(dtd);

            return new JsonResult(dtd);
        }

    }

}
