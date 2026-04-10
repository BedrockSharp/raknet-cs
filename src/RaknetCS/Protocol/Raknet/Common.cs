using System.Net.Sockets;
using System.Net;
using System;

namespace RaknetCS.Protocol.Raknet
{
    public class Common
    {
        public static readonly byte RAKNET_PROTOCOL_VERSION = 0xA;
        public static readonly ushort RAKNET_CLIENT_MTU = 1400;
        public static readonly int RECEIVE_TIMEOUT = 60000;
        public static long CurTimestampMillis() {
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSinceEpoch = DateTime.UtcNow - unixEpoch;
            long milliseconds = (long)timeSinceEpoch.TotalMilliseconds;
            return milliseconds;
        }

        public static UdpClient CreateListener(IPEndPoint endpoint)
        {
            UdpClient listener = new UdpClient();

            if (Environment.OSVersion.Platform != PlatformID.MacOSX)
            {
                listener.Client.ReceiveBufferSize = int.MaxValue;
                listener.Client.SendBufferSize = int.MaxValue;
            }

            listener.DontFragment = false;
            listener.EnableBroadcast = true;

            if (Environment.OSVersion.Platform != PlatformID.Unix && Environment.OSVersion.Platform != PlatformID.MacOSX)
            {
                uint IOC_IN = 0x80000000;
                uint IOC_VENDOR = 0x18000000;
                uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                listener.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
            }

            listener.Client.Bind(endpoint);
            return listener;
        }
    }
}
