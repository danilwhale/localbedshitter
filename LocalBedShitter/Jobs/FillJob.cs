using System.Buffers;
using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public sealed class FillJob(BlockPos min, BlockPos max, byte type) : Job
{
    public readonly BlockPos Min = min;
    public readonly BlockPos Max = max;
    public readonly byte Type = type;

    public int BlockCount => (Math.Abs(Max.X - Min.X) + 1) *
                             (Math.Abs(Max.Y - Min.Y) + 1) *
                             (Math.Abs(Max.Z - Min.Z) + 1);
    
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
                    if (block != Type) blocks.Add(new BlockPos(x, y, z));
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