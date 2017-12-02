using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisServer
{
    class ServerMain
    {

        public static void Main(String[] args)
        {
            //Handle args
            ServerConfig cfg = new ServerConfig();

            // listening_port number_of_columns maximum_number_of_lines delay_speed
            cfg.port = Convert.ToInt32(args[0]);
            cfg.columns = Convert.ToInt32(args[1]);
            cfg.maxLines = Convert.ToInt32(args[2]);
            cfg.delay_time = Convert.ToInt32(args[3]);

            new TetrisServer(cfg); //the server actually starts here
            Console.ReadLine();

        }
    }
}
