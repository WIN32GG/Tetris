using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tetris.client.graphics;
using TetrisNetwork;
using TetrisNetwork.packets;

namespace Tetris.client
{
    public class TetrisClient : NetworkCallback, GraphicCallback, SocketExceptionCallback
    {

        private NetHandler theNetwork;
        private GameEngine theEngine;

        private ClientConfig cfg;
        private int[] gridShape;

        private bool started = false;

        private Piece[] pieces;
        
        public TetrisClient(ClientConfig cfg)
        {
            this.cfg = cfg;
            InitPieces();
        }

        private void InitPieces()
        {
            int[,] shape = new int[,] { { GameEngine.OBJECT } };
            int[,] shape2 = new int[,] { { GameEngine.OBJECT, GameEngine.OBJECT }, { GameEngine.OBJECT, GameEngine.OBJECT } };

            pieces = new Piece[2];

            pieces[0] = new Piece(shape);
            pieces[1] = new Piece(shape2);
        }

        public int[] GridShape()
        {
            return this.gridShape;
        }

        internal void Connect()
        {
            Console.WriteLine("Connexion à " + cfg.serverAdress + ":" + cfg.serverPort);

            try
            {
                Socket sck = new Socket(SocketType.Stream, ProtocolType.Tcp);
                sck.Connect(cfg.serverAdress, cfg.serverPort);

                NetHandler handler = new NetHandler(sck, this);
                handler.SetCallBack(this);
                this.theNetwork = handler;
                this.SendHandshake();
                Console.WriteLine("=== Connected ===");
            }catch(Exception ex)
            {
                Console.WriteLine("Could not connect to server");
                Console.WriteLine(ex.ToString());
            }
        }

        private void SendHandshake()
        {
            Packet1Connect p1 = new Packet1Connect();
            p1.playerName = "unknown";
            p1.gameVersion = 1;
            this.theNetwork.SendPacket(p1);
        }

        private void StartGame()
        {
            this.theEngine = new GameEngine(this);
            new Thread(theEngine.RunGameEngine).Start();
            new Thread(HandlePlayerInputThreadTarget).Start();
        }

        private void HandlePlayerInputThreadTarget()
        {
            while (true)
            {
                ConsoleKey key = Console.ReadKey().Key;
                PlayerAction action = PlayerAction.NONE;
                if (key == this.cfg.down)
                {
                    action = PlayerAction.DOWN;
                }
                else if (key == this.cfg.left)
                {
                    action = PlayerAction.LEFT;
                }
                else if (key == this.cfg.right)
                {
                    action = PlayerAction.RIGHT;
                }
                else if (key == this.cfg.rotate)
                {
                    action = PlayerAction.ROTATE;
                }

                this.theEngine.OnPlayerAction(action);
            }
        }

        public void HandlePacket1Connect(Packet1Connect p1)
        {
            throw new NotImplementedException();
        }

        public void HandlePacket2Config(Packet2Config p2)
        {
            Console.Clear();
            this.gridShape = new int[] { p2.maxLines, p2.columns };
            this.StartGame();
            this.theEngine.SetDelaySpeed(p2.delaySpeed);
        }

        public void HandlePacket3Block(Packet3Block p3)
        {
            if(!this.started)
            {
                Console.Clear();
                this.started = true;
            }

            this.theEngine.SetCurrentPiece(pieces[p3.blockID]);
        }

        public void HandlePacket4Line(Packet4Line p4)
        {
            for(int i = 0; i< p4.lineNumber; i++)
            {
                this.theEngine.AddPenality();
            }
        }

        public void HandlePacket5GameOver(Packet5GameOver p5)
        {
            this.theEngine.Stop();
            Console.WriteLine("==== YOU WIN ===");
        }

        public void HandlePacket6Info(Packet6Info p6)
        {
            Console.WriteLine(p6.info+"    ");
            //TODO erace after delay
        }

        public void HitTop()
        {
            Packet5GameOver p5 = new Packet5GameOver();
            this.theNetwork.SendPacket(p5);
            this.theEngine.Stop();
            Console.WriteLine("==== GAME OVER ====");
        }

        public void LineFilled(int num)
        {
            Packet4Line p4 = new Packet4Line();
            p4.lineNumber = num;

            this.theNetwork.SendPacket(p4);
        }

        public void PieceCollides()
        {
            Packet3Block p3 = new Packet3Block();
            p3.blockID = -1;

            this.theNetwork.SendPacket(p3);
        }
        
        public void Disconnect(NetHandler handler, Exception ex)
        {
            Console.WriteLine("Disconnected from server");
            if(this.theEngine != null)
                this.theEngine.Stop();
        }
    }
}
