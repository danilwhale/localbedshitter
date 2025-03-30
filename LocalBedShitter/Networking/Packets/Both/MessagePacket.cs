namespace LocalBedShitter.Networking.Packets.Both;

public struct MessagePacket(sbyte playerId, string content) : IPacket
{
    public const int SizeInBytes = sizeof(sbyte) + 64;

    public int Length => SizeInBytes;
    
    public sbyte PlayerId = playerId;
    public string Content = content;
    
    public void Read(PacketReader reader)
    {
        PlayerId = reader.ReadSByte();
        Content = reader.ReadString();
    }

    public void Write(PacketWriter writer)
    {
        writer.WriteSByte(PlayerId);
        writer.WriteString(Content);
    }
}