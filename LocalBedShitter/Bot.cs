using System.Numerics;
using System.Text;
using LocalBedShitter.API;
using LocalBedShitter.API.Players;
using LocalBedShitter.Jobs;
using LocalBedShitter.Networking;
using LocalBedShitter.Networking.Packets;

namespace LocalBedShitter;

public sealed class Bot : IPacketProcessor
{
    public readonly NetworkManager Manager;
    public readonly PlayerManager PlayerManager;
    public readonly LocalPlayer LocalPlayer;
    public readonly HashSet<BotCommand> Commands = [];

    private readonly JobPool _jobs = new();

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
                LocalPlayer.SendMessage($"{player.Username}: Invalid location: [{args[0]}, {args[1]}, {args[2]}]");
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
            LocalPlayer.SendMessage($"{player.Username}: Placed block with ID {type} at [{x}, {y}, {z}]");
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
                LocalPlayer.SendMessage(
                    $"{player.Username}: Invalid location: [{args[0]}, {args[1]}, {args[2]}]:[{args[3]}, {args[4]}, {args[5]}]");
                return;
            }

            if (!byte.TryParse(args[6], out byte type))
            {
                LocalPlayer.SendMessage($"{player.Username}: Invalid block ID: {type}");
                return;
            }

            short minX = Math.Min(x0, x1), minY = Math.Min(y0, y1), minZ = Math.Min(z0, z1);
            short maxX = Math.Max(x0, x1), maxY = Math.Max(y0, y1), maxZ = Math.Max(z0, z1);

            _jobs.Add(FillNodeJob.CreateFill(new BlockPos(minX, minY, minZ), new BlockPos(maxX, maxY, maxZ), type));

            LocalPlayer.SendMessage($"{player.Username}: Enqueued job to fill " +
                                    $"{(maxX - minX + 1) * (maxY - minY + 1) * (maxZ - minZ + 1)} " +
                                    $"blocks with {type}");
        }));
        Commands.Add(new BotCommand("sphere", 5, async (player, args) =>
        {
            if (!short.TryParse(args[0], out short x) ||
                !short.TryParse(args[1], out short y) ||
                !short.TryParse(args[2], out short z))
            {
                LocalPlayer.SendMessage($"{player.Username}: Invalid location: [{args[0]}, {args[1]}, {args[2]}]");
                return;
            }

            if (!short.TryParse(args[3], out short radius))
            {
                LocalPlayer.SendMessage($"{player.Username}: Invalid radius: {args[3]}");
                return;
            }

            if (!byte.TryParse(args[4], out byte type))
            {
                LocalPlayer.SendMessage($"{player.Username}: Invalid block ID: {args[4]}");
                return;
            }

            _jobs.Add(new SphereJob(new BlockPos(x, y, z), radius, type));
            LocalPlayer.SendMessage($"{player.Username}: Enqueued job to place a sphere");
        }));
        Commands.Add(new BotCommand("veryeasy", 3, async (player, args) =>
        {
            if (!short.TryParse(args[0], out short x) ||
                !short.TryParse(args[1], out short y) ||
                !short.TryParse(args[2], out short z))
            {
                LocalPlayer.SendMessage($"{player.Username}: Invalid location: [{args[0]}, {args[1]}, {args[2]}]");
                return;
            }
            
            _jobs.Add(new VeryEasyJob(new BlockPos(x, y, z)));
            LocalPlayer.SendMessage($"{player.Username}: Enqueued job to place its very easy");
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
                case "clear":
                    _jobs.Clear();
                    LocalPlayer.SendMessage($"{player.Username}: Removed all pending jobs");
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
            while (_jobs.TryRemove(out Job? job))
            {
                await job.ExecuteAsync(LocalPlayer);
                LocalPlayer.SendMessage($"Finished {job}. {_jobs.Count} remaining jobs");
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