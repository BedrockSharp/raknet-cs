using RaknetCS.Protocol.Raknet;
using System.Collections.Generic;

namespace RaknetCS.Protocol.Packets
{
    [RegisterPacketID((int)PacketID.Ack)]
    public class Ack : Packet
    {
        public ushort record_count;
        public List<AckRange> sequences = new List<AckRange>();

        public Ack(byte[] buffer) : base(buffer) { }

        public Ack(ushort record_count, List<AckRange> sequences) : base(new byte[] { })
        {
            this.record_count = record_count;
            this.sequences = sequences;
        }

        protected override void Serialize(RaknetWriter cursor)
        {
            cursor.WriteSequences(sequences);
        }

        protected override void Deserialize(RaknetReader cursor)
        {
            record_count = cursor.ReadU16(Endian.Big);
            sequences = cursor.ReadSequences(record_count);
        }
    }
}
