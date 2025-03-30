using LocalBedShitter.API;

namespace LocalBedShitter.Networking.Packets.C2S;

public struct SetBlockC2SPacket(BlockPos pos, EditMode mode, byte type) : IPacket
{
    public const int SizeInBytes = BlockPos.SizeInBytes + sizeof(byte) + sizeof(byte);
    
    public int Length => SizeInBytes;

    public BlockPos Pos = pos;
    public EditMode Mode = mode;
    public byte Type = type;
    
    public void Read(ref PacketReader reader)
    {
        Pos = BlockPos.Read(ref reader);
        Mode = (EditMode)reader.ReadByte();
        Type = reader.ReadByte();
    }

    public void Write(ref PacketWriter writer)
    {
        BlockPos.Write(ref writer, Pos);
        writer.WriteByte((byte)Mode);
        writer.WriteByte(Type);
    }
}