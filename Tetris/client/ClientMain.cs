using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tetris.client.graphics;


namespace Tetris.client
{
    class ClientMain
    {
        public static void Main(String[] args)
        {

            ClientConfig cfg = new ClientConfig();
            //Handle args

            TetrisClient client = new TetrisClient(cfg);
            client.Connect();
            
            
        }

       
    }
}
