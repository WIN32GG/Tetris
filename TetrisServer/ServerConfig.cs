using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrisNetwork;

namespace TetrisServer
{
    public class ServerConfig
    {
        public int columns = 12;
        public int maxLines = 17;
        public int delay_time = 600;
        public int port = TetrisTCPServer.PORT;
    }
}
