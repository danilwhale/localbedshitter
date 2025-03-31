using System.Numerics;
using System.Text;
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
    
    private readonly Stack<BlockJob> _jobs = [];
    
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
        Commands.Add(new BotCommand("fill", 7, async (player, args) =>
        {
            if (!short.TryParse(args[0], out short x0) ||
                !short.TryParse(args[1], out short y0) ||
                !short.TryParse(args[2], out short z0) ||
                !short.TryParse(args[3], out short x1) ||
                !short.TryParse(args[4], out short y1) ||
                !short.TryParse(args[5], out short z1))
            {
                LocalPlayer.SendMessage($"{player.Username}: Invalid location");
                return;
            }

            if (!byte.TryParse(args[6], out byte type))
            {
                LocalPlayer.SendMessage($"{player.Username}: Invalid block ID: {type}");
                return;
            }

            short minX = Math.Min(x0, x1), minY = Math.Min(y0, y1), minZ = Math.Min(z0, z1);
            short maxX = Math.Max(x0, x1), maxY = Math.Max(y0, y1), maxZ = Math.Max(z0, z1);

            _jobs.Push(new BlockJob(new BlockPos(minX, minY, minZ), new BlockPos(maxX, maxY, maxZ), type));
            
            LocalPlayer.SendMessage($"{player.Username}: Enqueued job to fill " +
                                    $"{(maxX - minX) * (maxY - minY) * (maxZ - minZ)} " +
                                    $"blocks with {type}");
        }));
        Commands.Add(new BotCommand("jobs", 1, async (player, args) =>
        {
            switch (args[0])
            {
                case "list":
                    if (_jobs.Count == 0)
                    {
                        LocalPlayer.SendMessage($"{player.Username}: There are no jobs to do");
                        return;
                    }
            
                    LocalPlayer.SendMessage($"{player.Username}: There are {_jobs.Count} job(s) to do");
                    break;
            }
        }));

        PlayerManager.Message += OnMessage;

        Task.Run(ExecuteJobsAsync);
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

    private async Task ExecuteJobsAsync()
    {
        while (true)
        {
            while (_jobs.TryPop(out BlockJob? job))
            {
                await job.ExecuteAsync(LocalPlayer, 28);
                LocalPlayer.SendMessage($"Finished filling {job.Min} {job.Max} with {job.Type}");
            }
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