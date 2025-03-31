using System.Numerics;
using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public abstract class Job
{
    public abstract Task ExecuteAsync(LocalPlayer player);
    
    protected static async Task SetBlockAsync(LocalPlayer player, BlockPos pos, byte type)
    {
        player.Teleport(new Vector3(pos.X, pos.Y, pos.Z), Vector2.Zero);
        await Task.Delay(31);
        player.SetBlock(pos, type == 0 ? EditMode.Destroy : EditMode.Create, type);
    }
}