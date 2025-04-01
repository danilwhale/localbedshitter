using LocalBedShitter.API.Players;

namespace LocalBedShitter.Networking.Packets.S2C;

public struct UpdateUserTypeS2CPacket : IPacket
{
    public const int SizeInBytes = sizeof(byte);

    public int Length => SizeInBytes;

    public UserType UserType;
    
    public void Read(ref PacketReader reader)
    {
        UserType = (UserType)reader.ReadByte();
    }

    public void Write(ref PacketWriter writer)
    {
        writer.WriteByte((byte)UserType);
    }
}