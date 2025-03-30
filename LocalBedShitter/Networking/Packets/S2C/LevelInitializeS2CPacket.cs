namespace LocalBedShitter.Networking.Packets.S2C;

public struct LevelInitializeS2CPacket : IPacket
{
    public const int SizeInBytes = 0;
    
    public int Length => SizeInBytes;
    
    public void Read(PacketReader reader)
    {
    }

    public void Write(PacketWriter writer)
    {
    }
}