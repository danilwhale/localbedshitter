namespace LocalBedShitter.Networking.Packets.C2S;

public struct MessageC2SPacket(string content) : IPacket
{
    public const int SizeInBytes = sizeof(sbyte) + 64;

    public int Length => SizeInBytes;

    private sbyte _unused = -1;
    public string Content = content;
    
    public void Read(PacketReader reader)
    {
        _unused = reader.ReadSByte();
        Content = reader.ReadString();
    }

    public void Write(PacketWriter writer)
    {
        writer.WriteSByte(_unused);
        writer.WriteString(Content);
    }
}