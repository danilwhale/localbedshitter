using System.Numerics;

namespace LocalBedShitter.Networking.Packets.S2C;

public struct MoveS2CPacket : IPacket
{
    public const int SizeInBytes = sizeof(sbyte) + 3 * sizeof(byte);
    
    public int Length => SizeInBytes;

    public sbyte PlayerId;
    public Vector3 Delta;
    
    public void Read(PacketReader reader)
    {
        PlayerId = reader.ReadSByte();
        Delta = new Vector3(
            (float)reader.ReadByte() / (1 << 5),
            (float)reader.ReadByte() / (1 << 5),
            (float)reader.ReadByte() / (1 << 5)
        );
    }

    public void Write(PacketWriter writer)
    {
        writer.WriteSByte(PlayerId);
        writer.WriteByte((byte)(Delta.X * (1 << 5)));
        writer.WriteByte((byte)(Delta.Y * (1 << 5)));
        writer.WriteByte((byte)(Delta.Z * (1 << 5)));
    }
}