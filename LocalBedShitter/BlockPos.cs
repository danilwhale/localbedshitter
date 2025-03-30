using LocalBedShitter.Packets;

namespace LocalBedShitter;

public struct BlockPos(short x, short y, short z)
{
    public const int SizeInBytes = sizeof(short) * 3;
    
    public short X = x;
    public short Y = y;
    public short Z = z;
    
    public BlockPos Up => this with { Y = (short)(Y + 1) };
    public BlockPos Down => this with { Y = (short)(Y - 1) };
    public BlockPos East => this with { X = (short)(X + 1) };
    public BlockPos West => this with { X = (short)(X - 1) };
    public BlockPos South => this with { Z = (short)(Z + 1) };
    public BlockPos North => this with { Z = (short)(Z - 1) };

    public void Read(PacketReader reader)
    {
        X = reader.ReadShort();
        Y = reader.ReadShort();
        Z = reader.ReadShort();
    }

    public void Write(PacketWriter writer)
    {
        writer.WriteShort(X);
        writer.WriteShort(Y);
        writer.WriteShort(Z);
    }
}