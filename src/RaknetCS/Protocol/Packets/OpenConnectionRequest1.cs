using RaknetCS.Protocol.Raknet;

namespace RaknetCS.Protocol.Packets
{
    [RegisterPacketID((int)PacketID.OpenConnectionRequest1)]
    public class OpenConnectionRequest1 : Packet
    {
        public bool magic;
        public byte protocol_version;
        public ushort mtu_size;

        public OpenConnectionRequest1(byte[] buffer) : base(buffer) { }

        public OpenConnectionRequest1(bool magic, byte protocol_version, ushort mtu_size) : base(new byte[] { })
        {
            this.magic = magic;
            this.protocol_version = protocol_version;
            this.mtu_size = mtu_size;
        }

        protected override void Serialize(RaknetWriter cursor)
        {
            cursor.WriteMagic();
            cursor.WriteU8(protocol_version);
            cursor.Write(new byte[mtu_size - 46]);
        }

        protected override void Deserialize(RaknetReader cursor)
        {
            magic = cursor.ReadMagic();
            protocol_version = cursor.ReadU8();
            mtu_size = (ushort)(Buffer.Length + 28);
        }
    }
}
