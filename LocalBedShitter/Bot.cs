using System.Numerics;
using LocalBedShitter.API;
using LocalBedShitter.API.Players;
using LocalBedShitter.Networking;
using LocalBedShitter.Networking.Packets;

namespace LocalBedShitter;

public sealed class Bot : IPacketProcessor
{
    public readonly NetworkManager Manager;
    public readonly PlayerManager PlayerManager;
    public readonly LocalPlayer LocalPlayer;
    public readonly HashSet<BotCommand> Commands = [];

    public Bot(NetworkManager manager, string username, string mpPass)
    {
        Manager = manager;
        manager.Processors.Add(this);

        PlayerManager = new PlayerManager(manager);

        LocalPlayer = new LocalPlayer(manager, username);
        LocalPlayer.Authenticate(mpPass);

        Commands.Add(new BotCommand("tp", 1, async (player, args) =>
        {
            RemotePlayer? targetPlayer = PlayerManager.Players.FirstOrDefault(p =>
                p.Username.Equals(args[0], StringComparison.InvariantCultureIgnoreCase));
            if (targetPlayer == null)
            {
                LocalPlayer.SendMessage($"{player.Username}: No such player as '{args[0]}'");
                return;
            }

            LocalPlayer.Teleport(targetPlayer);
            LocalPlayer.SendMessage($"{player.Username}: Teleported to {targetPlayer.Username}");
        }));
        Commands.Add(new BotCommand("tp", 3, async (player, args) =>
        {
            if (!float.TryParse(args[0], out float x) ||
                !float.TryParse(args[1], out float y) ||
                !float.TryParse(args[2], out float z))
            {
                LocalPlayer.SendMessage($"{player.Username}: Invalid location: {args[0]}, {args[1]}, {args[2]}");
                return;
            }

            LocalPlayer.Teleport(new Vector3(x, y, z), Vector2.Zero);
            LocalPlayer.SendMessage($"{player.Username}: Teleported to {x}, {y}, {z}");
        }));
        Commands.Add(new BotCommand("setblock", 4, async (player, args) =>
        {
            if (!short.TryParse(args[0], out short x) ||
                !short.TryParse(args[1], out short y) ||
                !short.TryParse(args[2], out short z))
            {
                LocalPlayer.SendMessage($"{player.Username}: Invalid location: {args[0]}, {args[1]}, {args[2]}");
                return;
            }

            if (!byte.TryParse(args[3], out byte type))
            {
                LocalPlayer.SendMessage($"{player.Username}: Invalid block ID: {args[3]}");
                return;
            }

            float dx = LocalPlayer.Position.X - x;
            float dy = LocalPlayer.Position.Y - y;
            float dz = LocalPlayer.Position.Z - z;
            if (dx * dx + dy * dy + dz * dz >= 125)
            {
                LocalPlayer.Teleport(new Vector3(x, y, z + 1), Vector2.Zero);
            }
            
            LocalPlayer.SetBlock(new BlockPos(x, y, z), type != 0 ? EditMode.Create : EditMode.Destroy, type);
            LocalPlayer.SendMessage($"{player.Username}: Placed block with ID {type} at {x}, {y}, {z}");
        }));

        PlayerManager.Message += OnMessage;
    }

    private void OnMessage(RemotePlayer player, string content)
    {
        int colonIndex = content.IndexOf(':');

        if (colonIndex < 0) return;
        string actualContent = content[(colonIndex + 1)..].Trim();

        if (!actualContent.StartsWith('^')) return;
        string[] split = actualContent.Split(' ');

        if (split.Length < 1) return;
        string name = split[0][1..];
        foreach (BotCommand command in Commands
                     .Where(c => c.Name == name && (c.ArgumentCount == split.Length - 1 || c.ArgumentCount < 0)))
        {
            Task.Run(async () => await command.InvokeAsync(player, split.Length == 1 ? [] : split[1..]));
            break;
        }
    }

    public async Task RunAsync()
    {
        while (true)
        {
            await Manager.PollAsync();
        }
    }

    public void ProcessPacket(ref readonly IPacket packet)
    {
    }
}