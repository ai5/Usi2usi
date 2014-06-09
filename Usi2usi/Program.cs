using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Usi2usi
{
    class Program
    {
        static void Main(string[] args)
        {
            UsiEngine engine;
            bool log_enable = false;

            string path = ConfigurationManager.AppSettings["EngineName"];
            if (path == null)
            {
                Console.Error.WriteLine("EngineNameが無い?");
                return;
            }

            string log = ConfigurationManager.AppSettings["Log"];
            if (log != null && log == "true")
            {
                log_enable = true;
            }

            Log.Open(log_enable);
            engine = new UsiEngine();

            try
            {
                engine.Connect(path);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }


            while (true)
            {
                string cmd = Console.ReadLine();

                if (cmd == null)
                {
                    cmd = "quit";
                }
                else
                {
                    Log.WriteLine(cmd);
                }

                if (cmd == "quit")
                {
                    engine.Disconnect();
                    break;
                }
                else
                {
                    engine.Send(cmd);
                }
            }

            Log.Close();
        }
    }
}
