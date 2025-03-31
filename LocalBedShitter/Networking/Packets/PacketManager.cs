using System.Diagnostics.CodeAnalysis;
using LocalBedShitter.Networking.Packets.Both;
using LocalBedShitter.Networking.Packets.C2S;
using LocalBedShitter.Networking.Packets.S2C;

namespace LocalBedShitter.Networking.Packets;

public static class PacketManager
{
    private static readonly Dictionary<byte, Type> ById = [];
    private static readonly Dictionary<Type, byte> ByType = [];

    public static void
        Register<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(byte id)
        where T : IPacket
    {
        ById[id] = typeof(T);
        ByType[typeof(T)] = id;
    }

    public static bool TryRead(Stream stream, [NotNullWhen(true)] out IPacket? packet)
    {
        packet = null;
        
        // read first byte to find out packet type
        int nid = stream.ReadByte();
        if (nid < 0) return false; // eos
        byte id = (byte)nid;

        // now we can get type of the packet
        if (!ById.TryGetValue(id, out Type? type)) return false;

        // finally create the packet
        packet = Activator.CreateInstance(type) as IPacket;
        if (packet == null) return false;
        
        // now read the data
        if (packet.Length > 0)
        {
            Span<byte> buffer = stackalloc byte[packet.Length];
            stream.ReadExactly(buffer);
            PacketReader reader = new(buffer);
            packet.Read(ref reader);
        }

        return true;
    }

    public static void Write(Stream stream, IPacket packet)
    {
        // find id of the packet
        if (!ByType.TryGetValue(packet.GetType(), out byte id)) return;
        
        // now we can write the data
        Span<byte> buffer = stackalloc byte[packet.Length + 1 /* for packet id */];
        PacketWriter writer = new(buffer);
        writer.WriteByte(id);
        if (packet.Length > 0)
        {
            packet.Write(ref writer);
        }

        // and then send it to the stream
        stream.Write(buffer);
    }

    static PacketManager()
    {
        Register<PlayerIdC2SPacket>(0x00);
        Register<SetBlockC2SPacket>(0x05);
        
        Register<ServerIdS2CPacket>(0x00);
        Register<PingS2CPacket>(0x01);
        Register<LevelInitializeS2CPacket>(0x02);
        Register<LevelChunkS2CPacket>(0x03);
        Register<LevelFinalizeS2CPacket>(0x04);
        Register<SetBlockS2CPacket>(0x06);
        Register<SpawnPlayerS2CPacket>(0x07);
        Register<UpdateTransformS2CPacket>(0x09);
        Register<MoveS2CPacket>(0x0a);
        Register<RotateS2CPacket>(0x0b);
        Register<DespawnPlayerS2CPacket>(0x0c);
        Register<DisconnectPlayerS2CPacket>(0x0e);
        Register<UpdateUserTypeS2CPacket>(0x0f);
        
        Register<MessagePacket>(0x0d);
        Register<TeleportPacket>(0x08);
    }
}