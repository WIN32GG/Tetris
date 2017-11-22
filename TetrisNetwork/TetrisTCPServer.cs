using System;
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
        private IServer theServer;
      
        public TetrisTCPServer(IServer server)
            : this(server, PORT)
        {
            Console.WriteLine("The default port has been used");
        }

        public TetrisTCPServer(IServer server, int port)
            :base(IPAddress.Any, port)
        {
            this.theServer = server;
            Console.WriteLine("Server listening on port " + port);
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
                NetHandler h = new NetHandler(base.AcceptSocket(), theServer);
                Console.WriteLine("Connection from: " + h.GetAdress());
                this.theServer.ClientConnect(h);
            }
        }

    }
}
