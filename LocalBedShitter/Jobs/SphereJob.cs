using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public sealed class SphereJob(BlockPos pos, short radius, byte type) : Job
{
    public readonly BlockPos Pos = pos;
    public readonly short Radius = radius;
    public readonly byte Type = type;

    public override async Task ExecuteAsync(PlayerPool players, Level level)
    {
        for (int x = -Radius; x <= Radius; x++)
        {
            for (int z = -Radius; z <= Radius; z++)
            {
                int xx = x;
                int zz = z;
                await players.RunOrWaitAsync(async player =>
                {
                    for (int y = -Radius; y <= Radius; y++)
                    {
                        float distance = -xx * -xx + -y * -y + -zz * -zz;
                        if (distance <= Radius * Radius)
                        {
                            await SetBlockAsync(player, level, Pos + new BlockPos(xx, y, zz), Type);
                        }
                    }
                });
            }
        }
    }
}