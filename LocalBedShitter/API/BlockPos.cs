using LocalBedShitter.Networking.Packets;

namespace LocalBedShitter.API;

public record struct BlockPos(short X, short Y, short Z)
{
    public const int SizeInBytes = sizeof(short) * 3;
    
    public short X = X;
    public short Y = Y;
    public short Z = Z;

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