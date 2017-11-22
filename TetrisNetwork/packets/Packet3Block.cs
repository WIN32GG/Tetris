using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisNetwork.packets
{
    public class Packet3Block : Packet
    {
        public int blockID = -1;

        public Packet3Block() : base(3)
        {
        }
        
        public override void ReadPacket(BinaryReader reader)
        {
            this.blockID = reader.ReadInt32();
        }

        public override void WritePacket(BinaryWriter writer)
        {
            writer.Write(this.blockID);
        }
    }
}
