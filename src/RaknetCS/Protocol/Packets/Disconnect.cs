using RaknetCS.Protocol.Raknet;

namespace RaknetCS.Protocol.Packets
{
    [RegisterPacketID((int)PacketID.Disconnect)]
    public class Disconnect : Packet
    {
        public Disconnect(byte[] buffer) : base(buffer) { }

        public Disconnect() : base(new byte[] { })
        {
        }

        protected override void Serialize(RaknetWriter cursor)
        {
        }

        protected override void Deserialize(RaknetReader cursor)
        {
        }
    }
}
