using RaknetCS.Protocol.Raknet;
using RaknetCS.Protocol.Packets;
using RaknetCS.Protocol;

using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Net;
using System;

namespace RaknetCS.Network
{
    public class RaknetSession
    {
        public IPEndPoint PeerEndPoint { get; private set; }

        private static readonly Dictionary<int, List<(Type, Delegate)>> Listeners = new Dictionary<int, List<(Type, Delegate)>>();

        public Thread SenderThread;
        public byte rak_version;
        public Timer PingTimer;
        public bool Connected;
        public RecvQ Recvq;
        public SendQ Sendq;

        private readonly AsyncUdpClient Socket;
        private bool thrownUnkownPackets;
        public int MaxRepingCount = 6;
        private int repingCount;
        private ulong guid;

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public delegate void SessionDisconnectedDelegate(RaknetSession session);
        public SessionDisconnectedDelegate SessionDisconnected = delegate { };

        public delegate void PacketReceiveBytesDelegate(IPEndPoint address, byte[] bytes);
        public PacketReceiveBytesDelegate SessionReceiveRaw = delegate { };

        public RaknetSession(AsyncUdpClient Socket, IPEndPoint Address, ulong guid, byte rakVersion, RecvQ recvQ, SendQ sendQ, bool thrownUnkownPackets = false)
        {
            this.Socket = Socket;
            PeerEndPoint = Address;
            this.guid = guid;
            Connected = true;
            this.thrownUnkownPackets = thrownUnkownPackets;

            rak_version = rakVersion;

            Sendq = sendQ;
            Recvq = recvQ;

            StartPing();
            SenderThread = StartSender();
        }

        public void Subscribe<T>(Action<T> action) where T : Packet {
            Type packetType = typeof(T);

            RegisterPacketID attribute = 
                packetType.GetCustomAttribute<RegisterPacketID>() ?? throw new Exception(packetType.FullName + " must have the [RegisterPacketID(int)] attribute.");

            bool hasBufferConstructor = false;
            foreach(ConstructorInfo constructor in packetType.GetConstructors())
            {
                ParameterInfo[] parameters = constructor.GetParameters();

                if (parameters.Length != 1) continue;
                if (parameters[0].ParameterType == typeof(byte[])) continue;

                hasBufferConstructor = true;
            }

            if (!hasBufferConstructor) throw new Exception(packetType.FullName + " must have a constructor that takes only a byte[].");

            int packetId = attribute.ID;

            bool exists = Listeners.TryGetValue(packetId, out var listeners);
            if(!exists)
            {
                listeners = new List<(Type, Delegate)>();
                Listeners.Add(packetId, listeners);
            }

            listeners.Add((packetType, action));
        }

        public void HandleFrameSet(IPEndPoint peer_addr, byte[] data)
        {
            if (!Connected)
            {
                foreach (FrameSetPacket f in Recvq.Flush(peer_addr))
                {
                    HandleFrame(peer_addr, f);
                }
            }

            RaknetReader stream = new RaknetReader(data);
            byte headerFlags = stream.ReadU8();

            switch ((PacketID)headerFlags)
            {
                case PacketID.Nack:
                    {
                        lock (Sendq)
                        {
                            Nack nack = new Nack(data);
                            nack.Deserialize();
                            for (int i = 0; i < nack.record_count; i++)
                            {
                                if (nack.sequences[i].Start == nack.sequences[i].End)
                                {
                                    Sendq.Nack(nack.sequences[i].Start, Common.CurTimestampMillis());
                                }
                                else
                                {
                                    for (uint j = nack.sequences[i].Start; j <= nack.sequences[i].End; j++)
                                    {
                                        Sendq.Nack(j, Common.CurTimestampMillis());
                                    }
                                }
                            }
                        }

                        break;
                    }
                case PacketID.Ack:
                    {
                        lock (Sendq)
                        {
                            Ack ack = new Ack(data);
                            ack.Deserialize();
                            for (int i = 0; i < ack.record_count; i++)
                            {
                                if (ack.sequences[i].Start == ack.sequences[i].End)
                                {
                                    Sendq.Ack(ack.sequences[i].Start, Common.CurTimestampMillis());
                                }
                                else
                                {
                                    for (uint j = ack.sequences[i].Start; j <= ack.sequences[i].End; j++)
                                    {
                                        Sendq.Ack(j, Common.CurTimestampMillis());
                                    }
                                }
                            }
                        }

                        break;
                    }
                default:
                    {
                        if ((PacketID)data[0] >= PacketID.FrameSetPacketBegin 
                            && (PacketID)data[0] <= PacketID.FrameSetPacketEnd)
                        {
                            var frames = new FrameVec(data);
                            lock (Recvq)
                            {
                                foreach (var frame in frames.frames)
                                 {
                                     Recvq.Insert(frame);
                                     foreach (FrameSetPacket f in Recvq.Flush(peer_addr))
                                     {
                                         HandleFrame(peer_addr, f);
                                     }
                                 }
 
                             }
                             var acks = Recvq.GetAck();
                             if (acks.Count != 0)
                             {
                                 Ack packet = new Ack((ushort)acks.Count, acks);
                                 byte[] buf = packet.Serialize();
                                 Socket.Send(peer_addr, buf);
                             }
                         }
                         break;
                     }
             }
         }
 
