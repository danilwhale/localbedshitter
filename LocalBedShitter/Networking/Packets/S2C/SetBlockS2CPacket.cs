using LocalBedShitter.API;

namespace LocalBedShitter.Networking.Packets.S2C;

public struct SetBlockS2CPacket : IPacket
{
    public const int SizeInBytes = BlockPos.SizeInBytes + sizeof(byte);
    
    public int Length => SizeInBytes;

    public BlockPos Pos;
    public byte Type;
    
    public void Read(ref PacketReader reader)
    {
        Pos = BlockPos.Read(ref reader);
        Type = reader.ReadByte();
    }

    public void Write(ref PacketWriter writer)
    {
        BlockPos.Write(ref writer, Pos);
        writer.WriteByte(Type);
    }
}