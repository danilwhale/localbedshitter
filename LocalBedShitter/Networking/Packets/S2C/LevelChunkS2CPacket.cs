namespace LocalBedShitter.Networking.Packets.S2C;

public struct LevelChunkS2CPacket : IPacket
{
    public const int SizeInBytes = sizeof(short) + 1024 + sizeof(byte);
    
    public int Length => SizeInBytes;

    public short DataLength;
    public byte[] Data;
    public byte PercentComplete;
    
    public void Read(ref PacketReader reader)
    {
        DataLength = reader.ReadShort();
        Data = reader.ReadBytes(1024).ToArray();
        PercentComplete = reader.ReadByte();
    }

    public void Write(ref PacketWriter writer)
    {
        Span<byte> paddedData = stackalloc byte[1024];
        Data.CopyTo(paddedData);
        writer.WriteShort(DataLength);
        writer.WriteBytes(paddedData);
        writer.WriteByte(PercentComplete);
    }
}