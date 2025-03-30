namespace LocalBedShitter.Networking.Packets.S2C;

public struct DisconnectPlayerS2CPacket : IPacket
{
    public const int SizeInBytes = 64;
    
    public int Length => SizeInBytes;

    public string Reason;
    
    public void Read(ref PacketReader reader)
    {
        Reason = reader.ReadString();
    }

    public void Write(ref PacketWriter writer)
    {
        writer.WriteString(Reason);
    }
}