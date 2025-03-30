using System.Numerics;

namespace LocalBedShitter.Networking.Packets.S2C;

public struct MoveS2CPacket : IPacket
{
    public const int SizeInBytes = sizeof(sbyte) + 3 * sizeof(sbyte);
    
    public int Length => SizeInBytes;

    public sbyte PlayerId;
    public Vector3 Delta;
    
    public void Read(PacketReader reader)
    {
        PlayerId = reader.ReadSByte();
        Delta = new Vector3(
            (float)reader.ReadSByte() / (1 << 5),
            (float)reader.ReadSByte() / (1 << 5),
            (float)reader.ReadSByte() / (1 << 5)
        );
    }

    public void Write(PacketWriter writer)
    {
        writer.WriteSByte(PlayerId);
        writer.WriteSByte((sbyte)(Delta.X * (1 << 5)));
        writer.WriteSByte((sbyte)(Delta.Y * (1 << 5)));
        writer.WriteSByte((sbyte)(Delta.Z * (1 << 5)));
    }
}