using System.Buffers.Binary;
using System.Text;

namespace LocalBedShitter.Networking.Packets;

// https://minecraft.wiki/w/Classic_server_protocol#Packet_protocol
public ref struct PacketWriter(Span<byte> buffer)
{
    private static readonly Encoding Ibm437 = CodePagesEncodingProvider.Instance.GetEncoding("IBM437")!;
    
    public readonly Span<byte> Buffer = buffer;
    public int Position;

    public void WriteByte(byte value) => Buffer[Position++] = value;
    public void WriteSByte(sbyte value) => Buffer[Position++] = (byte)value;
    public void WriteShort(short value)
    {
        BinaryPrimitives.WriteInt16BigEndian(Buffer[Position..], value);
        Position += sizeof(short);
    }

    public void WriteString(string value)
    {
        Buffer[Position..(Position + 64)].Fill(0x20);
        Ibm437.GetBytes(value, Buffer[Position..(Position + 64)]);
        Position += 64;
    }

    public void WriteBytes(scoped ReadOnlySpan<byte> value)
    {
        value.CopyTo(Buffer[Position..]);
        Position += value.Length;
    }
}