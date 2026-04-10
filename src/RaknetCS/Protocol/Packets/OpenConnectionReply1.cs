using RaknetCS.Protocol.Raknet;

namespace RaknetCS.Protocol.Packets
{
    [RegisterPacketID((int)PacketID.OpenConnectionReply1)]
    public class OpenConnectionReply1 : Packet
    {
        public bool magic;
        public ulong guid;
        public byte use_encryption;
        public uint cookie;
        public ushort mtu_size;

        public OpenConnectionReply1(byte[] buffer) : base(buffer) { }

        public OpenConnectionReply1(bool magic, ulong guid, byte use_encryption, uint cookie, ushort mtu_size) : base(new byte[] { })
        {
            this.magic = magic;
            this.guid = guid;
            this.use_encryption = use_encryption;
            this.cookie = cookie;
            this.mtu_size = mtu_size;
        }

        protected override void Serialize(RaknetWriter cursor)
        {
            cursor.WriteMagic();
            cursor.WriteU64(guid, Endian.Big);
            cursor.WriteU8(use_encryption);
            if (use_encryption != 0) {
                cursor.WriteU32(cookie, Endian.Big);
            }
            cursor.WriteU16(mtu_size, Endian.Big);
        }

        protected override void Deserialize(RaknetReader cursor)
        {
            magic = cursor.ReadMagic();
            guid = cursor.ReadU64(Endian.Big);
            use_encryption = cursor.ReadU8();
            if (use_encryption != 0) {
                cookie = cursor.ReadU32(Endian.Big);
            }
            mtu_size = cursor.ReadU16(Endian.Big);
        }
    }
}
