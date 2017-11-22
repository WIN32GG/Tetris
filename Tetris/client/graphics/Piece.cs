using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.client.graphics
{
    class Piece
    {

        public readonly int[,] shape;

        private int[] pos = new int[] { 0, 0 };

        public Piece(int[,] shape)
        {
            this.shape = shape;
        }

        public void SetPos(int x, int y)
        {
            pos[0] = x;
            pos[1] = y;
        }

        public int[] GetPos()
        {
            return pos;
        }

        public void UpdatePos(int mx, int my)
        {
            this.pos[0] += mx;
            this.pos[1] += my;
        }

        //checks if the piece is out of the grid
        public bool IsOut(int[,] grid)
        {
            if (pos[1]  < 0)
                return true;
            if (pos[1] + shape.GetLength(1) > grid.GetLength(1))
                return true;

            return false;
        }

        //Checks if this Piece enters in collision with elements in the grid, aka supperposition
        public bool Collides(int[,] grid)
        {
            for(int i = 0; i < shape.GetLength(0); i++)
            {
                for(int j = 0; j < shape.GetLength(1); j++)
                {
                    if(pos[0] + i + 1 > grid.GetLength(0) || shape[i,j] == GameEngine.OBJECT && grid[pos[0] + i, pos[1] + j] != GameEngine.FREE)
                        return true;
                }
            }
            return false;
        }



    }
}
