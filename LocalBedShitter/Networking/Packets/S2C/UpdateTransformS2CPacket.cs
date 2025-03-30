using System.Numerics;

namespace LocalBedShitter.Networking.Packets.S2C;

public struct UpdateTransformS2CPacket : IPacket
{
    public const int SizeInBytes = sizeof(sbyte) + 3 * sizeof(sbyte) + 2 * sizeof(byte);
    
    public int Length => SizeInBytes;

    public sbyte PlayerId;
    public Vector3 PositionDelta;
    public Vector2 Rotation;
    
    public void Read(ref PacketReader reader)
    {
        PlayerId = reader.ReadSByte();
        PositionDelta = new Vector3(
            (float)reader.ReadSByte() / (1 << 5),
            (float)reader.ReadSByte() / (1 << 5),
            (float)reader.ReadSByte() / (1 << 5)
        );
        Rotation = new Vector2(
            360 * (reader.ReadByte() / 256.0f),
            360 * (reader.ReadByte() / 256.0f)
        );
    }

    public void Write(ref PacketWriter writer)
    {
        writer.WriteSByte(PlayerId);
        writer.WriteSByte((sbyte)(PositionDelta.X * (1 << 5)));
        writer.WriteSByte((sbyte)(PositionDelta.Y * (1 << 5)));
        writer.WriteSByte((sbyte)(PositionDelta.Z * (1 << 5)));
        writer.WriteByte((byte)(Rotation.Y / 360.0f * 256));
        writer.WriteByte((byte)(Rotation.X / 360.0f * 256));
    }
}