<p align="center">
  <h1 align="center">RaknetCS</h1>
  <p align="center"><i>A high-performance, modern .NET implementation of the RakNet protocol.</i></p>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-9.0-512bd4?style=for-the-badge&logo=.net" alt=".NET 9.0">
  <img src="https://img.shields.io/badge/License-MIT-green?style=for-the-badge" alt="License MIT">
  <img src="https://img.shields.io/github/actions/workflow/status/BedrockSharp/raknet-cs/build.yml?style=for-the-badge" alt="Build Status">
  <img src="https://img.shields.io/badge/PRs-welcome-cyan?style=for-the-badge" alt="PRs Welcome">
</p>

**RaknetCS** is a performance-oriented, modernized C# implementation of the RakNet protocol. Designed for the high demands of game networking (particularly Minecraft Bedrock Edition), it provides a robust, reliable UDP transport layer with advanced packet handling and session management.

---

## 🚀 Features

- ✅ **Modern Architecture**: Built for .NET 9.0+.
- ✅ **Reliability Layers**: Supports Unreliable, Reliable, and Ordered packet delivery.
- ✅ **High Performance**: Asynchronous socket operations for minimum latency.
- ✅ **Clean API**: Intuitive classes for both Listeners (Servers) and Clients.
- ✅ **Packet Attribute System**: Easy packet registration and dispatching.
- ✅ **Fragmentation & Reassembly**: Automatically handles large data packets.

## 🛠️ Getting Started

### Installation
Clone the repository and build the project:
```bash
git clone https://github.com/BedrockSharp/raknet-cs.git
cd raknet-cs
dotnet build
```

### Running the Example Server
To see the project in action and see how it looks in the Minecraft server list:
```bash
# From the root directory
dotnet run --project src/RaknetCS.Example
```
This will start a server on port `19132`. Open Minecraft Bedrock, go to the **Friends** tab, and you should see the **"RaknetCS Demo Server"** under **Local Games**.

### Server Code Snippet
```csharp
using System.Net;
using RaknetCS.Network;

var listener = new RaknetListener(new IPEndPoint(IPAddress.Any, 19132));
listener.Motd = "MCPE;My Awesome Server;..."; // Set your custom MOTD

listener.SessionConnected += (session) => {
    Console.WriteLine($"New session from: {session.PeerEndPoint}");
};

listener.BeginListener();
```

### Client Example (RaknetClient)
```csharp
using System.Net;
using RaknetCS.Network;

var client = new RaknetClient();
client.SessionEstablished += (session) => {
    Console.WriteLine("Connected to server!");
};

client.BeginConnection(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 19132));
```

## 📦 Defining Custom Packets

Use the attribute-based system to register your packets effortlessly:

```csharp
[RegisterPacketID(0xFE)]
public class MyCustomPacket : Packet {
    public string Message;

    public MyCustomPacket(byte[] buffer) : base(buffer) {}

    protected override void Serialize(RaknetWriter stream) {
        stream.WriteString(Message);
    }

    protected override void Deserialize(RaknetReader stream) {
        Message = stream.ReadString();
    }
}
```

## 🤝 Contributing

Contributions are what make the open-source community such an amazing place!
1. **Fork** the project.
2. **Create** your Feature Branch (`git checkout -b feature/AmazingFeature`).
3. **Commit** your changes (`git commit -m 'Add some AmazingFeature'`).
4. **Push** to the Branch (`git push origin feature/AmazingFeature`).
5. **Open** a Pull Request.

Please see [CONTRIBUTING.md](CONTRIBUTING.md) for details.

## 📄 License

Distributed under the MIT License. See [LICENSE](LICENSE) for more information.

---
<p align="center">
  Maintained with ❤️ by the BedrockSharp team.
</p>
