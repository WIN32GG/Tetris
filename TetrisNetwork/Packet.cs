using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisNetwork
{
    public abstract class Packet
    {
        private int id;

        protected Packet(int id)
        {
            this.id = id;
        }

        public int GetID()
        {
            return id;
        }

        public abstract void ReadPacket(BinaryReader reader);
        public abstract void WritePacket(BinaryWriter writer);

    }
}
