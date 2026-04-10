using System.Net;
using RaknetCS.Network;

Console.WriteLine("========================================");
Console.WriteLine("      RaknetCS Server Demo Example      ");
Console.WriteLine("========================================");

const int port = 19132;
var endpoint = new IPEndPoint(IPAddress.Any, port);
var listener = new RaknetListener(endpoint);

const int protocol = 944;
const string versionStr = "1.26.0";

listener.Motd = $"MCPE;Bedrock Server;{protocol};{versionStr};0;10;{DateTimeOffset.Now.ToUnixTimeMilliseconds()};Powered by RaknetCS;Survival;1;{port};{port + 1};";

Console.WriteLine($"[INFO] Server listening on {endpoint}");
Console.WriteLine("[INFO] You should see this server in your local 'Friends' tab in Minecraft Bedrock.");
Console.WriteLine("[INFO] Press Ctrl+C to stop.");

listener.SessionConnected += (session) => {
    Console.WriteLine($"[CONN] New session established with: {session.PeerEndPoint}");
    
    session.SessionDisconnected += (s) => {
        Console.WriteLine($"[DISC] Session with {s.PeerEndPoint} disconnected.");
    };
};

try {
    listener.BeginListener();
    
    while (true) {
        Thread.Sleep(1000);
    }
} catch (Exception ex) {
    Console.WriteLine($"[ERROR] {ex.Message}");
} finally {
    listener.StopListener();
    Console.WriteLine("[INFO] Server stopped.");
}
