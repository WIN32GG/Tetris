﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrisNetwork;
using TetrisNetwork.packets;

namespace Tetris.client
{
    public class TetrisClient : NetworkCallback, GraphicCallback
    {
        
        public TetrisClient()
        {

        }

        public void HandlePacket1Connect(Packet1Connect p1)
        {
            throw new NotImplementedException();
        }

        public void HandlePacket2Config(Packet2Config p2)
        {
            
        }

        public void HandlePacket3Block(Packet3Block p3)
        {
            
        }

        public void HandlePacket4Line(Packet4Line p4)
        {
            
        }

        public void HandlePacket5GameOver(Packet5GameOver p5)
        {
            
        }

        public void HandlePacket6Info(Packet6Info p6)
        {
            
        }

        public void HitTop()
        {
            
        }

        public void LineFilled(int num)
        {
            
        }

        public void PieceCollides()
        {
            
        }

        public int[] GridShape()
        {
            return null;
        }
    }
}
