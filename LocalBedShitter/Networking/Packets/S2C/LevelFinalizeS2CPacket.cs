﻿namespace LocalBedShitter.Networking.Packets.S2C;

public struct LevelFinalizeS2CPacket : IPacket
{
    public const int SizeInBytes = 3 * sizeof(short);
    
    public int Length => SizeInBytes;

    public short Width;
    public short Height;
    public short Depth;
    
    public void Read(ref PacketReader reader)
    {
        Width = reader.ReadShort();
        Height = reader.ReadShort();
        Depth = reader.ReadShort();
    }

    public void Write(ref PacketWriter writer)
    {
        writer.WriteShort(Width);
        writer.WriteShort(Height);
        writer.WriteShort(Depth);
    }
}