using RaknetCS.Protocol.Raknet;

namespace RaknetCS.Protocol.Packets
{
    [RegisterPacketID((int)PacketID.ConnectedPing)]
    public class ConnectedPing : Packet
    {
        public long client_timestamp;

        public ConnectedPing(byte[] buffer) : base(buffer) { }

        public ConnectedPing(long client_timestamp) : base(new byte[] { })
        {
            this.client_timestamp = client_timestamp;
        }

        protected override void Serialize(RaknetWriter cursor)
        {
            cursor.WriteI64(client_timestamp, Endian.Big);
        }

        protected override void Deserialize(RaknetReader cursor)
        {
            client_timestamp = cursor.ReadI64(Endian.Big);
        }
    }
}
