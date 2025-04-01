using System.Buffers;
using System.Numerics;
using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Jobs;

public abstract class Job : IDisposable
{
    protected const int SetBlockDelay = 25;

    private BlockPos[] _blocks;
    
    public abstract void Initialize(Level level);
    public abstract Task ExecuteAsync(LocalPlayer player, Level level);

    protected void SetupBlocks(List<BlockPos> blocks)
    {
        _blocks = ArrayPool<BlockPos>.Shared.Rent(blocks.Count);
        blocks.CopyTo(_blocks);
        Random.Shared.Shuffle(_blocks);
    }

    protected async Task SetBlocksAsync(LocalPlayer player, Level level, byte type)
    {
        foreach (BlockPos pos in _blocks)
        {
            await SetBlockAsync(player, level, pos, type);
        }
    }
    
    protected static async Task SetBlockAsync(LocalPlayer player, Level level, BlockPos pos, byte type)
    {
        if (level.GetBlock(pos.X, pos.Y, pos.Z) != type)
        {
            player.Teleport(new Vector3(pos.X, pos.Y, pos.Z), Vector2.Zero);
            await Task.Delay(SetBlockDelay);
            player.SetBlock(pos, type == 0 ? EditMode.Destroy : EditMode.Create, type);
            level.SetBlock(pos.X, pos.Y, pos.Z, type);
        }
    }

    public virtual void Dispose()
    {
        if (_blocks != null) ArrayPool<BlockPos>.Shared.Return(_blocks);
    }
}