using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrisNetwork;

namespace Tetris.client
{
    public class ClientConfig
    {
        public String serverAdress = "127.0.0.1";
        public int serverPort = TetrisTCPServer.PORT;

        public ConsoleKey rotate = ConsoleKey.Z;
        public ConsoleKey down = ConsoleKey.S;
        public ConsoleKey left = ConsoleKey.Q;
        public ConsoleKey right = ConsoleKey.D;

    }
}
