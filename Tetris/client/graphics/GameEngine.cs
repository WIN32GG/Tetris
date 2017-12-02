using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris.client.graphics
{
    class GameEngine
    {
        public const int FREE = 0;
        public const int OBJECT = 1;
        public const int BLOCKED = 2;
        public readonly char[] REPRESENTATION_ARRAY = new char[] {' ', '#', '*'};

        private GraphicCallback callback;
        private volatile int[] gridShape;
        private int[,] grid;
        private int delaySpeed = 500;
        private volatile bool los = false; //the piece has just been added, if collision -> lost
        private Stopwatch gravityWatcher = new Stopwatch();
        private Piece currentPiece = null;
        private bool straightDown = false;
        private volatile bool running = true;

        private volatile int penalities = 0;
        private volatile int filled = 0;

        private object consoleLock = new object();

        public GameEngine(GraphicCallback cb)
        {
            this.callback = cb;
            gridShape = cb.GridShape();
            Console.CursorVisible = false;
        }

        public void OnPlayerAction(PlayerAction act)
        {
            switch(act)
            {
                case (PlayerAction.DOWN):
                    straightDown = true;
                    while (straightDown)
                        DoGameUpdate();                    
                    return;

                case (PlayerAction.LEFT):
                    MovePlayer(-1);
                    return;

                case (PlayerAction.RIGHT):
                    MovePlayer(1);
                    return;
            }
        }

        public void Stop()
        {
            running = false;
        }

        /// <summary>
        /// Try to move the player in the desired direction
        /// </summary>
        /// <param name="where">the direction</param>
        private void MovePlayer(int where)
        {
            if (currentPiece == null)
                return;

            lock(consoleLock)
            {
                currentPiece.UpdatePos(0, where);
                if (currentPiece.IsOut(grid) || currentPiece.Collides(grid))
                {
                    currentPiece.UpdatePos(0, where * -1);
                    return;
                }

                Render(DoGameUpdate());
            }
            
        }

        public void SetDelaySpeed(int sp)
        {
            this.delaySpeed = sp;
        }

        /// <summary>
        /// Set this piece to be the current piece controlled by the Player, sets a random pos
        /// </summary>
        /// <param name="p"></param>
        public void SetCurrentPiece(Piece p)
        {
            p.SetPos(0, 2);
            this.currentPiece = p;
            los = true;           
        }

        /// <summary>
        /// A never ending method updating and printing the game
        /// </summary>
        public void RunGameEngine()
        {
            grid = new int[gridShape[0], gridShape[1]];
            gravityWatcher.Start();
            
            while(running)
            {
                if (los && currentPiece.Collides(grid))
                    this.callback.HitTop();
                los = false;

                Thread.Sleep(1000 / 60); //60 FPS  --> use delta time

                lock(consoleLock)
                { 
                    Render(DoGameUpdate());
                } 
            }
        }

        /// <summary>
        /// Check the grid to see if there are complete horizontal lines
        /// </summary>
        /// <returns>the number of complete horizontals lines</returns>
        private int CheckLines()
        {
            int n = 0;
            for (int i = grid.GetLength(0) -1 ; i > 0 ; i--)
            {
                bool line = true;
                for(int j = 0; j < grid.GetLength(1); j++)
                {
                    if(grid[i, j] != GameEngine.OBJECT)
                    {
                        line = false;
                        break;
                    }
                }

                if(line)
                {
                    n += 1;
                    RemoveLine(i);
                    GoDown(i);
                    i++; //FIXME <- 
                }

            }
            return n;
        }

    

        private void GoDown(int from, int l, int c)
        {
            if (l == 0)
            {
                GoDown(from, from, c + 1);
                return;
            }
                
            if (c == grid.GetLength(1))
                return;

            grid[l, c] = grid[l - 1, c];
            grid[l - 1, c] = FREE;

            GoDown(from, l - 1, c);
             
        }

        public void GoUp()
        {
            GoUp(0, 0);
        }

        private void GoDown(int l)
        {
            GoDown(l, l, 0);
        }

        private void GoUp(int l, int c)
        {
            if (l == grid.GetLength(0) - 1)
            {
                GoUp(0, c + 1);
                return;
            }

            if (c == grid.GetLength(1))
                return;

            grid[l, c] = grid[l + 1, c];
            grid[l + 1, c] = FREE;

            GoUp(l + 1, c);
        }

        public void AddPenality()
        {
            penalities++;
            GoUp();
            SetLine(grid.GetLength(0) - 1, GameEngine.BLOCKED);
           
        }

        private void SetLine(int l, int val)
        {
            for (int c = 0; c < grid.GetLength(1); c++)
            {
                grid[l, c] = val;
            }
        }

        private void RemoveLine(int l)
        {
            SetLine(l, GameEngine.FREE);  
        }

        /// <summary>
        ///  Updates the falling Piece, checks the collision and roll back and add the piece to the grid if any
        /// </summary>
        private int[,] DoGameUpdate()
        {
            if (currentPiece == null)
                return grid;
            
            int[,] printGrid = PrintOnGrid(grid, currentPiece);

            if (straightDown || gravityWatcher.ElapsedMilliseconds > this.delaySpeed)
            {
                gravityWatcher.Restart();
                currentPiece.UpdatePos(1, 0);                

                if (currentPiece.Collides(grid))
                {
                    currentPiece.UpdatePos(-1, 0); //rollback
                    grid = printGrid;
                    
                    currentPiece = null;
                    this.straightDown = false;
                    this.callback.PieceCollides();
                    int n = this.CheckLines();
                    this.filled += n;
                    if (n > 0)
                        this.callback.LineFilled(n);
                } 
            }

            return printGrid;
        }

        private int[,] PrintOnGrid(int[,] grid, Piece piece)
        {
            int[,] tmp = (int[,])grid.Clone();

            int[] pos = piece.GetPos();
            for (int i = 0; i < piece.shape.GetLength(0); i++)
            {
                for(int j = 0; j < piece.shape.GetLength(1); j++)
                {
                    if(piece.shape[i,j] == GameEngine.OBJECT)
                        tmp[pos[0] + i, pos[1] + j] = piece.shape[i, j];
                }
            }

            return tmp;
        }
   
        private void DrawLine(ref string buff)
        {
            for(int i = 0; i < gridShape[1] + 2; i ++)
            {
                buff += "█";
            }
            buff += "\n";
        }

        //Prints to the Console
        public void Render(int[,] grid)
        {
            string buffer = "";
            
            DrawLine(ref buffer);
            for(int i = 0; i< gridShape[0]; i++)
            {
                buffer += "█";
                for (int j = 0; j< gridShape[1]; j++)
                {
                    buffer += REPRESENTATION_ARRAY[grid[i,j]];
                }
                buffer += "█\n";
            }
            DrawLine(ref buffer);
            buffer += "\n";
            buffer += "Lignes faites: " + filled+"\n";
            buffer += "Pénalités reçues: " + penalities+"\n";
            buffer += "===========================\n";

            Console.SetCursorPosition(0, 0);
            Console.Write(buffer);

        }

    }
}
