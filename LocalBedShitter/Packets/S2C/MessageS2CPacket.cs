using System.Text;

namespace LocalBedShitter.Packets.S2C;

public struct MessageS2CPacket : IPacket
{
    public const int SizeInBytes = sizeof(sbyte) + 64;
    
    public int Length => SizeInBytes;

    public sbyte PlayerId;
    public string Content;

    public string SanitizedContent
    {
        get
        {
            StringBuilder sb = new();
            for (int i = 0; i < Content.Length; i++)
            {
                if (Content[i] == '&')
                {
                    i++;
                    continue;
                }

                sb.Append(Content[i]);
            }

            return sb.ToString();
        }
    }
    
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
}