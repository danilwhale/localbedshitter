using System.Numerics;
using LocalBedShitter.Networking;
using LocalBedShitter.Networking.Packets;
using LocalBedShitter.Networking.Packets.C2S;
using LocalBedShitter.Networking.Packets.S2C;

namespace LocalBedShitter.API.Players;

public sealed class LocalPlayer(NetworkManager manager, string username)
    : Player(manager, -1, username)
{
    public bool IsAuthenticated;
    public UserType UserType;
    
    public void Authenticate(string mpPass)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, typeof(LocalPlayer));
        if (IsAuthenticated) throw new InvalidOperationException("Already authenticated!");
        Manager.SendPacket(new PlayerIdC2SPacket(Username, mpPass));
        IsAuthenticated = true;
    }
    
    public void Teleport(Vector3 position, Vector2 rotation)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, typeof(LocalPlayer));
        Manager.SendPacket(new TeleportC2SPacket(position, rotation));
        Position = position;
        Rotation = rotation;
    }

    public void Teleport(Player player) => Teleport(player.Position, player.Rotation);

    public void SetBlock(BlockPos pos, EditMode mode, byte type)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, typeof(LocalPlayer));
        Manager.SendPacket(new SetBlockC2SPacket(pos, mode, type));
    }
    
    public void SendMessage(string message)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, typeof(LocalPlayer));
        Manager.SendPacket(new MessageC2SPacket(message));
    }

    public override void ProcessPacket(ref readonly IPacket packet)
    {
        base.ProcessPacket(in packet);
        switch (packet)
        {
            case ServerIdS2CPacket serverId:
                UserType = serverId.UserType;
                break;
            case UpdateUserTypeS2CPacket updateUserType:
                UserType = updateUserType.UserType;
                break;
        }
    }
}