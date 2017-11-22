using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisNetwork.packets
{
    public class Packet4Line : Packet
    {
        public int lineNumber;

        public Packet4Line() : base(4)
        {
        }

        public override void ReadPacket(BinaryReader reader)
        {
            this.lineNumber = reader.ReadInt32();
        }

        public override void WritePacket(BinaryWriter writer)
        {
            writer.Write(this.lineNumber);
        }
    }
}
