using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisNetwork
{
    public interface NetworkCallback
    {
        void HandlePacket1Connect(Packet1Connect p1);

    }
}
