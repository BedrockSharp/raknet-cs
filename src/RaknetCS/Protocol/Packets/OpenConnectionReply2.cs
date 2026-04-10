using RaknetCS.Protocol.Raknet;
using System.Net;

namespace RaknetCS.Protocol.Packets
{
    [RegisterPacketID((int)PacketID.OpenConnectionReply2)]
    public class OpenConnectionReply2 : Packet
    {
        public bool magic;
        public ulong guid;
        public IPEndPoint address;
        public ushort mtu;
        public byte encryption_enabled;

        public OpenConnectionReply2(byte[] buffer) : base(buffer) { }

        public OpenConnectionReply2(bool magic, ulong guid, IPEndPoint address, ushort mtu, byte encryption_enabled) : base(new byte[] { })
        {
            this.magic = magic;
            this.guid = guid;
            this.address = address;
            this.mtu = mtu;
            this.encryption_enabled = encryption_enabled;
        }

        protected override void Serialize(RaknetWriter cursor)
        {
            cursor.WriteMagic();
            cursor.WriteU64(guid, Endian.Big);
            cursor.WriteAddress(address);
            cursor.WriteU16(mtu, Endian.Big);
            cursor.WriteU8(encryption_enabled);
        }

        protected override void Deserialize(RaknetReader cursor)
        {
            magic = cursor.ReadMagic();
            guid = cursor.ReadU64(Endian.Big);
            address = cursor.ReadAddress();
            mtu = cursor.ReadU16(Endian.Big);
            encryption_enabled = cursor.ReadU8();
        }
    }
}
