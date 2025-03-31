using System.Diagnostics.CodeAnalysis;
using LocalBedShitter.Networking.Packets;

namespace LocalBedShitter.API;

public struct BlockPos(short x, short y, short z) : IEquatable<BlockPos>
{
    public const int SizeInBytes = sizeof(short) * 3;

    public int LengthSquared => X * X + Y * Y + Z * Z;
    
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

    public override string ToString()
    {
        return $"[{X}, {Y}, {Z}]";
    }

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

    public static BlockPos operator +(BlockPos left, BlockPos right)
    {
        return new BlockPos(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    public static BlockPos operator -(BlockPos left, BlockPos right)
    {
        return new BlockPos(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }

    public bool Equals(BlockPos other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override bool Equals(object? obj)
    {
        return obj is BlockPos other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public static bool operator ==(BlockPos left, BlockPos right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BlockPos left, BlockPos right)
    {
        return !(left == right);
    }
}