using System.Numerics;
using LocalBedShitter.API;
using LocalBedShitter.API.Players;
using LocalBedShitter.Jobs;

namespace LocalBedShitter;

public sealed partial class MainBot
{
    private Task HandleTpPlayerCommand(RemotePlayer player, string[] args)
    {
        RemotePlayer? targetPlayer = PlayerManager.Players.FirstOrDefault(p =>
            p.Username.Equals(args[0], StringComparison.InvariantCultureIgnoreCase));
        if (targetPlayer == null)
        {
            LocalPlayer.SendMessage($"{player.Username}: No such player as '{args[0]}'");
            return Task.CompletedTask;
        }

        LocalPlayer.Teleport(targetPlayer);
        LocalPlayer.SendMessage($"{player.Username}: Teleported to {targetPlayer.Username}");
        return Task.CompletedTask;
    }

    private Task HandleTpCoordsCommand(RemotePlayer player, string[] args)
    {
        if (!float.TryParse(args[0], out float x) || !float.TryParse(args[1], out float y) ||
            !float.TryParse(args[2], out float z))
        {
            LocalPlayer.SendMessage($"{player.Username}: Invalid location: {args[0]}, {args[1]}, {args[2]}");
            return Task.CompletedTask;
        }

        LocalPlayer.Teleport(new Vector3(x, y, z), Vector2.Zero);
        LocalPlayer.SendMessage($"{player.Username}: Teleported to {x}, {y}, {z}");
        return Task.CompletedTask;
    }

    private Task HandleSayCommand(RemotePlayer _, string[] args)
    {
        string message = string.Join(" ", args);
        LocalPlayer.SendMessage(message);
        if (message.StartsWith('/'))
        {
            foreach (ChildBot child in _children)
            {
                child.LocalPlayer.SendMessage(message);
            }
        }
        return Task.CompletedTask;
    }

