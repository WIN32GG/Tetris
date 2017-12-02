using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        private bool launching = false;
        private bool playing = false;
        
        private ServerConfig cfg;
        private ConcurrentDictionary<NetHandler, Client> clients = new ConcurrentDictionary<NetHandler, Client>();
        

        internal TetrisServer(ServerConfig cfg)
        {
            instance = this;
            this.cfg = cfg;
            tcpServer = new TetrisTCPServer(this, cfg.port);
        }

        internal void SendBlockIfStarted(Client client)
        {
            if (playing)
                SendBlock(client);
        }

        public NetHandler GetNetHandlerForClient(Client c)
        {
            foreach(NetHandler n in clients.Keys)
            {
                if (clients[n].Equals(c))
                    return n;
            }

            return null;    
        }

        internal void ClientLost(Client client)
        {         
            client.inGame = false;
            int ig = this.InGamePlayers();
            if (ig == 1)
            {
                //A player won
                foreach(Client c in clients.Values)
                {
                    if(c.inGame)
                    {
                        Packet5GameOver p5 = new Packet5GameOver();
                        
                        this.SendPacket(p5, c);
                        Thread.Sleep(2000);
                        this.ResetAndRestart();
                    }
                }
                return;
            }
                
            Broadcast(ig + " players remaining");

        }

        private void ResetAndRestart()
        {
            this.launching = false;
            this.playing = false;

            foreach(Client c in this.clients.Values)
            {
                this.SendConfig(c);
                c.inGame = true;
            }

            this.LaunchGame();
            Console.WriteLine("Game is restarting...");
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
                if(except == null || !except.Contains(clients[nh]))
                    nh.SendPacket(p);   
            }
        }

        public void ClientConnect(NetHandler handler)
        {
            Client client = new Client();
            handler.SetCallBack(client);

            clients[handler] = client;

            LaunchGame();
        }

        public int InGamePlayers()
        {
            int cl = 0;
            foreach(Client c in clients.Values)
            {
                if (c.inGame)
                    cl++;
            }

            return cl;
        }

        public void Broadcast(String str)
        {
            Packet6Info p6 = new Packet6Info();
            p6.info = str;
            p6.durationTick = 0;

            this.Broadcast(p6, null);
        }

        private void LaunchGame()
        {
            if (launching)
                return;
            launching = true;

            new Thread(this.LaunchGameThreadTarget).Start();
        }

        private void LaunchGameThreadTarget()
        {
            int sec = 10;
            while(sec >= 0)
            {
                this.Broadcast("Début dans " + sec);
                Thread.Sleep(1000);
                sec--;
            }
            playing = true;

            foreach(Client c in this.clients.Values)
            {
                this.SendBlock(c);
            }
        }

        public void Disconnect(NetHandler handler, Exception ex)
        {
            Console.WriteLine("Removed Client");
            Client c = null;
            clients.TryRemove(handler, out c);
        }
    }
}
