using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisNetwork.packets
{
    public class Packet5GameOver : Packet
    {
        public Packet5GameOver() : base(5)
        {
        }

        public override void ReadPacket(BinaryReader reader)
        {
            
        }

        public override void WritePacket(BinaryWriter writer)
        {
            
        }
    }
}
