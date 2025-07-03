using LocalBedShitter.API;

namespace LocalBedShitter.Jobs;

public sealed class DryJob(BlockPos min, BlockPos max) : Job
{
    public readonly BlockPos Min = min;
    public readonly BlockPos Max = max;
    
    public override async Task ExecuteAsync(PlayerPool players, Level level)
    {
        for (short x = Min.X; x <= Max.X; x += 4)
        {
            for (short z = Min.Z; z <= Max.Z; z += 4)
            {
                short xx = x;
                short zz = z;
                await players.RunOrWaitAsync(async player =>
                {
                    for (short y = Max.Y; y >= Min.Y; y -= 4)
                    {
                        for (int i = 0; i < 64; i++)
                        {
                            int lx = i >> 4;
                            int ly = (i >> 2) & 3;
                            int lz = i & 3;
                            if (level.GetBlock((short)(xx + lx), (short)(y + ly), (short)(zz + lz)) == 9)
                            {
                                await SetBlockAsync(player, level, new BlockPos(xx + 2, y + 2, zz + 2), 19);
                                break;
                            }
                        }
                    }
                });
            }
        }
    }
}