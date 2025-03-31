using System.Diagnostics.CodeAnalysis;
using LocalBedShitter.Networking.Packets.Both;
using LocalBedShitter.Networking.Packets.C2S;
using LocalBedShitter.Networking.Packets.S2C;

namespace LocalBedShitter.Networking.Packets;

public static class PacketManager
{
    private static readonly Dictionary<byte, Type> IncomingById = [];
    private static readonly Dictionary<Type, byte> OutgoingByType = [];
    
    public static void RegisterIncoming<T>(byte id)
        where T : IPacket
    {
        IncomingById[id] = typeof(T);
    }
    
    public static void RegisterOutgoing<T>(byte id)
        where T : IPacket
    {
        OutgoingByType[typeof(T)] = id;
    }

    public static bool TryRead(Stream stream, [NotNullWhen(true)] out IPacket? packet)
    {
        packet = null;
        
        // read first byte to find out packet type
        int nid = stream.ReadByte();
        if (nid < 0) return false; // eos
        byte id = (byte)nid;

        // now we can get type of the packet
        if (!IncomingById.TryGetValue(id, out Type? type)) return false;

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
        if (!OutgoingByType.TryGetValue(packet.GetType(), out byte id)) return;
        
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
        RegisterOutgoing<PlayerIdC2SPacket>(0x00);
        RegisterIncoming<ServerIdS2CPacket>(0x00);
        RegisterIncoming<PingS2CPacket>(0x01);
        RegisterIncoming<LevelInitializeS2CPacket>(0x02);
        RegisterIncoming<LevelChunkS2CPacket>(0x03);
        RegisterIncoming<LevelFinalizeS2CPacket>(0x04);
        RegisterOutgoing<SetBlockC2SPacket>(0x05);
        RegisterIncoming<SetBlockS2CPacket>(0x06);
        RegisterIncoming<SpawnPlayerS2CPacket>(0x07);
        RegisterIncoming<TeleportPacket>(0x08);
        RegisterOutgoing<TeleportPacket>(0x08);
        RegisterIncoming<UpdateTransformS2CPacket>(0x09);
        RegisterIncoming<MoveS2CPacket>(0x0a);
        RegisterIncoming<RotateS2CPacket>(0x0b);
        RegisterIncoming<DespawnPlayerS2CPacket>(0x0c);
        RegisterIncoming<MessagePacket>(0x0d);
        RegisterOutgoing<MessagePacket>(0x0d);
        RegisterIncoming<DisconnectPlayerS2CPacket>(0x0e);
        RegisterIncoming<UpdateUserTypeS2CPacket>(0x0f);
    }
}