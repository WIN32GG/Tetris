using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisNetwork.packets
{
    public class Packet6Info : Packet
    {
        public String info;
        public int durationTick;

        public Packet6Info() : base(6)
        {
        }

        public override void ReadPacket(BinaryReader reader)
        {
            this.info = reader.ReadString();
            this.durationTick = reader.ReadInt32();
        }

        public override void WritePacket(BinaryWriter writer)
        {
            writer.Write(info);
            writer.Write(durationTick);
        }
    }
}
