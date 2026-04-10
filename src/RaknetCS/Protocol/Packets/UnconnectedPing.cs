using RaknetCS.Protocol.Raknet;

namespace RaknetCS.Protocol.Packets {

    [RegisterPacketID((int)PacketID.UnconnectedPing1)]
    public class UnconnectedPing : Packet {
        public long time;
        public bool magic;
        public ulong guid;

        public UnconnectedPing(byte[] buffer) : base(buffer) {}
        public UnconnectedPing(long time, bool magic, ulong guid) : base(new byte[] {}) {
            this.time = time;
            this.magic = magic;
            this.guid = guid;
        }

        protected override void Serialize(RaknetWriter stream) {
            stream.WriteI64(time, Endian.Big);
            stream.WriteMagic();
            stream.WriteU64(guid, Endian.Big);
        }

        protected override void Deserialize(RaknetReader stream)
        {
            time = stream.ReadI64(Endian.Big);
            magic = stream.ReadMagic();
            guid = stream.ReadU64(Endian.Big);
        }
    }
}
