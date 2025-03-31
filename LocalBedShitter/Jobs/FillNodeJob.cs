using System.Numerics;
using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public sealed class FillNodeJob(BlockPos min, BlockPos max, byte type) : Job
{
    public readonly BlockPos Min = min;
    public readonly BlockPos Max = max;
    public readonly byte Type = type;

    public override async Task ExecuteAsync(LocalPlayer player)
    {
        if ((Max - Min).LengthSquared > 125)
        {
            throw new InvalidOperationException("Node cannot be larger than 5x5x5");
        }

        BlockPos mm = Max + Min;
        player.Teleport(new Vector3(mm.X * 0.5f, mm.Y * 0.5f, mm.Z * 0.5f), Vector2.Zero);

        for (short x = Min.X; x <= Max.X; x++)
        {
            for (short y = Min.Y; y <= Max.Y; y++)
            {
                for (short z = Min.Z; z <= Max.Z; z++)
                {
                    await Task.Delay(29);
                    player.SetBlock(new BlockPos(x, y, z), Type == 0 ? EditMode.Destroy : EditMode.Create, Type);
                }
            }
        }

        await Task.CompletedTask;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Min, Max, Type);
    }

    public override string ToString()
    {
        return $"{Min}:{Max} -> {Type}";
    }

    public static HashSet<FillNodeJob> CreateFill(BlockPos min, BlockPos max, byte type)
    {
        HashSet<FillNodeJob> jobs = [];
        
        BlockPos size = max - min;

        for (int x = 0; x <= (size.X / 5); x++)
        {
            for (int y = 0; y <= (size.Y / 5); y++)
            {
                for (int z = 0; z <= (size.Z / 5); z++)
                {
                    BlockPos nodeMin = min + new BlockPos(x * 5, y * 5, z * 5);
                    BlockPos nodeMax = new(
                        Math.Min(min.X + (x + 1) * 5, max.X),
                        Math.Min(min.Y + (y + 1) * 5, max.Y),
                        Math.Min(min.Z + (z + 1) * 5, max.Z)
                    );
                    jobs.Add(new FillNodeJob(nodeMin, nodeMax, type));
                }
            }
        }

        return jobs;
    }
}