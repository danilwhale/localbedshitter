using System.Text.RegularExpressions;

namespace LocalBedShitter.Networking.Packets.Both;

public partial struct MessagePacket(string content) : IPacket
{
    public const int SizeInBytes = sizeof(sbyte) + 64;

    public int Length => SizeInBytes;
    
    private sbyte _unused = -1;
    public string Content = content;
    public string? Author
    {
        get
        {
            // NOTE: servers may use different message formatting and this *will* break
            // but here we except it to be generic formatting: 'USERNAME: MESSAGE'
            int colonIndex = Content.IndexOf(':');
            return colonIndex < 0 ? null : Content[..colonIndex];
        }
    }

    public void Read(ref PacketReader reader)
    {
        _unused = reader.ReadSByte();
        Content = reader.ReadString();
    }

    public void Write(ref PacketWriter writer)
    {
        writer.WriteSByte(_unused);
        writer.WriteString(Content);
    }
}