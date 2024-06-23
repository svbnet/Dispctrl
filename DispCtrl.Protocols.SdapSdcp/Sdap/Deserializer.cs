using System.Net;
using System.Runtime.Serialization;

namespace DispCtrl.Protocols.SdapSdcp.Sdap;

internal static class Deserializer
{
    private static readonly char[] HeaderMagic = ['D', 'A'];

    public static void DeserializePacket(PacketSerializer reader, SdapDevice device)
    {
        var headerDa = reader.ReadChars(2);
        if (!headerDa.SequenceEqual(HeaderMagic))
        {
            throw new SerializationException($"{device.IpAddress}: Packet header malformed");
        }

        device.Version = reader.ReadByte();
        device.Category = (SdapDeviceCategory)reader.ReadByte();
        device.Community = reader.ReadFixedAsciiCString(4);
        device.ProductName = reader.ReadFixedAsciiCString(12);
        device.SerialNumber = (int) reader.ReadUInt32();
        device.PowerStatus = reader.ReadFixedAsciiCString(2);
        device.Location = reader.ReadFixedAsciiCString(24);

        if (device.Version >= 4) DeserializePacket4(reader, device);
        if (device.Version >= 3) DeserializePacket3(reader, device); 
        if (device.Version >= 2) DeserializePacket2(reader, device);
    }

    private static void DeserializePacket2(PacketSerializer reader, SdapDevice device)
    {
        device.ConnectionIp = new IPAddress(reader.ReadBytes(4));
        var ips = new List<IPAddress>();
        for (var i = 0; i < 4; i++)
        {
            ips.Add(new IPAddress(reader.ReadBytes(4)));
        }

        device.AcceptableIps = ips.ToArray();
    }

    private static void DeserializePacket3(PacketSerializer reader, SdapDevice device)
    {
        device.Error = reader.ReadUInt16();
        device.Region = reader.ReadFixedAsciiCString(24);
        device.Name = reader.ReadFixedAsciiCString(24);
    }

    private static void DeserializePacket4(PacketSerializer reader, SdapDevice device)
    {
        device.GroupId = reader.ReadByte();
        device.UnitId = reader.ReadByte();
    }

}