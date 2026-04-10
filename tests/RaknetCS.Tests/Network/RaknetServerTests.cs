using RaknetCS.Network;
using RaknetCS.Protocol;
using RaknetCS.Protocol.Raknet;
using RaknetCS.Protocol.Packets;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System;
using Xunit;

namespace RaknetCS.Tests.Network
{
    public class RaknetServerTests
    {
        [Fact]
        public async Task TestUnconnectedPingPong()
        {
            var serverAddress = new IPEndPoint(IPAddress.Loopback, 0);
            var listener = new RaknetListener(serverAddress);
            
            var actualPort = ((IPEndPoint)listener.Socket.Socket.Client.LocalEndPoint!).Port;
            var actualAddress = new IPEndPoint(IPAddress.Loopback, actualPort);

            string expectedMotd = $"MCPE;RaknetCS;527;1.19.1;0;10;12345678;Bedrock;Survival;1;{actualPort};{actualPort + 1};";
            listener.Motd = expectedMotd;

            _ = Task.Run(() => listener.BeginListener());

            try
            {
                using var udpClient = new UdpClient();
                
                var ping = new UnconnectedPing(1337, true, 8888);
                byte[] pingBytes = ping.Serialize();
                await udpClient.SendAsync(pingBytes, pingBytes.Length, actualAddress);

                var receiveTask = udpClient.ReceiveAsync();
                if (await Task.WhenAny(receiveTask, Task.Delay(2000)) == receiveTask)
                {
                    var result = await receiveTask;
                    var pong = new UnconnectedPong(result.Buffer);
                    pong.Deserialize();

                    Assert.Equal(ping.time, pong.time);
                    Assert.True(pong.magic);
                    Assert.Equal(expectedMotd, pong.motd);
                }
                else
                {
                    Assert.Fail("Timed out waiting for Unconnected Pong");
                }
            }
            finally
            {
                listener.StopListener();
            }
        }

        [Fact]
        public async Task TestFullConnectionHandshake()
        {
            var serverAddress = new IPEndPoint(IPAddress.Loopback, 0);
            var listener = new RaknetListener(serverAddress);
            
            var actualPort = ((IPEndPoint)listener.Socket.Socket.Client.LocalEndPoint!).Port;
            var actualAddress = new IPEndPoint(IPAddress.Loopback, actualPort);

            RaknetSession serverSession = null;
            listener.SessionConnected += (session) => 
            {
                serverSession = session;
            };

            _ = Task.Run(() => listener.BeginListener());

            try
            {
                var client = new RaknetClient();
                RaknetSession clientSession = null;
                client.SessionEstablished += (session) =>
                {
                    clientSession = session;
                };

                client.BeginConnection(actualAddress);

                int attempts = 0;
                while ((clientSession == null || serverSession == null) && attempts < 50)
                {
                    await Task.Delay(100);
                    attempts++;
                }

                Assert.NotNull(clientSession);
                Assert.NotNull(serverSession);
                Assert.True(clientSession.Connected);
                Assert.True(serverSession.Connected);

                client.EndConnection();
            }
            finally
            {
                listener.StopListener();
            }
        }
    }
}
