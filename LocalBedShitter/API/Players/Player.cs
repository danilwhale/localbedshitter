using System.Numerics;
using LocalBedShitter.Networking;
using LocalBedShitter.Networking.Packets;
using LocalBedShitter.Networking.Packets.Both;
using LocalBedShitter.Networking.Packets.S2C;

namespace LocalBedShitter.API.Players;

public abstract class Player : IPacketProcessor
{
    public readonly sbyte Id;
    public readonly string Username;
    public BlockPos BlockPos => new(FastMath.Floor(Position.X), FastMath.Floor(Position.Y), FastMath.Floor(Position.Z));
    public Vector3 Position;
    public Vector2 Rotation;
    
    protected readonly NetworkManager Manager;

    protected Player(NetworkManager manager, sbyte id, string username)
    {
        Manager = manager;
        manager.Processors.Add(this);

        Id = id;
        Username = username;
    }
    
    public virtual void ProcessPacket(ref readonly IPacket packet)
    {
        switch (packet)
        {
            case TeleportPacket teleport when teleport.PlayerId == Id:
                Position = teleport.Position;
                Rotation = teleport.Rotation;
                break;
            case UpdateTransformS2CPacket transform when transform.PlayerId == Id:
                Position += transform.PositionDelta;
                Rotation = transform.Rotation;
                break;
            case MoveS2CPacket move when move.PlayerId == Id:
                Position += move.Delta;
                break;
            case RotateS2CPacket rotate when rotate.PlayerId == Id:
                Rotation += rotate.Delta;
                break;
            case SpawnPlayerS2CPacket spawn when spawn.PlayerId == Id:
                Position = spawn.Position;
                Rotation = spawn.Rotation;
                break;
        }
    }
}