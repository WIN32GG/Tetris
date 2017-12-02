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
        public static ConsoleKey GetKeyFromInput(String str)
        {
            try
            {
                return (ConsoleKey)Enum.Parse(typeof(ConsoleKey), str.ToUpper());
            } catch(Exception ex)
            {
                Console.WriteLine("Unrecognized key: " + str);
                return ConsoleKey.NoName;
            }
            
        }

        public static void Main(String[] args)
        {

            ClientConfig cfg = new ClientConfig();
            //Handle args

            // server_address server_port high_key right_key low_key left_key
            try
            {
                cfg.serverAdress = args[0];
                cfg.serverPort = Convert.ToInt32(args[1]);

                cfg.rotate = GetKeyFromInput(args[2]);
                cfg.right = GetKeyFromInput(args[3]);
                cfg.down = GetKeyFromInput(args[4]);
                cfg.left = GetKeyFromInput(args[5]);
            } catch(IndexOutOfRangeException ex)
            {
                Console.WriteLine("Outof bounds: not all arguments were given");
            }
           
            TetrisClient client = new TetrisClient(cfg);
            client.Connect();
            
            
        }

       
    }
}
