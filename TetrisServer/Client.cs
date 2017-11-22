using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrisNetwork;
using TetrisNetwork.packets;

namespace TetrisServer
{
    public class Client : NetworkCallback
    {
        public void HandlePacket1Connect(Packet1Connect p1)
        {
            //Send config
            TetrisServer.GetInstance().SendConfig(this);
        }

        public void HandlePacket2Config(Packet2Config p2)
        {
            throw new NotImplementedException();
        }

        public void HandlePacket3Block(Packet3Block p3)
        {
            //block request
            TetrisServer.GetInstance().SendBlock(this);
        }

        public void HandlePacket4Line(Packet4Line p4)
        {
            //send penality
            TetrisServer.GetInstance().SendLine(this, p4.lineNumber);
        }

        public void HandlePacket5GameOver(Packet5GameOver p5)
        {
            //Deco?
        }

        public void HandlePacket6Info(Packet6Info p6)
        {
            throw new NotImplementedException();
        }
    }
}
