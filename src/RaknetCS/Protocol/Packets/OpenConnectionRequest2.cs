using RaknetCS.Protocol.Raknet;
using System.Net;

namespace RaknetCS.Protocol.Packets
{
    [RegisterPacketID((int)PacketID.OpenConnectionRequest2)]
    public class OpenConnectionRequest2 : Packet
    {
        public bool magic;
        public bool server_has_security;
        public uint cookie;
        public bool client_supports_security;
        public IPEndPoint address;
        public ushort mtu;
        public ulong guid;

        public OpenConnectionRequest2(byte[] buffer) : base(buffer) { }

        public OpenConnectionRequest2(bool magic, bool server_has_security, uint cookie, bool client_supports_security, IPEndPoint address, ushort mtu, ulong guid) : base(new byte[] { })
        {
            this.magic = magic;
            this.server_has_security = server_has_security;
            this.cookie = cookie;
            this.client_supports_security = client_supports_security;
            this.address = address;
            this.mtu = mtu;
            this.guid = guid;
        }

        protected override void Serialize(RaknetWriter cursor)
        {
            cursor.WriteMagic();
            if (server_has_security) {
                cursor.WriteU32(cookie, Endian.Big);
                cursor.WriteU8((byte)(client_supports_security ? 1 : 0));
            }
            cursor.WriteAddress(address);
            cursor.WriteU16(mtu, Endian.Big);
            cursor.WriteU64(guid, Endian.Big);
        }

        protected override void Deserialize(RaknetReader cursor)
        {
            magic = cursor.ReadMagic();
            if (server_has_security) {
                cookie = cursor.ReadU32(Endian.Big);
                client_supports_security = cursor.ReadU8() != 0;
            }
            address = cursor.ReadAddress();
            mtu = cursor.ReadU16(Endian.Big);
            guid = cursor.ReadU64(Endian.Big);
        }
    }
}
