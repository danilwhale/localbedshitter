using System.Numerics;
using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public abstract class Job
{
    protected const int SetBlockDelay = 25;
    
    public abstract int BlockCount { get; }
    public TimeSpan EstimateExecutionTime => TimeSpan.FromMilliseconds(BlockCount * SetBlockDelay);
    public abstract Task ExecuteAsync(LocalPlayer player, Level level);
    
    protected static async Task SetBlockAsync(LocalPlayer player, Level level, BlockPos pos, byte type)
    {
        if (level.GetBlock(pos.X, pos.Y, pos.Z) != type)
        {
            player.Teleport(new Vector3(pos.X, pos.Y, pos.Z), Vector2.Zero);
            await Task.Delay(SetBlockDelay);
            player.SetBlock(pos, type == 0 ? EditMode.Destroy : EditMode.Create, type);
        }
    }
}