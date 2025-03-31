using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public sealed class VeryEasyJob(BlockPos pos) : Job
{
    public readonly BlockPos Pos = pos;

    public override int BlockCount => 28;

    public override async Task ExecuteAsync(LocalPlayer player, Level level)
    {
        for (int i = 0; i < 2; i++)
        {
            await SetBlockAsync(player, level, new BlockPos(Pos.X + i * 3, Pos.Y, Pos.Z + 1), 41);
            for (int j = 0; j < 5; j++)
            {
                await SetBlockAsync(player, level, new BlockPos(Pos.X + i * 3, Pos.Y + j, Pos.Z), 41);
            }
        }

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                await SetBlockAsync(player, level, new BlockPos(Pos.X + i, Pos.Y + 5 + j, Pos.Z), 41);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            await SetBlockAsync(player, level, new BlockPos(Pos.X + 1, Pos.Y + 5 - i, Pos.Z + 1), 41);
        }

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                await SetBlockAsync(player, level, new BlockPos(Pos.X - 2 + i * 6 + j, Pos.Y + 9, Pos.Z), 41);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                await SetBlockAsync(player, level, new BlockPos(Pos.X + 1 + i, Pos.Y + 10 + j, Pos.Z), 41);
            }
        }
    }
}