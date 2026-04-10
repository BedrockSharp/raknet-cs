using RaknetCS.Protocol.Raknet;
using System.Net;

namespace RaknetCS.Protocol.Packets
{
    [RegisterPacketID((int)PacketID.NewIncomingConnection)]
    public class NewIncomingConnection : Packet
    {
        public IPEndPoint server_address;
        public long request_timestamp;
        public long accepted_timestamp;

        public NewIncomingConnection(byte[] buffer) : base(buffer) { }

        public NewIncomingConnection(IPEndPoint server_address, long request_timestamp, long accepted_timestamp) : base(new byte[] { })
        {
            this.server_address = server_address;
            this.request_timestamp = request_timestamp;
            this.accepted_timestamp = accepted_timestamp;
        }

        protected override void Serialize(RaknetWriter cursor)
        {
            cursor.WriteAddress(server_address);
            IPEndPoint tmpAddress = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 0);
            for (int i = 0; i < 10; i++)
            {
                cursor.WriteAddress(tmpAddress);
            }
            cursor.WriteI64(request_timestamp, Endian.Big);
            cursor.WriteI64(accepted_timestamp, Endian.Big);
        }

        protected override void Deserialize(RaknetReader cursor)
        {
            server_address = cursor.ReadAddress();
            for (int i = 0; i < 10; i++)
            {
                cursor.ReadAddress();
            }
            request_timestamp = cursor.ReadI64(Endian.Big);
            accepted_timestamp = cursor.ReadI64(Endian.Big);
        }
    }
}
