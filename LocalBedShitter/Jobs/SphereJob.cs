using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public sealed class SphereJob(BlockPos centerPos, short radius, byte type) : Job
{
    public readonly BlockPos CenterPos = centerPos;
    public readonly short Radius = radius;
    public readonly byte Type = type;
    
    public override async Task ExecuteAsync(LocalPlayer player)
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
                        await SetBlockAsync(player, CenterPos + new BlockPos(x, y, z), Type);
                    }
                }
            }
        }
    }

    public override string ToString()
    {
        return $"a sphere at {CenterPos}, r={Radius} with block {Type}";
    }
}