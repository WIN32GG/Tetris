using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TetrisNetwork.packets;

namespace TetrisNetwork
{
    /// <summary>
    /// Manages a Connection between a Server a a Client
    /// </summary>
    public class NetHandler
    {
        public static Type[] PACKETS = new Type[] { null, typeof(Packet1Connect) };

        private Socket socket;
        private NetworkCallback callback = null;
        private SocketExceptionCallback exHandler;

        private BinaryReader inputStream;
        private BinaryWriter outputStream;

        private volatile bool running = true;

        public NetHandler(Socket sck, SocketExceptionCallback srv)
        {
            this.exHandler = srv;
            this.socket = sck;
        }

        public void Stop()
        {
            running = false;
            this.Close();
        }

        public void SetCallBack(NetworkCallback clb)
        {
            this.callback = clb;
            this.SetupIO();
        }

        public string GetAdress()
        {
            return this.socket.RemoteEndPoint.Serialize().ToString();
        }

        private void SetupIO()
        {
            Stream stream = new NetworkStream(this.socket);

            this.inputStream = new BinaryReader(stream);
            this.outputStream = new BinaryWriter(stream);

            new Thread(this.ReceptionThreadTarget).Start();
        }

        public void SendPacket(Packet p)
        {
            this.outputStream.Write(p.GetID());
            p.WritePacket(this.outputStream);
        }

        private void DispatchPacket(Packet p)
        {
            //TEST par réflection
            MethodInfo theMethod = this.callback.GetType().GetMethod("Handle" + p.GetType().Name);

            if(theMethod == null)
            {
                Console.WriteLine("NoSuchMethod for packet with id "+p.GetID());
                throw new InvalidDataException("Unknown Packet id "+p.GetID());
            }

            theMethod.Invoke(this.callback, new object[] { p });
             
            /*
            switch (p.GetID())
            {
                case (1):
                    this.callback.HandlePacket1Connect((Packet1Connect)p);
                    return;
            }*/
        }

        private void Close()
        {
            if(!this.socket.Connected)
            {
                this.socket.Shutdown(SocketShutdown.Both);
                this.socket.Close();
            }
        }

        private void ReceptionThreadTarget()
        {
            while(running)
            {
                Packet p = null;
                
                try
                {
                    int packetId = this.inputStream.ReadInt32();
                    p = (Packet)Activator.CreateInstance(PACKETS[packetId]);
                    p.ReadPacket(this.inputStream);

                } catch(Exception e1)
                {
                    Console.WriteLine("/!\\ WARNING /!\\ Closing socket to: " + this.GetAdress());
                    this.Close();
                    this.exHandler.Disconnect(this, e1);
                }
                
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
