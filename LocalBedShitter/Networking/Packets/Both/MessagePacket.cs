using System.Text.RegularExpressions;

namespace LocalBedShitter.Networking.Packets.Both;

public partial struct MessagePacket(sbyte playerId, string content) : IPacket
{
    public const int SizeInBytes = sizeof(sbyte) + 64;

    public int Length => SizeInBytes;

    public string SanitizedContent => ColorCodeRegex().Replace(Content, "");
    
    public sbyte PlayerId = playerId;
    public string Content = content;
    
    public void Read(PacketReader reader)
    {
        PlayerId = reader.ReadSByte();
        Content = reader.ReadString();
    }

    public void Write(PacketWriter writer)
    {
        writer.WriteSByte(PlayerId);
        writer.WriteString(Content);
    }

    [GeneratedRegex("&[0-9a-fA-F]")]
    private static partial Regex ColorCodeRegex();
}