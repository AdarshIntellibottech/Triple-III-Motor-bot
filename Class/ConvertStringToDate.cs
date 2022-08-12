using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace III_ProjectOne.Class
{
    class ConvertStringToDate
    {
        public static string convertToDate(string input)
        {
            try
            {
                LogMessage.Log("Inside convertToDate, to parse string as date");
                return DateTime.FromOADate(double.Parse(input)).ToShortDateString();

            }
            catch (Exception ex)
            {
                LogMessage.Log("Error : convertToDate - " + ex.Message);
                return input;
            }
        }
    }
}
