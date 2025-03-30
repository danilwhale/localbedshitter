using System.Diagnostics.CodeAnalysis;
using LocalBedShitter.Networking;
using LocalBedShitter.Networking.Packets;
using LocalBedShitter.Networking.Packets.Both;
using LocalBedShitter.Networking.Packets.S2C;

namespace LocalBedShitter.API.Players;

public sealed class PlayerManager : IPacketProcessor, IDisposable
{
    public readonly HashSet<RemotePlayer> Players = [];

    public event Action<RemotePlayer>? Connected;
    public event Action<RemotePlayer>? Disconnected;
    public event Action<RemotePlayer, string>? Message; 
    
    private readonly NetworkManager _manager;

    public PlayerManager(NetworkManager manager)
    {
        _manager = manager;
        manager.Processors.Add(this);
    }

    public bool IsConnected(sbyte id) => Players.Any(p => p.Id == id);
    public RemotePlayer? GetById(sbyte id) => Players.FirstOrDefault(p => p.Id == id);

    public bool TryGetById(sbyte id, [NotNullWhen(true)] out RemotePlayer? player)
    {
        return (player = Players.FirstOrDefault(p => p.Id == id)) != null;
    }
    
    public void ProcessPacket(ref readonly IPacket packet)
    {
        RemotePlayer? player;
        switch (packet)
        {
            case SpawnPlayerS2CPacket spawn:
                if (spawn.PlayerId < 0) break; // ignore local player
                if (!IsConnected(spawn.PlayerId))
                {
                    player = new RemotePlayer(_manager, spawn);
                    Players.Add(player);
                    Connected?.Invoke(player);
                }

                break;
            case DespawnPlayerS2CPacket despawn:
                if (despawn.PlayerId < 0) break; // ignore local player
                if (TryGetById(despawn.PlayerId, out player))
                {
                    Players.Remove(player);
                    Disconnected?.Invoke(player);
                }

                break;
            case MessagePacket message:
                if (TryGetById(message.PlayerId, out player))
                {
                    Message?.Invoke(player, message.SanitizedContent);
                }

                break;
        }
    }
    
    public void Dispose()
    {
        _manager.Processors.Remove(this);
    }
}