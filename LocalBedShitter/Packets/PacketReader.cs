using System.Buffers.Binary;
using System.Text;

namespace LocalBedShitter.Packets;

// https://minecraft.wiki/w/Classic_server_protocol#Packet_protocol
public ref struct PacketReader(ReadOnlySpan<byte> buffer)
{
    private static readonly Encoding Ibm437 = CodePagesEncodingProvider.Instance.GetEncoding("IBM437")!;
    
    public readonly ReadOnlySpan<byte> Buffer = buffer;
    public int Position;

    public byte ReadByte() => Buffer[Position++];
    public sbyte ReadSByte() => (sbyte)Buffer[Position++];
    public short ReadShort()
    {
        short value = BinaryPrimitives.ReadInt16BigEndian(Buffer[Position..]);
        Position += sizeof(short);
        return value;
    }

    public string ReadString()
    {
        string value = Ibm437.GetString(Buffer[Position..(Position + 64)]).TrimEnd(' ');
        Position += 64;
        return value;
    }

    public ReadOnlySpan<byte> ReadBytes(int length)
    {
        ReadOnlySpan<byte> value = Buffer[Position..(Position + length)];
        Position += length;
        return value;
    }
}