using RaknetCS.Protocol.Raknet;

namespace RaknetCS.Protocol.Packets
{
    [RegisterPacketID((int)PacketID.AlreadyConnected)]
    public class AlreadyConnected : Packet
    {
        public bool magic;
        public ulong guid;

        public AlreadyConnected(byte[] buffer) : base(buffer) { }

        public AlreadyConnected(bool magic, ulong guid) : base(new byte[] { })
        {
            this.magic = magic;
            this.guid = guid;
        }

        protected override void Serialize(RaknetWriter cursor)
        {
            cursor.WriteMagic();
            cursor.WriteU64(guid, Endian.Big);
        }

        protected override void Deserialize(RaknetReader cursor)
        {
            magic = cursor.ReadMagic();
            guid = cursor.ReadU64(Endian.Big);
        }
    }
}
