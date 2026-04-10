using RaknetCS.Protocol.Raknet;

namespace RaknetCS.Protocol.Packets
{
    [RegisterPacketID((int)PacketID.ConnectionRequest)]
    public class ConnectionRequest : Packet
    {
        public ulong guid;
        public long time;
        public byte use_encryption;

        public ConnectionRequest(byte[] buffer) : base(buffer) { }

        public ConnectionRequest(ulong guid, long time, byte use_encryption) : base(new byte[] { })
        {
            this.guid = guid;
            this.time = time;
            this.use_encryption = use_encryption;
        }

        protected override void Serialize(RaknetWriter cursor)
        {
            cursor.WriteU64(guid, Endian.Big);
            cursor.WriteI64(time, Endian.Big);
            cursor.WriteU8(use_encryption);
        }

        protected override void Deserialize(RaknetReader cursor)
        {
            guid = cursor.ReadU64(Endian.Big);
            time = cursor.ReadI64(Endian.Big);
            use_encryption = cursor.ReadU8();
        }
    }
}
