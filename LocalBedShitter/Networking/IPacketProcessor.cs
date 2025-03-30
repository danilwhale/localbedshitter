using LocalBedShitter.Networking.Packets;

namespace LocalBedShitter.Networking;

public interface IPacketProcessor
{
    void ProcessPacket(ref readonly IPacket packet);
}