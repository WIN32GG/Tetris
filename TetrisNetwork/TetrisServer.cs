﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TetrisNetwork
{
    public class TetrisTCPServer: TcpListener
    {

        public const int PORT = 28943;

        private volatile bool running = true;
      
        public TetrisTCPServer(NetworkCallback clb)
            : base(IPAddress.Any, PORT) 
        {
            
        }

        public new void Stop()
        {
            running = false;
            base.Stop();
        }
        
        private void ReceptionThreadTarget()
        {
            while (running)
            {
                NetHandler h = new NetHandler(base.AcceptSocket());
                //new TetrisClient
                    
            }
        }


    }
}
