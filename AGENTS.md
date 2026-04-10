
## AI Agent Guidelines

### Repository Context
This repository is a modernized C# implementation of the Raknet protocol (`RaknetCS`).

### Architecture
- **Source**: Located in `src/RaknetCS/`.
- **Framework**: Targeting .NET 9.0.
- **Namespaces**: All code resides within the `RaknetCS` root namespace.

### Development Standards
- **Packet Definition**: Use the `[RegisterPacketID(ID)]` attribute on packet classes.
- **Serialization**: Packets should implement `Serialize()` and `Deserialize()` methods.
- **Network Logic**: Use `AsyncUdpClient` for low-level socket operations and `RaknetSession` for protocol-level state.

### Useful Commands
- **Build**: `dotnet build RaknetCS.sln`
- **Restore**: `dotnet restore RaknetCS.sln`

### Refactoring Rules
- Prefer modern C# features (.NET 9+).
- Maintain compatibility with the Raknet protocol.
- When adding new packets, ensure they are registered in the appropriate `Network` handlers.
