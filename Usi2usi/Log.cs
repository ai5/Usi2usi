using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Usi2usi
{
    public static class Log
    {
        private static TextWriter fs;
        private static object syncObj;

        public static void Open(bool log)
        {
            syncObj = new object();

            if (log)
            {
                try
                {
                    fs = new StreamWriter("usi2usi.txt", false);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            }
        }

        public static void WriteLine(string str)
        {
            lock (syncObj)
            {
                if (fs != null)
                {
                    fs.WriteLine(str);
                }
            }
        }

        public static void Close()
        {
            lock (syncObj)
            {
                if (fs != null)
                {
                    fs.Close();

                    fs = null;
                }
            }
        }
    }
}
