using System.Net;
using System.Net.Sockets;
using DispCtrl.Protocols.SdapSdcp.Sdap;

namespace DispCtrl.Protocols.SdapSdcp.Sdcp;

public class SdcpClient(IPAddress address) : IDisposable
{
    public const int WorkPort = 53484;
    private const int MaxBufferSize = 256;

    private readonly TcpClient client = new();
    private readonly SdapDevice? device;
    private NetworkStream? stream;

    public SdcpClient(SdapDevice device) : this(device.IpAddress)
    {
        this.device = device;
    }

    public IPAddress Address => address;

    public async Task ConnectAsync()
    {
        await client.ConnectAsync(address, WorkPort);
        stream = client.GetStream();
    }

    public async Task<SdcpResponse> SendPacketAsync(byte[] vmcCommand)
    {
        if (stream is null) throw new InvalidOperationException("Must call ConnectAsync first");

        var writer = new PacketSerializer(Serializer.MinPacketLength);

        Serializer.Write(writer, MakeRequestPacket(vmcCommand));
        await stream.WriteAsync(writer.Data);

        var respData = new byte[MaxBufferSize];
        // ReSharper disable once MustUseReturnValue
        await stream.ReadAtLeastAsync(respData, Serializer.MinPacketLength);
        
        var reader = new PacketSerializer(respData);
        var packet = new SdcpPacket();
        Serializer.Read(reader, packet);

        return new SdcpResponse
        {
            Success = packet.RequestResponse == 1,
            Data = packet.Data
        };
    }

    private SdcpPacket MakeRequestPacket(byte[] data)
    {
        var packet = new SdcpPacket
        {
            Version = SdcpPacket.DefaultVersion,
            GroupId = 0,
            UnitId = 0,
            ItemNo = MonitorCommandKind.Monitor,
            RequestResponse = 0,
            Data = data
        };
        if (device is not null)
        {
            packet.UnitId = (byte) device.UnitId.GetValueOrDefault();
        }

        return packet;
    }

    public void Dispose()
    {
        stream?.Dispose();
        client.Dispose();
    }
}