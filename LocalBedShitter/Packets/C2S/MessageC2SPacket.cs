namespace LocalBedShitter.Packets.C2S;

public struct MessageC2SPacket(string content) : IPacket
{
    public const int SizeInBytes = sizeof(byte) + 64;

    public int Length => SizeInBytes;

    private byte _unused = 0xFF;
    public string Content = content;
    
    public void Read(PacketReader reader)
    {
        _unused = reader.ReadByte();
        Content = reader.ReadString();
    }

    public void Write(PacketWriter writer)
    {
        writer.WriteByte(_unused);
        writer.WriteString(Content);
    }
}