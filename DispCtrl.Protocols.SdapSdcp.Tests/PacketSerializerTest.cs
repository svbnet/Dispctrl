namespace DispCtrl.Protocols.SdapSdcp.Tests;

[TestClass]
public class PacketSerializerTest
{
    private static readonly byte[] Packet = [ 0xde, 0xad, 0xbe, 0xef, 0x48, 0x45, 0x4c, 0x4c, 0x4f, 0x0, 0x0, 0x0 ];

    [TestMethod]
    public void ReadByte_AtStart()
    {
        var serializer = new PacketSerializer(Packet);
        Assert.AreEqual(0xde, serializer.ReadByte());
    }

    [TestMethod]
    public void ReadBytes_AtStart()
    {
        var serializer = new PacketSerializer(Packet);
        CollectionAssert.AreEqual(new byte[] { 0xde, 0xad, 0xbe, 0xef}, serializer.ReadBytes(4));
    }

    [TestMethod]
    public void ReadUShort_AtStart()
    {
        var serializer = new PacketSerializer(Packet);
        Assert.AreEqual(57005, serializer.ReadUInt16());
    }

    [TestMethod]
    public void ReadUInt_AtStart()
    {
        var serializer = new PacketSerializer(Packet);
        Assert.AreEqual(3735928559, serializer.ReadUInt32());
    }

    [TestMethod]
    public void ReadByte_UShort_UInt()
    {
        var serializer = new PacketSerializer(Packet);
        Assert.AreEqual(0xde, serializer.ReadByte());
        Assert.AreEqual(44478, serializer.ReadUInt16());
        Assert.AreEqual(4014490956, serializer.ReadUInt32());
    }

    [TestMethod]
    public void ReadUint_StringExactLength()
    {
        var serializer = new PacketSerializer(Packet);
        Assert.AreEqual(3735928559, serializer.ReadUInt32());
        Assert.AreEqual("HELLO", serializer.ReadFixedAsciiCString(5));
    }

    [TestMethod]
    public void ReadUint_StringMaxLength()
    {
        var serializer = new PacketSerializer(Packet);
        Assert.AreEqual(3735928559, serializer.ReadUInt32());
        Assert.AreEqual("HELLO", serializer.ReadFixedAsciiCString(8));
    }

    [TestMethod]
    public void ReadBytes_UInt_After_End_Throws()
    {
        var serializer = new PacketSerializer(Packet);
        serializer.ReadBytes(10);
        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            serializer.ReadUInt32();
        });
    }

    [TestMethod]
    public void Read_All_Bytes_Byte_After_End_Throws()
    {
        var serializer = new PacketSerializer(Packet);
        serializer.ReadBytes(12);
        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            serializer.ReadByte();
        });
    }

    [TestMethod]
    public void ReadBytes_More_Bytes_After_End_Throws()
    {
        var serializer = new PacketSerializer(Packet);
        serializer.ReadBytes(10);
        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            serializer.ReadBytes(6);
        });
    }

    [TestMethod]
    public void WriteByte_NoExpand()
    {
        var serializer = new PacketSerializer(1);
        serializer.Write((byte) 0xce);
        CollectionAssert.AreEqual(new byte[] { 0xce }, serializer.Data);
    }

    [TestMethod]
    public void WriteBytes_NoExpand()
    {
        var serializer = new PacketSerializer(4);
        serializer.Write(new byte[] { 0xde, 0xad, 0xbe, 0xef });
        CollectionAssert.AreEqual(new byte[] { 0xde, 0xad, 0xbe, 0xef }, serializer.Data);
    }

    [TestMethod]
    public void WriteString_NoExpand()
    {
        var serializer = new PacketSerializer(5);
        serializer.Write("Hello");
        CollectionAssert.AreEqual("Hello"u8.ToArray(), serializer.Data);
    }

    [TestMethod]
    public void WriteString_Bytes_UShort_NoExpand()
    {
        var serializer = new PacketSerializer(11);
        serializer.Write("Hello");
        serializer.Write(new byte[] { 0xde, 0xad, 0xbe, 0xef });
        serializer.Write(61864);
        CollectionAssert.AreEqual(new byte[] { 0x48, 0x65, 0x6c, 0x6c, 0x6f, 0xde, 0xad, 0xbe, 0xef, 0xf1, 0xa8 }, serializer.Data);
    }

    [TestMethod]
    public void WriteString_Bytes_UShort_Expand()
    {
        var serializer = new PacketSerializer(6);
        serializer.Write("Hello");
        serializer.Write(new byte[] { 0xde, 0xad, 0xbe, 0xef });
        serializer.Write(61864);
        CollectionAssert.AreEqual(new byte[] { 0x48, 0x65, 0x6c, 0x6c, 0x6f, 0xde, 0xad, 0xbe, 0xef, 0xf1, 0xa8 }, serializer.Data);
    }
}