using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.client
{
    interface GraphicCallback
    {
        void LineFilled(int num);
        void PieceCollides();
        void HitTop();
        int[] GridShape();
    }
}
