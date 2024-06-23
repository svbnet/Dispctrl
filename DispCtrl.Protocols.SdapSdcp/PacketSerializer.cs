using System.Buffers.Binary;
using System.Text;

namespace DispCtrl.Protocols.SdapSdcp;

public class PacketSerializer
{
    private byte[] data;
    private int nextPosition = 0;

    public byte[] Data => data;

    public PacketSerializer(byte[] data)
    {
        this.data = data;
    }

    public PacketSerializer(int length)
    {
        data = new byte[length];
    }

    private int Advance(int amount)
    {
        var peekNext = nextPosition + amount;
        if (peekNext > data.Length) throw new InvalidOperationException("Cannot move past the end of the data");
        var index = nextPosition;
        nextPosition = peekNext;
        return index;
    }

    private Span<byte> SliceAndAdvance(int amount)
    {
        return new Span<byte>(data, Advance(amount), amount);
    }

    public byte ReadByte()
    {
        return data[Advance(1)];
    }

    public byte[] ReadBytes(int length)
    {
        var bytes = SliceAndAdvance(length);
        return bytes.ToArray();
    }

    public char[] ReadChars(int length)
    {
        return Encoding.ASCII.GetChars(ReadBytes(length));
    }

    public string ReadFixedAsciiCString(int length)
    {
        var chars = Encoding.ASCII.GetChars(data, Advance(length), length);
        var nullIdx = Array.IndexOf(chars, '\x00');
        return nullIdx < 0 ? new string(chars) : new string(chars, 0, nullIdx);
    }

    public int ReadInt32()
    {
        return BinaryPrimitives.ReadInt32BigEndian(SliceAndAdvance(sizeof(int)));
    }

    public uint ReadUInt32()
    {
        return BinaryPrimitives.ReadUInt32BigEndian(SliceAndAdvance(sizeof(uint)));
    }

    public ushort ReadUInt16()
    {
        return BinaryPrimitives.ReadUInt16BigEndian(SliceAndAdvance(sizeof(ushort)));
    }

    public short ReadInt16()
    {
        return BinaryPrimitives.ReadInt16BigEndian(SliceAndAdvance(sizeof(short)));
    }

    public ulong ReadUInt64()
    {
        return BinaryPrimitives.ReadUInt64BigEndian(SliceAndAdvance(sizeof(ulong)));
    }

    public long ReadInt64()
    {
        return BinaryPrimitives.ReadInt64BigEndian(SliceAndAdvance(sizeof(long)));
    }

    private void ResizeIfNeeded(int size)
    {
        if (size + nextPosition <= data.Length) return;
        Array.Resize(ref data, data.Length - (data.Length - nextPosition) + size);
    }

    public void Write(byte b)
    {
        ResizeIfNeeded(sizeof(byte));
        data[nextPosition++] = b;
    }

    public void Write(byte[] bytes)
    {
        ResizeIfNeeded(bytes.Length);
        Array.Copy(bytes, 0, data, nextPosition, bytes.Length);
        nextPosition += bytes.Length;
    }

    public void Write(string str)
    {
        var bytes = Encoding.ASCII.GetBytes(str);
        ResizeIfNeeded(bytes.Length);
        Array.Copy(bytes, 0, data, nextPosition, bytes.Length);
        nextPosition += bytes.Length;
    }

    public void Write(ushort u)
    {
        ResizeIfNeeded(sizeof(ushort));
        var slice = new Span<byte>(data, nextPosition, sizeof(ushort));
        BinaryPrimitives.WriteUInt16BigEndian(slice, u);
        nextPosition += sizeof(ushort);
    }

}