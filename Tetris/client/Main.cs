using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tetris.client.graphics;


namespace Tetris.client
{
    class ClientMain: GraphicCallback
    {

        static GameEngine eng;
        static Piece p2;
        static Piece p;

        public static void Main(String[] args)
        {

            FallTest();
            Console.ReadKey();
            
        }

        public static void HandlePlayerInput()
        {
            
            while(true)
            {
      
                ConsoleKey k = Console.ReadKey().Key;

                if (k == ConsoleKey.S)
                {
                    eng.OnPlayerAction(PlayerAction.DOWN);
                }

                if(k == ConsoleKey.D)
                {
                    eng.OnPlayerAction(PlayerAction.RIGHT);
                }

                if(k == ConsoleKey.Q)
                {
                    eng.OnPlayerAction(PlayerAction.LEFT);
                }
            }
        }

        public static void FallTest()
        {
            eng = new GameEngine(new ClientMain());
            Thread t = new Thread(eng.RunGameEngine);
            t.Start();

            Thread t2 = new Thread(HandlePlayerInput);
            t2.Start();

            int[,] shape = new int[,] { { GameEngine.OBJECT} };
            int[,] shape2 = new int[,] { { GameEngine.OBJECT, GameEngine.OBJECT }, { GameEngine.OBJECT, GameEngine.OBJECT } };

            p = new Piece(1, shape);
            p2 = new Piece(2, shape2);
            eng.SetCurrentPiece(p2);
            while (true)
            {
                Thread.Sleep(20000);
            }

        }

        public void LineFilled(int num)
        {
            throw new NotImplementedException();
        }

        public void PieceCollides()
        {
            eng.SetCurrentPiece(p);
        }

        public void HitTop()
        {
            Console.WriteLine("Game over");
            Console.ReadKey();
        }

        public int[] GridShape()
        {
            return new int[] { 7, 6 };
        }
    }
}
