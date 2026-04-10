using System.Collections.Generic;
using System.Reflection;
using System.Net;
using System;

namespace RaknetCS.Protocol.Raknet {
    public abstract class Packet {
        public byte[] Buffer { get; set; }

        public Packet(byte[] buffer) {
            Buffer = buffer;
        }

        public T Cast<T>() where T : Packet {
            return (T)Activator.CreateInstance(typeof(T), new object[] { Buffer });
        }

        public byte[] Serialize() {
            var attribute = GetType().GetCustomAttribute<RegisterPacketID>();
            if (attribute == null) throw new Exception(GetType().FullName + " must have the [RegisterPacketID(int)] attribute.");

            RaknetWriter writer = new RaknetWriter();
            writer.WriteU8((byte)attribute.ID);
            Serialize(writer);
            return writer.GetRawPayload();
        }

        public void Deserialize() {
            if (Buffer == null) throw new Exception("Buffer is not present");

            RaknetReader reader = new RaknetReader(Buffer);
            reader.ReadU8(); // Skip packet ID
            Deserialize(reader);
        }

        protected abstract void Serialize(RaknetWriter writer);
        protected abstract void Deserialize(RaknetReader reader);
    }

    public enum PacketID
    {
        ConnectedPing = 0x00,
        UnconnectedPing1 = 0x01,
        UnconnectedPing2 = 0x02,
        ConnectedPong = 0x03,
        OpenConnectionRequest1 = 0x05,
        OpenConnectionReply1 = 0x06,
        OpenConnectionRequest2 = 0x07,
        OpenConnectionReply2 = 0x08,
        ConnectionRequest = 0x09,
        ConnectionRequestAccepted = 0x10,
        AlreadyConnected = 0x12,
        NewIncomingConnection = 0x13,
        Disconnect = 0x15,
        UnconnectedPong = 0x1c,
        IncompatibleProtocolVersion = 0x19,
        FrameSetPacketBegin = 0x80,
        FrameSetPacketEnd = 0x8d,
        Nack = 0xa0,
        Ack = 0xc0,
        Game = 0xfe,
    }
}
