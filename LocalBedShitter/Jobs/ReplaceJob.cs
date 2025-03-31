using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public sealed class ReplaceJob(BlockPos min, BlockPos max, byte source, byte type) : Job
{
    public readonly BlockPos Min = min;
    public readonly BlockPos Max = max;
    public readonly byte Source = source;
    public readonly byte Type = type;
    
    public override int BlockCount => (Math.Abs(Max.X - Min.X) + 1) *
                                      (Math.Abs(Max.Y - Min.Y) + 1) *
                                      (Math.Abs(Max.Z - Min.Z) + 1);
    
    public override async Task ExecuteAsync(LocalPlayer player, Level level)
    {
        Random random = Random.Shared;
        HashSet<BlockPos> visitedBlocks = [];
        for (short x = Min.X; x <= Max.X; x++)
        {
            for (short y = Min.Y; y <= Max.Y; y++)
            {
                for (short z = Min.Z; z <= Max.Z; z++)
                {
                    byte block = level.GetBlock(x, y, z);
                    if (block == Type || block != Source)
                    {
                        visitedBlocks.Add(new BlockPos(x, y, z));
                    }
                }
            }
        }
        
        int toVisit = BlockCount;
        while (visitedBlocks.Count != toVisit)
        {
            BlockPos pos = new(
                random.Next(Min.X, Max.X + 1),
                random.Next(Min.Y, Max.Y + 1),
                random.Next(Min.Z, Max.Z + 1)
            );
            if (!visitedBlocks.Add(pos)) continue;
            await SetBlockAsync(player, level, pos, Type);
        }
    }
}