         public void HandleFrame(IPEndPoint address, FrameSetPacket frame)
         {
             PacketID packetID = (PacketID)frame.data[0];
 
             switch (packetID)
             {
                 case PacketID.ConnectedPing:
                     HandleConnectPing(frame.data);
                     break;
                 case PacketID.ConnectedPong:
                     repingCount = 0;
                     break;
                 case PacketID.ConnectionRequest:
                     HandleConnectionRequest(address, frame.data);
                     break;
                 case PacketID.ConnectionRequestAccepted:
                     HandleConnectionRequestAccepted(frame.data);
                     break;
                 case PacketID.NewIncomingConnection:
                     break;
                 case PacketID.Disconnect:
                     HandleDisconnectionNotification();
                     break;
                 default:
                     SessionReceiveRaw(address, frame.data);
                     HandleIncomingPacket(frame.data);
                     break;
             }
         }
 
         private void HandleIncomingPacket(byte[] buffer) {
             byte packetID = buffer[0];
 
             bool exists = Listeners.TryGetValue(packetID, out List<(Type, Delegate)> value);
             if (!exists) return;
 
             foreach((Type, Delegate) registration in value)
             {
                 Delegate callback = registration.Item2;
                 Type packetType = registration.Item1;
 
                 MethodInfo method = packetType.GetMethod("Deserialize");
                 if (method == null) return;
 
                 object packet = Activator.CreateInstance(packetType, new object[] { buffer });
                 method.Invoke(packet, new object[] {});
 
                 callback.DynamicInvoke(packet);
             }
         }
 
         private void HandleConnectPing(byte[] data)
         {
             ConnectedPing pingPacket = new ConnectedPing(data);
             pingPacket.Deserialize();
             ConnectedPong pongPacket = new ConnectedPong(pingPacket.client_timestamp, Common.CurTimestampMillis());
 
             byte[] buf = pongPacket.Serialize();
             lock (Sendq)
                 Sendq.Insert(Reliability.Unreliable, buf);
         }
 
         private void HandleConnectionRequestAccepted(byte[] data)
         {
             ConnectionRequestAccepted packet = new ConnectionRequestAccepted(data);
             packet.Deserialize();
             NewIncomingConnection packet_reply = new NewIncomingConnection((IPEndPoint)Socket.Socket.Client.LocalEndPoint, packet.request_timestamp, Common.CurTimestampMillis());
             
             byte[] buf = packet_reply.Serialize();
             lock (Sendq)
                 Sendq.Insert(Reliability.ReliableOrdered, buf);
         }
 
         private void HandleConnectionRequest(IPEndPoint peer_addr, byte[] data)
         {
             ConnectionRequest packet = new ConnectionRequest(data);
             packet.Deserialize();
             
             ConnectionRequestAccepted packet_reply = new ConnectionRequestAccepted(peer_addr, 0, packet.time, Common.CurTimestampMillis());
             
             byte[] buf = packet_reply.Serialize();
             lock (Sendq)
                 Sendq.Insert(Reliability.ReliableOrdered, buf);
         }
 
         private void HandleDisconnectionNotification()
         {
             Connected = false;
            _cts.Cancel();
            PingTimer.Dispose();
             SessionDisconnected(this);
             GC.Collect();
         }
 
         public void HandleConnect()
         {
             ConnectionRequest requestPacket = new ConnectionRequest(guid, Common.CurTimestampMillis(), 0x00);
             
             byte[] buf = requestPacket.Serialize();
             lock (Sendq)
                 Sendq.Insert(Reliability.ReliableOrdered, buf);
         }
 
         public Thread StartSender()
        {
            CancellationToken token = _cts.Token;
            Thread thread = new Thread(() => 
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        Thread.Sleep(100);
                        foreach (FrameSetPacket item in Sendq.Flush(Common.CurTimestampMillis(), PeerEndPoint))
                        {
                            byte[] sdata = item.Serialize();
                            Socket.Send(PeerEndPoint, sdata);
                        }
                    }
                    catch (ThreadInterruptedException)
                    {
                        break;
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            });
            thread.Start();
            return thread;
        }
 
         public void StartPing()
         {
             PingTimer = new Timer(SendPing, null,
                 new Random().Next(1000, 1500), new Random().Next(1000, 1500));
         }
 
         public void SendPing(object obj)
         {
             if (!Connected) return;
             ConnectedPing pingPacket = new ConnectedPing(Common.CurTimestampMillis());
 
             byte[] buffer = pingPacket.Serialize();
             lock (Sendq)
                 Sendq.Insert(Reliability.Unreliable, buffer);
 
             repingCount++;
             if (repingCount < MaxRepingCount) return;
 
             HandleDisconnectionNotification();
         }
     }
}
