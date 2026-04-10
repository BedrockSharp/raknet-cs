using RaknetCS.Protocol.Raknet;
using System.Net;

namespace RaknetCS.Protocol.Packets
{
    [RegisterPacketID((int)PacketID.ConnectionRequestAccepted)]
    public class ConnectionRequestAccepted : Packet
    {
        public IPEndPoint client_address;
        public ushort system_index;
        public long request_timestamp;
        public long accepted_timestamp;

        public ConnectionRequestAccepted(byte[] buffer) : base(buffer) { }

        public ConnectionRequestAccepted(IPEndPoint client_address, ushort system_index, long request_timestamp, long accepted_timestamp) : base(new byte[] { })
        {
            this.client_address = client_address;
            this.system_index = system_index;
            this.request_timestamp = request_timestamp;
            this.accepted_timestamp = accepted_timestamp;
        }

        protected override void Serialize(RaknetWriter cursor)
        {
            cursor.WriteAddress(client_address);
            cursor.WriteU16(system_index, Endian.Big);
            for (int i = 0; i < 10; i++)
            {
                IPEndPoint tmpEndpoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 19132); // I know this is the stupidest way to put IP addresses
                cursor.WriteAddress(tmpEndpoint);
            }

            cursor.WriteI64(request_timestamp, Endian.Big);
            cursor.WriteI64(accepted_timestamp, Endian.Big);
        }

        protected override void Deserialize(RaknetReader cursor)
        {
            client_address = cursor.ReadAddress();
            system_index = cursor.ReadU16(Endian.Big);
            for (int i = 0; i < 10; i++)
            {
                cursor.ReadAddress();
            }
            request_timestamp = cursor.ReadI64(Endian.Big);
            accepted_timestamp = cursor.ReadI64(Endian.Big);
        }
    }
}
