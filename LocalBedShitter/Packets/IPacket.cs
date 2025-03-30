namespace LocalBedShitter.Packets;

public interface IPacket
{
    int Length { get; }

    void Read(PacketReader reader);
    void Write(PacketWriter writer);
}