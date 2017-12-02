using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace TetrisNetwork
{
    public class TetrisTCPServer: TcpListener
    {

        public const int PORT = 28943; //default port that was used for test

        private volatile bool running = true;
        private IServer theServer;
      

        public TetrisTCPServer(IServer server, int port)
            :base(IPAddress.Any, port)
        {
            base.Start();
            this.theServer = server;
            new Thread(this.ReceptionThreadTarget).Start();
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
                try
                {
                    NetHandler h = new NetHandler(base.AcceptSocket(), theServer);
                    Console.WriteLine("Connection from: " + h.GetAdress());
                    this.theServer.ClientConnect(h);
                }catch(Exception ex)
                {
                    Console.WriteLine("Error while listening for connections");
                    Console.WriteLine(ex);
                    return;
                }
            }
        }

    }
}
