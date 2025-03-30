namespace LocalBedShitter.Networking.Packets;

public interface IPacket
{
    int Length { get; }

    void Read(ref PacketReader reader);
    void Write(ref PacketWriter writer);
}