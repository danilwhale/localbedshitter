using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public sealed class PyramidJob(BlockPos pos, short radius, short layers, byte type) : Job
{
    public readonly BlockPos Pos = pos;
    public readonly short Radius = radius;
    public readonly short Layers = layers;
    public readonly byte Type = type;

    public override int BlockCount => FastMath.Floor(1.0f / 3 * (Radius * Radius) * Layers);

    public override async Task ExecuteAsync(LocalPlayer player, Level level)
    {
        for (int y = 0; y <= Layers; y++)
        {
            int radius = FastMath.Floor(Radius * (1.0f - 1.0f / Layers * y));
            for (int x = -radius; x <= radius; x++)
            {
                for (int z = -radius; z <= radius; z++)
                {
                    await SetBlockAsync(player, level, Pos + new BlockPos(x, y, z), Type);
                }
            }   
        }
    }
}