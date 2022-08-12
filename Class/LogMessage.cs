using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace III_ProjectOne
{
    class LogMessage
    {
        
        public static void Log(string Message)
        {
            string[] start = { DateTime.Now + ": "+Message };
            File.AppendAllLines(GlobalVariable.logFileName, start);

        }
        
    }
}
