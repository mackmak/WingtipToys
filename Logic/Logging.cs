using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WingtipToys.Logic
{
    public sealed class Logging
    {
        public static void LogException(Exception ex, string source)
        {
            var logFile = "~/App_Data/ErrorLog.txt";
            logFile = HttpContext.Current.Server.MapPath(logFile);

            //open file
            var streamWriter = new StreamWriter(logFile, true);
            streamWriter.WriteLine($"********** {DateTime.Now} **********");

            if(ex.InnerException != null)
            {
                streamWriter.WriteLine($"Inner Exception Type: {ex.InnerException.GetType().ToString()}");
                streamWriter.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                streamWriter.WriteLine($"Inner Source: {ex.InnerException.Source}");
                if (ex.InnerException.StackTrace != null)
                {
                    streamWriter.WriteLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
                }
            }
            streamWriter.WriteLine($"Exception Type: {ex.GetType().ToString()}");
            streamWriter.WriteLine($"Exception: {ex.Message}");
            streamWriter.WriteLine($"Source: {source}");
            
            if (ex.StackTrace != null)
            {
                streamWriter.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            streamWriter.Close();

        }

    }
}