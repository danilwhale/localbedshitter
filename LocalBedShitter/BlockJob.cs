using System.Numerics;
using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter;

public sealed class BlockJob(BlockPos min, BlockPos max, byte type)
{
    public readonly BlockPos Min = min;
    public readonly BlockPos Max = max;
    public readonly byte Type = type;

    public async Task ExecuteAsync(LocalPlayer player, int delayMilliseconds = 33)
    {
        for (short x = Min.X; x <= Max.X; x++)
        {
            for (short y = Min.Y; y <= Max.Y; y++)
            {
                for (short z = Min.Z; z <= Max.Z; z++)
                {
                    player.Teleport(new Vector3(x, y, z + 1), Vector2.Zero);
                    await Task.Delay(delayMilliseconds);
                    player.SetBlock(new BlockPos(x, y, z), Type == 0 ? EditMode.Destroy : EditMode.Create, Type);
                }
            }
        }
    }
}