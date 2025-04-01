using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public sealed class ReplaceJob(BlockPos min, BlockPos max, byte oldType, byte newType) : Job
{
    public readonly BlockPos Min = min;
    public readonly BlockPos Max = max;
    public readonly byte OldType = oldType;
    public readonly byte NewType = newType;

    public override void Initialize(Level level)
    {
        List<BlockPos> blocks = [];
        for (short x = Min.X; x <= Max.X; x++)
        {
            for (short y = Min.Y; y <= Max.Y; y++)
            {
                for (short z = Min.Z; z <= Max.Z; z++)
                {
                    byte block = level.GetBlock(x, y, z);
                    if (block == OldType) blocks.Add(new BlockPos(x, y, z));
                }
            }
        }

        SetupBlocks(blocks);
    }

    public override async Task ExecuteAsync(LocalPlayer player, Level level)
    {
        await SetBlocksAsync(player, level, NewType);
    }
}