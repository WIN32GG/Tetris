using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TetrisNetwork.packets;

namespace TetrisNetwork
{
    public interface NetworkCallback
    {
        void HandlePacket1Connect(Packet1Connect p1);
        void HandlePacket2Config(Packet2Config p2);
        void HandlePacket3Block(Packet3Block p3);
        void HandlePacket4Line(Packet4Line p4);
        void HandlePacket5GameOver(Packet5GameOver p5);
        void HandlePacket6Info(Packet6Info p6);
    }
}
