using System.Diagnostics;
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
    public readonly Level Level;
    public readonly HashSet<BotCommand> Commands = [];

    private readonly JobPool _jobs = new();

    public Bot(NetworkManager manager, string username, string mpPass)
    {
        Manager = manager;
        manager.Processors.Add(this);

        PlayerManager = new PlayerManager(manager);

        LocalPlayer = new LocalPlayer(manager, username);
        LocalPlayer.Authenticate(mpPass);

        Level = new Level(manager);

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
        Commands.Add(new BotCommand("say", -1, async (_, args) =>
        {
            LocalPlayer.SendMessage(string.Join(" ", args));
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
                LocalPlayer.SendMessage($"{player.Username}: Invalid block ID: {args[6]}");
                return;
            }

            short minX = Math.Min(x0, x1), minY = Math.Min(y0, y1), minZ = Math.Min(z0, z1);
            short maxX = Math.Max(x0, x1), maxY = Math.Max(y0, y1), maxZ = Math.Max(z0, z1);

            FillJob job = new(new BlockPos(minX, minY, minZ), new BlockPos(maxX, maxY, maxZ), type);
            _jobs.Add(job);

            LocalPlayer.SendMessage($"{player.Username}: Enqueued a job to fill " +
                                    $"{(maxX - minX + 1) * (maxY - minY + 1) * (maxZ - minZ + 1)} " +
                                    $"blocks with {type}. This will take ~{job.EstimateExecutionTime}");
        }));
        Commands.Add(new BotCommand("replace", 8, async (player, args) =>
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

            if (!byte.TryParse(args[6], out byte source))
            {
                LocalPlayer.SendMessage($"{player.Username}: Invalid source block ID: {args[6]}");
                return;
            }
            
            if (!byte.TryParse(args[7], out byte type))
            {
                LocalPlayer.SendMessage($"{player.Username}: Invalid block ID: {args[6]}");
                return;
            }

            short minX = Math.Min(x0, x1), minY = Math.Min(y0, y1), minZ = Math.Min(z0, z1);
            short maxX = Math.Max(x0, x1), maxY = Math.Max(y0, y1), maxZ = Math.Max(z0, z1);

            ReplaceJob job = new(new BlockPos(minX, minY, minZ), new BlockPos(maxX, maxY, maxZ), source, type);
            _jobs.Add(job);

            LocalPlayer.SendMessage($"{player.Username}: Enqueued a job to replace " +
                                    $"{(maxX - minX + 1) * (maxY - minY + 1) * (maxZ - minZ + 1)} " +
                                    $"blocks of type {source} with {type}. This will take ~{job.EstimateExecutionTime:g}");
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

            SphereJob job = new(new BlockPos(x, y, z), radius, type);
            _jobs.Add(job);
            LocalPlayer.SendMessage($"{player.Username}: Enqueued a job to place the sphere. This will take ~{job.EstimateExecutionTime:g}");
        }));
        Commands.Add(new BotCommand("pyramid", 6, async (player, args) =>
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

            if (!short.TryParse(args[4], out short layers))
            {
                LocalPlayer.SendMessage($"{player.Username}: Invalid layers: {args[4]}");
                return;
            }

            if (!byte.TryParse(args[5], out byte type))
            {
                LocalPlayer.SendMessage($"{player.Username}: Invalid block ID: {args[5]}");
                return;
            }
            
            PyramidJob job = new(new BlockPos(x, y, z), radius, layers, type);
            _jobs.Add(job);
            LocalPlayer.SendMessage($"{player.Username}: Enqueued a job to place the pyramid. This will take ~{job.EstimateExecutionTime:g}");
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

            VeryEasyJob job = new(new BlockPos(x, y, z));
            _jobs.Add(job);
            LocalPlayer.SendMessage($"{player.Username}: Enqueued a job to place its very easy. This will take ~{job.EstimateExecutionTime:g}");
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

                    LocalPlayer.SendMessage($"{player.Username}: There are {_jobs.Count} job(s) to do: ");
                    foreach (Job job in _jobs.Queue)
                    {
                        LocalPlayer.SendMessage($"- {job} (~{job.EstimateExecutionTime})");
                    }
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
        Stopwatch jobStopwatch = new();
        Stopwatch totalStopwatch = new();
        
        while (true)
        {
            if (_jobs.IsEmpty) continue;
            int jobCount = _jobs.Count;
            totalStopwatch.Restart();
            while (_jobs.TryRemove(out Job? job))
            {
                jobStopwatch.Restart();
                await job.ExecuteAsync(LocalPlayer, Level);
                jobStopwatch.Stop();
                LocalPlayer.SendMessage($"Finished {job} in {jobStopwatch.Elapsed:g}. {_jobs.Count} remaining jobs");
            }
            totalStopwatch.Stop();
            LocalPlayer.SendMessage($"Finished {jobCount} jobs in {totalStopwatch:g}");
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