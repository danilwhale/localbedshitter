using LocalBedShitter.Networking;
using LocalBedShitter.Networking.Packets.S2C;

namespace LocalBedShitter.API.Players;

public sealed class RemotePlayer : Player
{
    public RemotePlayer(NetworkManager manager, SpawnPlayerS2CPacket packet) : base(manager, packet.PlayerId,
        TextUtility.Sanitize(packet.Username))
    {
        ArgumentOutOfRangeException.ThrowIfNegative(packet.PlayerId);
        Position = packet.Position;
        Rotation = packet.Rotation;
    }
}