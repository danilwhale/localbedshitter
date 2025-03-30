namespace LocalBedShitter.Networking.Packets.C2S;

public struct PlayerIdC2SPacket(string username, string mpPass) : IPacket
{
    public const int SizeInBytes = sizeof(byte) + 64 + 64 + sizeof(byte);
    
    public int Length => SizeInBytes;

    public byte ProtocolVersion = 0x07;
    public string Username = username;
    public string MpPass = mpPass;
    private byte _unused = 0;
    
    public void Read(PacketReader reader)
    {
        ProtocolVersion = reader.ReadByte();
        Username = reader.ReadString();
        MpPass = reader.ReadString();
        _unused = reader.ReadByte();
    }

    public void Write(PacketWriter writer)
    {
        writer.WriteByte(ProtocolVersion);
        writer.WriteString(Username);
        writer.WriteString(MpPass);
        writer.WriteByte(_unused);
    }
}