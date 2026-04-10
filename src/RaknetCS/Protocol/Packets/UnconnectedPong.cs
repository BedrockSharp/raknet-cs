using RaknetCS.Protocol.Raknet;

namespace RaknetCS.Protocol.Packets
{
    [RegisterPacketID((int)PacketID.UnconnectedPong)]
    public class UnconnectedPong : Packet
    {
        public long time;
        public ulong guid;
        public bool magic;
        public string motd = "";

        public UnconnectedPong(byte[] buffer) : base(buffer) { }

        public UnconnectedPong(long time, ulong guid, bool magic, string motd) : base(new byte[] { })
        {
            this.time = time;
            this.guid = guid;
            this.magic = magic;
            this.motd = motd;
        }

        protected override void Serialize(RaknetWriter cursor)
        {
            cursor.WriteI64(time, Endian.Big);
            cursor.WriteU64(guid, Endian.Big);
            cursor.WriteMagic();
            cursor.WriteString(motd);
        }

        protected override void Deserialize(RaknetReader cursor)
        {
            time = cursor.ReadI64(Endian.Big);
            guid = cursor.ReadU64(Endian.Big);
            magic = cursor.ReadMagic();
            motd = cursor.ReadString();
        }
    }
}
