using System.Numerics;

namespace LocalBedShitter.Networking.Packets.S2C;

public struct SpawnPlayerS2CPacket : IPacket
{
    public const int SizeInBytes = sizeof(sbyte) + 64 + 3 * sizeof(short) + 2 * sizeof(byte);
    
    public int Length => SizeInBytes;

    public sbyte PlayerId;
    public string Username;
    public Vector3 Position;
    public Vector2 Rotation;
    
    public void Read(ref PacketReader reader)
    {
        PlayerId = reader.ReadSByte();
        Username = reader.ReadString();
        Position = new Vector3(
            (float)reader.ReadShort() / (1 << 5),
            (float)reader.ReadShort() / (1 << 5),
            (float)reader.ReadShort() / (1 << 5)
        );
        Rotation = new Vector2(
            360 * (reader.ReadByte() / 256.0f),
            360 * (reader.ReadByte() / 256.0f)
        );
    }

    public void Write(ref PacketWriter writer)
    {
        writer.WriteSByte(PlayerId);
        writer.WriteString(Username);
        writer.WriteShort((short)(Position.X * (1 << 5)));
        writer.WriteShort((short)(Position.Y * (1 << 5)));
        writer.WriteShort((short)(Position.Z * (1 << 5)));
        writer.WriteByte((byte)(Rotation.Y / 360.0f * 256));
        writer.WriteByte((byte)(Rotation.X / 360.0f * 256));
    }
}