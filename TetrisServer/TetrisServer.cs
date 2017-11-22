using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrisNetwork;
using TetrisNetwork.packets;

namespace TetrisServer
{
    public class TetrisServer : IServer
    {
        private static TetrisServer instance;
        public static TetrisServer GetInstance()
        {
            return instance;
        }


        private TetrisTCPServer tcpServer;
        private ServerConfig cfg;
        private Dictionary<NetHandler, Client> clients = new Dictionary<NetHandler, Client>();

        internal TetrisServer(ServerConfig cfg)
        {
            instance = this;
            this.cfg = cfg;
            tcpServer = new TetrisTCPServer(this);
        }

        public NetHandler GetNetHandlerForClient(Client c)
        {
            foreach(NetHandler n in clients.Keys)
            {
                if (n.Equals(c))
                    return n;
            }

            return null;    
        }

        public int GenerateBlockID()
        {
            return new Random().Next(2);
        }

        public void SendConfig(Client c)
        {
            Packet2Config p2 = new Packet2Config();
            p2.delaySpeed = this.cfg.delay_time;
            p2.columns = this.cfg.columns;
            p2.maxLines = this.cfg.maxLines;

            this.SendPacket(p2, c);
        }

        public void SendBlock(Client c)
        {
            Packet3Block p3 = new Packet3Block();
            p3.blockID = GenerateBlockID();

            SendPacket(p3, c);
        }

        public void SendLine(Client from, int many)
        {
            Packet4Line p4 = new Packet4Line();
            p4.lineNumber = many;

            this.Broadcast(p4, from);
        }

        public void SendPacket(Packet p, Client c)
        {
            this.GetNetHandlerForClient(c).SendPacket(p);
        }

        public void Broadcast(Packet p, params Client[] except)
        {
            foreach(NetHandler nh in clients.Keys)
            {
                if(except != null && !except.Contains(clients[nh]))
                    nh.SendPacket(p);   
            }
        }

        public void ClientConnect(NetHandler handler)
        {
            Client client = new Client();
            handler.SetCallBack(client);

            clients[handler] = client;
        }

        public void Disconnect(NetHandler handler, Exception ex)
        {
            Console.WriteLine("Removed Client " + handler.GetAdress());
            clients.Remove(handler);
        }
    }
}
