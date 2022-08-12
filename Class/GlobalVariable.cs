using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;

namespace III_ProjectOne
{
    class GlobalVariable
    {
        //Variables to access all over the project
        public static CancellationToken cancellationToken;
        public static IWebDriver m_driver = null;

        //Log file name
        public static string  logFileName = @"C:\III_ProjectOne\Log\Log_" + DateTime.Now.ToString("dd-MM-yyyy").ToString() + ".txt";

        //Dictionary to save config and navigation sheet data
        public static Dictionary<string, string> configDict =new Dictionary<string,string>();
        public static Dictionary<string, string> navigationDict =new Dictionary<string,string>();
        public static Dictionary<string, string> mappingDict =new Dictionary<string,string>();
        public static Dictionary<string, string> countryDict =new Dictionary<string,string>();
        public static Dictionary<string, string> defaultValues =new Dictionary<string,string>();
        public static Dictionary<string, string> LoginNavigation =new Dictionary<string,string>();
        

        // Status flag
        public static bool errorStatus;

        //Datatable to store customer and claim data
        public static DataTable dtClaimData = new DataTable();
        public static DataTable dtCustomerAgentData = new DataTable();

        //Creating summary table
        public static DataTable dtSummaryTable = new DataTable();
        

        public static void AddDataToSummaryTabe(string name,string type,string remarks,string status,string optionselected)
        {
            DataRow dataRow = dtSummaryTable.NewRow();
            switch (optionselected)
            {
                case "Customer":
                    dataRow["Name"] = name;
                    dataRow["Type"] = type;
                    dataRow["Remarks"] = remarks;
                    dataRow["Status"] = status;
                    break;
                case "Claim":
                    dataRow["Policy Number"] = name;
                    dataRow["Claim Number"] = type;
                    dataRow["Remarks"] = remarks;
                    dataRow["Status"] = status;
                    break;
            }
            

            dtSummaryTable.Rows.Add(dataRow);
        }




    }
}
