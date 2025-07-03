using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public sealed class FillJob(BlockPos min, BlockPos max, byte type) : Job
{
    public readonly BlockPos Min = min;
    public readonly BlockPos Max = max;
    public readonly byte Type = type;

    public override async Task ExecuteAsync(PlayerPool players, Level level)
    {
        for (short x = Min.X; x <= Max.X; x++)
        {
            for (short z = Min.Z; z <= Max.Z; z++)
            {
                short xx = x;
                short zz = z;
                await players.RunOrWaitAsync(async player =>
                {
                    for (short y = Max.Y; y >= Min.Y; y--)
                    {
                        await SetBlockAsync(player, level, new BlockPos(xx, y, zz), Type);
                    }
                });
            }
        }
    }
}