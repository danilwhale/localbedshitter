using System.Buffers;
using System.Numerics;
using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public abstract class Job
{
    protected const int SetBlockDelay = 25;

    public abstract Task ExecuteAsync(PlayerPool players, Level level);
    
    protected static async Task SetBlockAsync(LocalPlayer player, Level level, BlockPos pos, byte type)
    {
        if (level.GetBlock(pos.X, pos.Y, pos.Z) != type)
        {
            player.Teleport(new Vector3(pos.X, pos.Y, pos.Z), Vector2.Zero);
            player.SetBlock(pos, type == 0 ? EditMode.Destroy : EditMode.Create, type);
            level.SetBlock(pos.X, pos.Y, pos.Z, type);
            await Task.Delay(SetBlockDelay);
        }
    }
}