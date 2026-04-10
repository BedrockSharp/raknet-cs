using RaknetCS.Protocol.Raknet;

namespace RaknetCS.Protocol.Packets
{
    [RegisterPacketID((int)PacketID.IncompatibleProtocolVersion)]
    public class IncompatibleProtocolVersion : Packet
    {
        public byte server_protocol;
        public bool magic;
        public ulong server_guid;

        public IncompatibleProtocolVersion(byte[] buffer) : base(buffer) { }

        public IncompatibleProtocolVersion(byte server_protocol, bool magic, ulong server_guid) : base(new byte[] { })
        {
            this.server_protocol = server_protocol;
            this.magic = magic;
            this.server_guid = server_guid;
        }

        protected override void Serialize(RaknetWriter cursor)
        {
            cursor.WriteU8(server_protocol);
            cursor.WriteMagic();
            cursor.WriteU64(server_guid, Endian.Big);
        }

        protected override void Deserialize(RaknetReader cursor)
        {
            server_protocol = cursor.ReadU8();
            magic = cursor.ReadMagic();
            server_guid = cursor.ReadU64(Endian.Big);
        }
    }
}
