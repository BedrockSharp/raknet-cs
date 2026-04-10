using RaknetCS.Protocol;
using RaknetCS.Protocol.Raknet;
using RaknetCS.Protocol.Packets;
using System.Net;
using System.Collections.Generic;
using Xunit;

namespace RaknetCS.Tests.Protocol
{
    public class RaknetProtocolTests
    {
        [Fact]
        public void TestU8()
        {
            var writer = new RaknetWriter();
            writer.WriteU8(0x12);
            
            var reader = new RaknetReader(writer.GetRawPayload());
            Assert.Equal(0x12, reader.ReadU8());
        }

        [Fact]
        public void TestU16()
        {
            var writer = new RaknetWriter();
            writer.WriteU16(0x1234, Endian.Big);
            writer.WriteU16(0x5678, Endian.Little);
            
            var reader = new RaknetReader(writer.GetRawPayload());
            Assert.Equal(0x1234, reader.ReadU16(Endian.Big));
            Assert.Equal(0x5678, reader.ReadU16(Endian.Little));
        }

        [Fact]
        public void TestU24()
        {
            var writer = new RaknetWriter();
            writer.WriteU24(0x123456, Endian.Big);
            writer.WriteU24(0x789ABC, Endian.Little);
            
            var reader = new RaknetReader(writer.GetRawPayload());
            Assert.Equal((uint)0x123456, reader.ReadU24(Endian.Big));
            Assert.Equal((uint)0x789ABC, reader.ReadU24(Endian.Little));
        }

        [Fact]
        public void TestU32()
        {
            var writer = new RaknetWriter();
            writer.WriteU32(0x12345678, Endian.Big);
            writer.WriteU32(0x9ABCDEF0, Endian.Little);
            
            var reader = new RaknetReader(writer.GetRawPayload());
            Assert.Equal(0x12345678u, reader.ReadU32(Endian.Big));
            Assert.Equal(0x9ABCDEF0u, reader.ReadU32(Endian.Little));
        }

        [Fact]
        public void TestU64()
        {
            var writer = new RaknetWriter();
            writer.WriteU64(0x123456789ABCDEF0, Endian.Big);
            writer.WriteU64(0xFEDCBA9876543210, Endian.Little);
            
            var reader = new RaknetReader(writer.GetRawPayload());
            Assert.Equal(0x123456789ABCDEF0u, reader.ReadU64(Endian.Big));
            Assert.Equal(0xFEDCBA9876543210u, reader.ReadU64(Endian.Little));
        }

        [Fact]
        public void TestString()
        {
            var writer = new RaknetWriter();
            writer.WriteString("Hello Raknet!");
            
            var reader = new RaknetReader(writer.GetRawPayload());
            Assert.Equal("Hello Raknet!", reader.ReadString());
        }

        [Fact]
        public void TestMagic()
        {
            var writer = new RaknetWriter();
            writer.WriteMagic();
            
            var reader = new RaknetReader(writer.GetRawPayload());
            Assert.True(reader.ReadMagic());
        }

        [Fact]
        public void TestAddressV4()
        {
            var writer = new RaknetWriter();
            var endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 19132);
            writer.WriteAddress(endpoint);
            
            var reader = new RaknetReader(writer.GetRawPayload());
            var result = reader.ReadAddress();
            
            Assert.Equal(endpoint.Address, result.Address);
            Assert.Equal(endpoint.Port, result.Port);
        }

        [Fact]
        public void TestUnconnectedPingPong()
        {
            var ping = new UnconnectedPing(123456789, true, 987654321);

            var pingBytes = ping.Serialize();
            var reader = new RaknetReader(pingBytes);
            
            Assert.Equal((byte)PacketID.UnconnectedPing1, reader.ReadU8());
            Assert.Equal(ping.time, reader.ReadI64(Endian.Big));
            Assert.True(reader.ReadMagic());
            Assert.Equal(ping.guid, reader.ReadU64(Endian.Big));

            var pong = new UnconnectedPong(123456789, 987654321, true, "MCPE;Test Server;1;2;3;4");

            var pongBytes = pong.Serialize();
            var decodedPong = new UnconnectedPong(pongBytes);
            decodedPong.Deserialize();

            Assert.Equal(pong.time, decodedPong.time);
            Assert.Equal(pong.guid, decodedPong.guid);
            Assert.True(decodedPong.magic);
            Assert.Equal(pong.motd, decodedPong.motd);
        }

        [Fact]
        public void TestAckNackSequences()
        {
            var ack = new Ack(2, new List<AckRange>
            {
                new AckRange(1, 1),
                new AckRange(5, 10)
            });

            var bytes = ack.Serialize();
            var decodedAck = new Ack(bytes);
            decodedAck.Deserialize();

            Assert.Equal(ack.record_count, decodedAck.record_count);
            Assert.Equal(ack.sequences.Count, decodedAck.sequences.Count);
            Assert.Equal(ack.sequences[0].Start, decodedAck.sequences[0].Start);
            Assert.Equal(ack.sequences[0].End, decodedAck.sequences[0].End);
            Assert.Equal(ack.sequences[1].Start, decodedAck.sequences[1].Start);
            Assert.Equal(ack.sequences[1].End, decodedAck.sequences[1].End);
        }

        [Fact]
        public void TestConnectedPingPong()
        {
            var ping = new ConnectedPing(1337);

            var bytes = ping.Serialize();
            var decodedPing = new ConnectedPing(bytes);
            decodedPing.Deserialize();

            Assert.Equal(ping.client_timestamp, decodedPing.client_timestamp);

            var pong = new ConnectedPong(1337, 7331);

            bytes = pong.Serialize();
            var decodedPong = new ConnectedPong(bytes);
            decodedPong.Deserialize();

            Assert.Equal(pong.client_timestamp, decodedPong.client_timestamp);
            Assert.Equal(pong.server_timestamp, decodedPong.server_timestamp);
        }
    }
}
