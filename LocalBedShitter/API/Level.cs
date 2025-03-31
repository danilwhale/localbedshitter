using System.Buffers;
using System.IO.Compression;
using LocalBedShitter.Networking;
using LocalBedShitter.Networking.Packets;
using LocalBedShitter.Networking.Packets.S2C;

namespace LocalBedShitter.API;

public sealed class Level : IPacketProcessor
{
    public short Width;
    public short Height;
    public short Depth;
    public byte[] Blocks;
    
    public bool IsReady;
    
    private readonly NetworkManager _manager;
    private readonly Queue<byte[]> _chunkData = [];
    private int _dataLength;

    public Level(NetworkManager manager)
    {
        _manager = manager;
        manager.Processors.Add(this);
    }

    public byte GetBlock(short x, short y, short z)
    {
        if (!IsReady) return 0;
        if (x < 0 || y < 0 || z < 0 || x >= Width || y >= Height || z >= Depth) return 0;
        return Blocks[x + Width * (z + y * Depth)];
    }

    public void SetBlock(short x, short y, short z, byte block)
    {
        if (!IsReady) return;
        if (x < 0 || y < 0 || z < 0 || x >= Width || y >= Height || z >= Depth) return;
        Blocks[x + Width * (z + y * Depth)] = block;
    }
    
    public void ProcessPacket(ref readonly IPacket packet)
    {
        switch (packet)
        {
            case SetBlockS2CPacket block:
                SetBlock(block.Pos.X, block.Pos.Y, block.Pos.Z, block.Type);
                break;
            case LevelInitializeS2CPacket:
                _chunkData.Clear();
                IsReady = false;
                break;
            case LevelChunkS2CPacket chunk:
                _chunkData.Enqueue(chunk.Data[..chunk.DataLength]);
                Console.WriteLine($"Downloading terrain... {chunk.PercentComplete}% ({chunk.DataLength} bytes)");
                _dataLength += chunk.DataLength;
                break;
            case LevelFinalizeS2CPacket finalize:
            {
                Width = finalize.Width;
                Height = finalize.Height;
                Depth = finalize.Depth;
                Blocks = new byte[Width * Height * Depth];
                
                // now we can start connecting chunks of data
                byte[] gzipData = ArrayPool<byte>.Shared.Rent(_dataLength);
                
                for (int i = 0; _chunkData.TryDequeue(out byte[]? chunk); i += chunk.Length)
                {
                    chunk.CopyTo(gzipData.AsSpan(i));
                }

                // using MemoryStream ums = new(Blocks); // uncompressed memory stream
                using MemoryStream ms = new(gzipData); // compressed memory stream
                using GZipStream gz = new(ms, CompressionMode.Decompress);

                Span<byte> lenBuffer = stackalloc byte[4];
                gz.ReadExactly(lenBuffer);
                gz.ReadExactly(Blocks);
                
                ArrayPool<byte>.Shared.Return(gzipData);
                
                IsReady = true;
                break;
            }
        }
    }
}