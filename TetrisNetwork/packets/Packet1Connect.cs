using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisNetwork.packets
{
    public class Packet1Connect:Packet
    {

        public String playerName;
        public int gameVersion;

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
