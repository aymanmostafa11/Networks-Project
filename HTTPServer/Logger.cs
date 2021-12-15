using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(Exception ex)
        {
            // TODO: [DONE] Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 
            DateTime datetime = DateTime.Now;
            String message = ex.Message;
            String log = "Time : " + datetime.ToString() + "\n" + "Exception : " + message + "\n/////////\n";
            sr.WriteLine(log);
            sr.Close();
        }
    }
}
