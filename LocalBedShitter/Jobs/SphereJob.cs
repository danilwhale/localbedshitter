using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public sealed class SphereJob(BlockPos pos, short radius, byte type) : Job
{
    public readonly BlockPos Pos = pos;
    public readonly short Radius = radius;
    public readonly byte Type = type;

    public override int BlockCount => FastMath.Floor(4.0f / 3 * MathF.PI * (Radius * Radius * Radius));

    public override async Task ExecuteAsync(LocalPlayer player, Level level)
    {
        for (int x = -Radius; x <= Radius; x++)
        {
            for (int y = -Radius; y <= Radius; y++)
            {
                for (int z = -Radius; z <= Radius; z++)
                {
                    float distance = -x * -x + -y * -y + -z * -z;
                    if (distance <= Radius * Radius)
                    {
                        await SetBlockAsync(player, level, Pos + new BlockPos(x, y, z), Type);
                    }
                }
            }
        }
    }
}