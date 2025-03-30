using System.Numerics;

namespace LocalBedShitter.Packets.C2S;

public struct TeleportC2SPacket(Vector3 position, Vector2 rotation) : IPacket
{
    public const int SizeInBytes = sizeof(sbyte) + 3 * sizeof(short) + 2 * sizeof(byte); 
    
    public int Length => SizeInBytes;

    public sbyte PlayerId = -1;
    public Vector3 Position = position;
    public Vector2 Rotation = rotation;
    
    public void Read(PacketReader reader)
    {
        PlayerId = reader.ReadSByte();
        Position = new Vector3(
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
        writer.WriteShort((short)(Position.X * (1 << 5)));
        writer.WriteShort((short)(Position.Y * (1 << 5)));
        writer.WriteShort((short)(Position.Z * (1 << 5)));
        writer.WriteByte((byte)(Rotation.Y / 360.0f * 255));
        writer.WriteByte((byte)(Rotation.X / 360.0f * 255));
    }
}