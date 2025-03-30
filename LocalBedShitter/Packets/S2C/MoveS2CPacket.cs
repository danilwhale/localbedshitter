using System.Numerics;

namespace LocalBedShitter.Packets.S2C;

public struct MoveS2CPacket : IPacket
{
    public const int SizeInBytes = sizeof(sbyte) + 3 * sizeof(short);
    
    public int Length => SizeInBytes;

    public sbyte PlayerId;
    public Vector3 Delta;
    
    public void Read(PacketReader reader)
    {
        PlayerId = reader.ReadSByte();
        Delta = new Vector3(
            (float)reader.ReadShort() / (1 << 5),
            (float)reader.ReadShort() / (1 << 5),
            (float)reader.ReadShort() / (1 << 5)
        );
    }

    public void Write(PacketWriter writer)
    {
        writer.WriteSByte(PlayerId);
        writer.WriteShort((short)(Delta.X * (1 << 5)));
        writer.WriteShort((short)(Delta.Y * (1 << 5)));
        writer.WriteShort((short)(Delta.Z * (1 << 5)));
    }
}