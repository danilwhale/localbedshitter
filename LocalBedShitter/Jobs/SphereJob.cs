using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public sealed class SphereJob(BlockPos pos, short radius, byte type) : Job
{
    public readonly BlockPos Pos = pos;
    public readonly short Radius = radius;
    public readonly byte Type = type;

    public override void Initialize(Level level)
    {
        List<BlockPos> blocks = [];

        for (int x = -Radius; x <= Radius; x++)
        {
            for (int y = -Radius; y <= Radius; y++)
            {
                for (int z = -Radius; z <= Radius; z++)
                {
                    float distance = -x * -x + -y * -y + -z * -z;
                    if (distance <= Radius * Radius)
                    {
                        blocks.Add(Pos + new BlockPos(x, y, z));
                    }
                }
            }
        }
        
        SetupBlocks(blocks);
    }

    public override async Task ExecuteAsync(LocalPlayer player, Level level)
    {
        await SetBlocksAsync(player, level, Type);
    }
}