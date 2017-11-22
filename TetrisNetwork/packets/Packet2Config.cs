using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisNetwork.packets
{
    public class Packet2Config : Packet
    {
        public int maxLines;
        public int columns;
        public int delaySpeed;

        public Packet2Config() : base(2)
        {
        }
      
        public override void ReadPacket(BinaryReader reader)
        {
            this.maxLines = reader.ReadInt32();
            this.columns = reader.ReadInt32();
            this.delaySpeed = reader.ReadInt32();
        }

        public override void WritePacket(BinaryWriter writer)
        {
            writer.Write(this.maxLines);
            writer.Write(this.columns);
            writer.Write(this.delaySpeed);
        }
    }
}