    private Task HandleSetBlockCommand(RemotePlayer player, string[] args)
    {
        if (!short.TryParse(args[0], out short x) || !short.TryParse(args[1], out short y) ||
            !short.TryParse(args[2], out short z))
        {
            LocalPlayer.SendMessage($"{player.Username}: Invalid location: [{args[0]}, {args[1]}, {args[2]}]");
            return Task.CompletedTask;
        }

        if (!byte.TryParse(args[3], out byte type))
        {
            LocalPlayer.SendMessage($"{player.Username}: Invalid block ID: {args[3]}");
            return Task.CompletedTask;
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
        return Task.CompletedTask;
    }

    private Task HandleFillCommand(RemotePlayer player, string[] args)
    {
        if (!short.TryParse(args[0], out short x0) || !short.TryParse(args[1], out short y0) ||
            !short.TryParse(args[2], out short z0) || !short.TryParse(args[3], out short x1) ||
            !short.TryParse(args[4], out short y1) || !short.TryParse(args[5], out short z1))
        {
            LocalPlayer.SendMessage(
                $"{player.Username}: Invalid location: [{args[0]}, {args[1]}, {args[2]}]:[{args[3]}, {args[4]}, {args[5]}]");
            return Task.CompletedTask;
        }

        if (!byte.TryParse(args[6], out byte type))
        {
            LocalPlayer.SendMessage($"{player.Username}: Invalid block ID: {args[6]}");
            return Task.CompletedTask;
        }

        short minX = Math.Min(x0, x1), minY = Math.Min(y0, y1), minZ = Math.Min(z0, z1);
        short maxX = Math.Max(x0, x1), maxY = Math.Max(y0, y1), maxZ = Math.Max(z0, z1);

        FillJob job = new(new BlockPos(minX, minY, minZ), new BlockPos(maxX, maxY, maxZ), type);
        _jobs.Add(job);

        LocalPlayer.SendMessage($"{player.Username}: Enqueued a job to fill " +
                                $"{(maxX - minX + 1) * (maxY - minY + 1) * (maxZ - minZ + 1)} " +
                                $"blocks with {type}");
        return Task.CompletedTask;
    }
    
    private Task HandleReplaceCommand(RemotePlayer player, string[] args)
    {
        if (!short.TryParse(args[0], out short x0) || !short.TryParse(args[1], out short y0) ||
            !short.TryParse(args[2], out short z0) || !short.TryParse(args[3], out short x1) ||
            !short.TryParse(args[4], out short y1) || !short.TryParse(args[5], out short z1))
        {
            LocalPlayer.SendMessage(
                $"{player.Username}: Invalid location: [{args[0]}, {args[1]}, {args[2]}]:[{args[3]}, {args[4]}, {args[5]}]");
            return Task.CompletedTask;
        }

        if (!byte.TryParse(args[6], out byte oldType))
        {
            LocalPlayer.SendMessage($"{player.Username}: Invalid block ID: {args[6]}");
            return Task.CompletedTask;
        }
        
        if (!byte.TryParse(args[7], out byte newType))
        {
            LocalPlayer.SendMessage($"{player.Username}: Invalid block ID: {args[6]}");
            return Task.CompletedTask;
        }

        short minX = Math.Min(x0, x1), minY = Math.Min(y0, y1), minZ = Math.Min(z0, z1);
        short maxX = Math.Max(x0, x1), maxY = Math.Max(y0, y1), maxZ = Math.Max(z0, z1);

        ReplaceJob job = new(new BlockPos(minX, minY, minZ), new BlockPos(maxX, maxY, maxZ), oldType, newType);
        _jobs.Add(job);

        LocalPlayer.SendMessage($"{player.Username}: Enqueued a job to replace " +
                                $"{(maxX - minX + 1) * (maxY - minY + 1) * (maxZ - minZ + 1)} " +
                                $"blocks of {oldType} with {newType}");
        return Task.CompletedTask;
    }

    private Task HandleDryCommand(RemotePlayer player, string[] args)
    {
        if (!short.TryParse(args[0], out short x0) || !short.TryParse(args[1], out short y0) ||
            !short.TryParse(args[2], out short z0) || !short.TryParse(args[3], out short x1) ||
            !short.TryParse(args[4], out short y1) || !short.TryParse(args[5], out short z1))
        {
            LocalPlayer.SendMessage(
                $"{player.Username}: Invalid location: [{args[0]}, {args[1]}, {args[2]}]:[{args[3]}, {args[4]}, {args[5]}]");
            return Task.CompletedTask;
        }
        
        short minX = Math.Min(x0, x1), minY = Math.Min(y0, y1), minZ = Math.Min(z0, z1);
        short maxX = Math.Max(x0, x1), maxY = Math.Max(y0, y1), maxZ = Math.Max(z0, z1);

        DryJob job = new(new BlockPos(minX, minY, minZ), new BlockPos(maxX, maxY, maxZ));
        _jobs.Add(job);
        
        LocalPlayer.SendMessage($"{player.Username}: Enqueued a job to dry the area");
        return Task.CompletedTask;
    }

    private Task HandleEatChunksCommand(RemotePlayer player, string[] args)
    {
        if (!int.TryParse(args[0], out int count))
        {
            LocalPlayer.SendMessage($"{player.Username}: Invalid count: {count}");
            return Task.CompletedTask;
        }

        ChunkEaterJob job = new(count);
        _jobs.Add(job);
        
        LocalPlayer.SendMessage($"{player.Username}: Enqueued a job to eat {job.Count} chunks");
        return Task.CompletedTask;
    }

    private Task HandleSphereCommand(RemotePlayer player, string[] args)
    {
        if (!short.TryParse(args[0], out short x) || !short.TryParse(args[1], out short y) ||
            !short.TryParse(args[2], out short z))
        {
            LocalPlayer.SendMessage($"{player.Username}: Invalid location: [{args[0]}, {args[1]}, {args[2]}]");
            return Task.CompletedTask;
        }

        if (!short.TryParse(args[3], out short radius))
        {
            LocalPlayer.SendMessage($"{player.Username}: Invalid radius: {args[3]}");
            return Task.CompletedTask;
        }

        if (!byte.TryParse(args[4], out byte type))
        {
            LocalPlayer.SendMessage($"{player.Username}: Invalid block ID: {args[4]}");
            return Task.CompletedTask;
        }

        SphereJob job = new(new BlockPos(x, y, z), radius, type);
        _jobs.Add(job);
        LocalPlayer.SendMessage($"{player.Username}: Enqueued a job to place the sphere");
        return Task.CompletedTask;
    }

    private Task HandlePyramidCommand(RemotePlayer player, string[] args)
    {
        if (!short.TryParse(args[0], out short x) || !short.TryParse(args[1], out short y) ||
            !short.TryParse(args[2], out short z))
        {
            LocalPlayer.SendMessage($"{player.Username}: Invalid location: [{args[0]}, {args[1]}, {args[2]}]");
            return Task.CompletedTask;
        }

        if (!short.TryParse(args[3], out short radius))
        {
            LocalPlayer.SendMessage($"{player.Username}: Invalid radius: {args[3]}");
            return Task.CompletedTask;
        }

        if (!short.TryParse(args[4], out short layers))
        {
            LocalPlayer.SendMessage($"{player.Username}: Invalid layers: {args[4]}");
            return Task.CompletedTask;
        }

        if (!byte.TryParse(args[5], out byte type))
        {
            LocalPlayer.SendMessage($"{player.Username}: Invalid block ID: {args[5]}");
            return Task.CompletedTask;
        }

        PyramidJob job = new(new BlockPos(x, y, z), radius, layers, type);
        _jobs.Add(job);
        LocalPlayer.SendMessage($"{player.Username}: Enqueued a job to place the pyramid");
        return Task.CompletedTask;
    }

    private Task HandleVeryEasyCommand(RemotePlayer player, string[] args)
    {
        if (!short.TryParse(args[0], out short x) || !short.TryParse(args[1], out short y) ||
            !short.TryParse(args[2], out short z))
        {
            LocalPlayer.SendMessage($"{player.Username}: Invalid location: [{args[0]}, {args[1]}, {args[2]}]");
            return Task.CompletedTask;
        }

        VeryEasyJob job = new(new BlockPos(x, y, z));
        _jobs.Add(job);
        LocalPlayer.SendMessage($"{player.Username}: Enqueued a job to place its very easy");
        return Task.CompletedTask;
    }

    private Task HandleJobsCommand(RemotePlayer player, string[] args)
    {
        switch (args[0])
        {
            case "list":
                if (_jobs.Count == 0)
                {
                    LocalPlayer.SendMessage($"{player.Username}: There are no jobs to do");
                    return Task.CompletedTask;
                }

                LocalPlayer.SendMessage($"{player.Username}: There are {_jobs.Count} job(s) to do: ");
                foreach (Job job in _jobs.Queue)
                {
                    LocalPlayer.SendMessage($"- {job}");
                }

                break;
            case "clear":
                _jobs.Clear();
                LocalPlayer.SendMessage($"{player.Username}: Removed all pending jobs");
                break;
        }

        return Task.CompletedTask;
    }
}