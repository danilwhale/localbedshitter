using LocalBedShitter.Networking.Packets;

namespace LocalBedShitter.API;

public struct BlockPos(short x, short y, short z)
{
    public const int SizeInBytes = sizeof(short) * 3;
    
    public short X = x;
    public short Y = y;
    public short Z = z;

    public BlockPos(int x, int y, int z)
        : this((short)x, (short)y, (short)z)
    {
    }
    
    public BlockPos Up() => this with { Y = (short)(Y + 1) };
    public BlockPos Down() => this with { Y = (short)(Y - 1) };
    public BlockPos East() => this with { X = (short)(X + 1) };
    public BlockPos West() => this with { X = (short)(X - 1) };
    public BlockPos South() => this with { Z = (short)(Z + 1) };
    public BlockPos North() => this with { Z = (short)(Z - 1) };

    public static BlockPos Read(ref PacketReader reader)
    {
        return new BlockPos(reader.ReadShort(), reader.ReadShort(), reader.ReadShort());
    }

    public static void Write(ref PacketWriter writer, BlockPos pos)
    {
        writer.WriteShort(pos.X);
        writer.WriteShort(pos.Y);
        writer.WriteShort(pos.Z);
    }
}