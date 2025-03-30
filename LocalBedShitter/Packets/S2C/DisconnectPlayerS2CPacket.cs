namespace LocalBedShitter.Packets.S2C;

public struct DisconnectPlayerS2CPacket : IPacket
{
    public const int SizeInBytes = 64;
    
    public int Length => SizeInBytes;

    public string Reason;
    
    public void Read(PacketReader reader)
    {
        Reason = reader.ReadString();
    }

    public void Write(PacketWriter writer)
    {
        writer.WriteString(Reason);
    }
}