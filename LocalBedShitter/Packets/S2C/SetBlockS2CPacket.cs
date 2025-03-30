namespace LocalBedShitter.Packets.S2C;

public struct SetBlockS2CPacket : IPacket
{
    public const int SizeInBytes = BlockPos.SizeInBytes + sizeof(byte);
    
    public int Length => SizeInBytes;

    public BlockPos Pos;
    public byte Type;
    
    public void Read(PacketReader reader)
    {
        Pos.Read(reader);
        Type = reader.ReadByte();
    }

    public void Write(PacketWriter writer)
    {
        Pos.Write(writer);
        writer.WriteByte(Type);
    }
}