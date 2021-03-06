﻿using System;
using System.Collections.Concurrent;
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
        public static readonly Type[] PACKETS = new Type[] { null, typeof(Packet1Connect), typeof(Packet2Config), typeof(Packet3Block), typeof(Packet4Line), typeof(Packet5GameOver), typeof(Packet6Info) };

        private Socket socket;
        private volatile NetworkCallback callback = null;
        private volatile SocketExceptionCallback exHandler;

        private BinaryReader inputStream;
        private BinaryWriter outputStream;

        private ConcurrentQueue<Packet> outputPacketList = new ConcurrentQueue<Packet>();

        private volatile bool running = true;
        private Thread sendingThread;

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
            return this.socket.RemoteEndPoint.ToString();
        }

        private void SetupIO()
        {
            Stream stream = new NetworkStream(this.socket);

            this.inputStream = new BinaryReader(stream);
            this.outputStream = new BinaryWriter(stream);

            new Thread(this.ReceptionThreadTarget).Start();
            this.sendingThread = new Thread(this.SendingThreadTarget);
            this.sendingThread.Start();
        }

        public void SendPacket(Packet p)
        {
            this.outputPacketList.Enqueue(p);
            if(this.sendingThread.ThreadState == ThreadState.Suspended)
                this.sendingThread.Resume();
        }

        private void DispatchPacket(Packet p)
        {
            //par réflection
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

        private void SendingThreadTarget()
        {
            while(running)
            {

                Packet p = null;
                if (this.outputPacketList.IsEmpty)
                    Thread.CurrentThread.Suspend();
                this.outputPacketList.TryDequeue(out p); 
                if(p != null)
                {
                    this.outputStream.Write(p.GetID());
                    p.WritePacket(this.outputStream);
                    this.outputStream.Flush();
                }
            }
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
                    //Console.WriteLine("/!\\ WARNING /!\\ Closing socket to: " + this.GetAdress());
                    //Console.WriteLine(e1);
                    this.Close();
                    this.exHandler.Disconnect(this, e1);
                    return;
                }
                
                try
                {
                    this.DispatchPacket(p);
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    // return; // <-- ?
                }

            }
        }


    }
}
