using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public sealed class VeryEasyJob(BlockPos pos) : Job
{
    public readonly BlockPos Pos = pos;

    public override void Initialize(Level level)
    {
        List<BlockPos> blocks = [];
        
        for (int i = 0; i < 2; i++)
        {
            blocks.Add(new BlockPos(Pos.X + i * 3, Pos.Y, Pos.Z + 1));
            for (int j = 0; j < 5; j++)
            {
                blocks.Add(new BlockPos(Pos.X + i * 3, Pos.Y + j, Pos.Z));
            }
        }

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                blocks.Add(new BlockPos(Pos.X + i, Pos.Y + 5 + j, Pos.Z));
            }
        }

        for (int i = 0; i < 2; i++)
        {
            blocks.Add(new BlockPos(Pos.X + 1, Pos.Y + 5 - i, Pos.Z + 1));
        }

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                blocks.Add(new BlockPos(Pos.X - 2 + i * 6 + j, Pos.Y + 9, Pos.Z));
            }
        }

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                blocks.Add(new BlockPos(Pos.X + 1 + i, Pos.Y + 10 + j, Pos.Z));
            }
        }
        
        SetupBlocks(blocks);
    }

    public override async Task ExecuteAsync(LocalPlayer player, Level level)
    {
        await SetBlocksAsync(player, level, 41);
    }
}