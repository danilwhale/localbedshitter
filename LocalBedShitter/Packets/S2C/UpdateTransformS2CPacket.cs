using System.Numerics;

namespace LocalBedShitter.Packets.S2C;

public struct UpdateTransformS2CPacket : IPacket
{
    public const int SizeInBytes = sizeof(sbyte) + 3 * sizeof(short) + 2 * sizeof(byte);
    
    public int Length => SizeInBytes;

    public sbyte PlayerId;
    public Vector3 PositionDelta;
    public Vector2 Rotation;
    
    public void Read(PacketReader reader)
    {
        PlayerId = reader.ReadSByte();
        PositionDelta = new Vector3(
            (float)reader.ReadShort() / (1 << 5),
            (float)reader.ReadShort() / (1 << 5),
            (float)reader.ReadShort() / (1 << 5)
        );
        Rotation = new Vector2(
            360 * (reader.ReadByte() / 255.0f),
            360 * (reader.ReadByte() / 255.0f)
        );
    }

    public void Write(PacketWriter writer)
    {
        writer.WriteSByte(PlayerId);
        writer.WriteShort((short)(PositionDelta.X * (1 << 5)));
        writer.WriteShort((short)(PositionDelta.Y * (1 << 5)));
        writer.WriteShort((short)(PositionDelta.Z * (1 << 5)));
        writer.WriteByte((byte)(Rotation.Y / 360.0f * 255));
        writer.WriteByte((byte)(Rotation.X / 360.0f * 255));
    }
}