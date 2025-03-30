// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Numerics;
using LocalBedShitter;
using LocalBedShitter.Packets;
using LocalBedShitter.Packets.C2S;
using LocalBedShitter.Packets.S2C;

using TcpClient client = new(AddressFamily.InterNetwork);
client.Connect(IPAddress.Parse("92.119.126.203"), 65535);

await using NetworkManager manager = new(client);
manager.SendPacket(new PlayerIdC2SPacket("localbedshitter", "9be213a6bd85dd02ce7dd56bec0d9e20"));
// manager.SendPacket(new MessageC2SPacket("i'm ready to shit the bed"));
manager.SendPacket(new TeleportC2SPacket(new Vector3(512.0f, 42.0f + 1.59375f, 522.0f), new Vector2(45.0f, 45.0f)));

HashSet<(sbyte player, Vector3 pos)> players = [];

while (true)
{
    while (manager.TryReceivePacket(out IPacket? packet))
    {
        switch (packet)
        {
            case SpawnPlayerS2CPacket spawn:
                players.Add((spawn.PlayerId, spawn.Position));
                Console.WriteLine($"spawned {spawn.PlayerId}");
                break;
            case MessageS2CPacket message:
                Console.WriteLine(message.SanitizedContent);
                break;
        }
    }

    await Task.Delay(10);
}
