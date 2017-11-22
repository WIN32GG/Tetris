using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisNetwork
{
    public class Packet1Connect:Packet
    {

        private String playerName;
        private int gameVersion;

        public Packet1Connect() : base(1)
        {
        }

        public override void ReadPacket(BinaryReader reader)
        {
            this.playerName = reader.ReadString();
            this.gameVersion = reader.ReadInt32();
        }

        public override void WritePacket(BinaryWriter writer)
        {
            writer.Write(this.playerName);
            writer.Write(this.gameVersion);
        }
    }
}
