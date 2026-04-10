using RaknetCS.Protocol.Raknet;

namespace RaknetCS.Protocol.Packets
{
    [RegisterPacketID((int)PacketID.ConnectedPong)]
    public class ConnectedPong : Packet
    {
        public long client_timestamp;
        public long server_timestamp;

        public ConnectedPong(byte[] buffer) : base(buffer) { }

        public ConnectedPong(long client_timestamp, long server_timestamp) : base(new byte[] { })
        {
            this.client_timestamp = client_timestamp;
            this.server_timestamp = server_timestamp;
        }

        protected override void Serialize(RaknetWriter cursor)
        {
            cursor.WriteI64(client_timestamp, Endian.Big);
            cursor.WriteI64(server_timestamp, Endian.Big);
        }

        protected override void Deserialize(RaknetReader cursor)
        {
            client_timestamp = cursor.ReadI64(Endian.Big);
            server_timestamp = cursor.ReadI64(Endian.Big);
        }
    }
}
