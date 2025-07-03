using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public sealed class ReplaceJob(BlockPos min, BlockPos max, byte oldType, byte newType) : Job
{
    public readonly BlockPos Min = min;
    public readonly BlockPos Max = max;
    public readonly byte OldType = oldType;
    public readonly byte NewType = newType;

    public override async Task ExecuteAsync(PlayerPool players, Level level)
    {
        if (OldType == NewType) return;
        
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
                        if (level.GetBlock(xx, y, zz) != OldType) continue;
                        await SetBlockAsync(player, level, new BlockPos(xx, y, zz), NewType);
                    }
                });
            }
        }
    }
}