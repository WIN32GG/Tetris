﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TetrisNetwork
{
    public class NetHandler
    {
        public static Type[] PACKETS = new Type[] { null, typeof(Packet1Connect) };

        private Socket socket;
        private NetworkCallback callback = null;

        private BinaryReader inputStream;
        private BinaryWriter outputStream;

        private volatile bool running = true;

        public NetHandler(Socket sck)
        {
            this.socket = sck;
            this.SetupIO();
        }

        public void Stop()
        {
            running = false;
        }

        public void SetCallBack(NetworkCallback clb)
        {
            this.callback = clb;
        }

        private void SetupIO()
        {
            Stream stream = new NetworkStream(this.socket);

            this.inputStream = new BinaryReader(stream);
            this.outputStream = new BinaryWriter(stream);
        }

        public void SendPacket(Packet p)
        {
            this.outputStream.Write(p.GetID());
            p.WritePacket(this.outputStream);
        }

        /*private void SendingThreadTarget() thread pour l'envoi de packet
        {

        }*/

        private void DispatchPacket(Packet p)
        {
            switch (p.GetID())
            {
                case (1):
                    this.callback.HandlePacket1Connect((Packet1Connect)p);
                    return;
            }
        }

        private void ReceptionThreadTarget()
        {
            while(running)
            {
                int packetId = this.inputStream.ReadInt32();

                Packet p = (Packet)Activator.CreateInstance(PACKETS[packetId]);
                p.ReadPacket(this.inputStream);

                try
                {
                    this.DispatchPacket(p);
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    //wait?
                }

            }
        }


    }
}
