namespace LocalBedShitter.Networking.Packets.S2C;

public struct DespawnPlayerS2CPacket : IPacket
{
    public const int SizeInBytes = sizeof(sbyte);
    
    public int Length => SizeInBytes;

    public sbyte PlayerId;
    
    public void Read(ref PacketReader reader)
    {
        PlayerId = reader.ReadSByte();
    }

    public void Write(ref PacketWriter writer)
    {
        writer.WriteSByte(PlayerId);
    }
}