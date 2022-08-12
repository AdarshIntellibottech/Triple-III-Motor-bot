using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace III_ProjectOne
{
    class ConvertDtToDict
    {
        public static Dictionary<String,String> ConvertToDictionary(DataTable dataTabe)
        {
            Dictionary<String, String> outDict = new Dictionary<string, string>();
            try
            {

                if (dataTabe.Rows.Count <= 0)
                {
                    LogMessage.Log("Row count is 0, Unable to convert datatable to dictionay.");
                    return outDict;
                }

                foreach (DataRow row in dataTabe.Rows)
                {

                    GlobalVariable.cancellationToken.ThrowIfCancellationRequested();

                    if ((Convert.ToString(row["Key"]) == "")||(Convert.ToString(row["Value"]) == ""))
                    {
                        GlobalVariable.errorStatus = true;
                        LogMessage.Log("Row data is empty for "+ Convert.ToString(row["Key"]) + ", skipping...");
                    }
                    else
                    {
                        outDict[row["Key"].ToString().Trim()] = row["Value"].ToString().Trim();
                    }
                    
                    
                }
                LogMessage.Log("Converting Datatble to Dictionary is successful...");
            }
            catch(Exception ex)
            {
                GlobalVariable.errorStatus = true;
                LogMessage.Log("Error : "+ex.Message);
                LogMessage.Log("Error : "+ex.StackTrace);
            }

            return outDict;

        }
    }
}
