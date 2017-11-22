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

            new TetrisServer(cfg);

          
            Console.ReadLine();

        }
    }
}
