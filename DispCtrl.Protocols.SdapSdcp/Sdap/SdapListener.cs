using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DispCtrl.Protocols.SdapSdcp.Sdap;
public class SdapListener : IDisposable
{
    private const int ListenPort = 53862;

    public event EventHandler<DeviceFoundEventArgs>? DeviceFound;

    private readonly int timeout = 15000;
    private UdpClient? listener;
    private DateTime? startTime;

    public SdapListener()
    {
        
    }

    public SdapListener(int timeout)
    {
        this.timeout = timeout;
    }

    public async Task BeginDiscoveryAsync()
    {
        listener = new UdpClient(ListenPort);
        startTime = DateTime.Now;
        while (listener is not null && startTime is not null && (startTime?.AddMilliseconds(timeout) > DateTime.Now))
        {
            var resp = await listener.ReceiveAsync();
            var device = new SdapDevice
            {
                IpAddress = resp.RemoteEndPoint.Address
            };
            using (var memStream = new MemoryStream(resp.Buffer))
            {
                var reader = new PacketSerializer(resp.Buffer);
                Deserializer.DeserializePacket(reader, device);
            }

            OnDeviceFound(device);
        }
    }

    public void EndDiscovery()
    {
        if (listener is null) return;

        startTime = null;
        listener.Close();
        listener.Dispose();
        listener = null;
    }

    protected void OnDeviceFound(SdapDevice device)
    {
        DeviceFound?.Invoke(this, new DeviceFoundEventArgs { Device = device });
    }

    public void Dispose()
    {
        listener?.Dispose();
    }
}

