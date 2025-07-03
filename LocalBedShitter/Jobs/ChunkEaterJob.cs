using LocalBedShitter.API;

namespace LocalBedShitter.Jobs;

public sealed class ChunkEaterJob(int count) : Job
{
    public readonly int Count = count;
    
    public override async Task ExecuteAsync(PlayerPool players, Level level)
    {
        int maxChunks = level.Width * level.Depth >> 8;
        uint actualCount = (uint)(Count < 0 ? maxChunks : Math.Min(Count, maxChunks));
        Random random = Random.Shared;
        for (int i = 0; i < actualCount; i++)
        {
            short x0 = (short)(random.Next(0, level.Width >> 4) << 4);
            short x1 = (short)(x0 + (1 << 4));
            short z0 = (short)(random.Next(0, level.Depth >> 4) << 4);
            short z1 = (short)(z0 + (1 << 4));
            for (short x = x0; x < x1; x++)
            {
                for (short z = z0; z < z1; z++)
                {
                    short xx = x;
                    short zz = z;
                    await players.RunOrWaitAsync(async player =>
                    {
                        for (short y = (short)(level.Height - 1); y >= 0; y--)
                        {
                            await SetBlockAsync(player, level, new BlockPos(xx, y, zz), 0);
                        }
                    });
                }
            }
        }
    }
}