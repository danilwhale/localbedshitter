namespace LocalBedShitter.Packets.C2S;

public struct SetBlockC2SPacket(BlockPos pos, EditMode mode, byte type) : IPacket
{
    public const int SizeInBytes = BlockPos.SizeInBytes + sizeof(byte) + sizeof(byte);
    
    public int Length => SizeInBytes;

    public BlockPos Pos = pos;
    public EditMode Mode = mode;
    public byte Type = type;
    
    public void Read(PacketReader reader)
    {
        Pos.Read(reader);
        Mode = (EditMode)reader.ReadByte();
        Type = reader.ReadByte();
    }

    public void Write(PacketWriter writer)
    {
        Pos.Write(writer);
        writer.WriteByte((byte)Mode);
        writer.WriteByte(Type);
    }
}