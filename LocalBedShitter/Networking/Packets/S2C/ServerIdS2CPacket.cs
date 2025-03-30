using LocalBedShitter.API;
using LocalBedShitter.API.Players;

namespace LocalBedShitter.Networking.Packets.S2C;

public struct ServerIdS2CPacket : IPacket
{
    public const int SizeInBytes = sizeof(byte) + 64 + 64 + sizeof(byte);
    
    public int Length => SizeInBytes;

    public byte ProtocolVersion;
    public string ServerName;
    public string ServerMotd;
    public UserType UserType;
    
    public void Read(PacketReader reader)
    {
        ProtocolVersion = reader.ReadByte();
        ServerName = reader.ReadString();
        ServerMotd = reader.ReadString();
        UserType = (UserType)reader.ReadByte();
    }

    public void Write(PacketWriter writer)
    {
        writer.WriteByte(ProtocolVersion);
        writer.WriteString(ServerName);
        writer.WriteString(ServerMotd);
        writer.WriteByte((byte)UserType);
    }
}