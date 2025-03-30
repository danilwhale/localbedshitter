using System.Numerics;

namespace LocalBedShitter.Networking.Packets.S2C;

public struct RotateS2CPacket : IPacket
{
    public const int SizeInBytes = sizeof(sbyte) + 2 * sizeof(byte);
    
    public int Length => SizeInBytes;

    public sbyte PlayerId;
    public Vector2 Delta;
    
    public void Read(ref PacketReader reader)
    {
        PlayerId = reader.ReadSByte();
        Delta = new Vector2(
            360 * (reader.ReadByte() / 256.0f),
            360 * (reader.ReadByte() / 256.0f)
        );
    }

    public void Write(ref PacketWriter writer)
    {
        writer.WriteSByte(PlayerId);
        writer.WriteByte((byte)(Delta.Y / 360.0f * 256));
        writer.WriteByte((byte)(Delta.X / 360.0f * 256));
    }
}