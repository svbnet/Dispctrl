using System.Net;
using System.Text;
using Dispctrl;
using DispCtrl.Protocols.SdapSdcp.Sdap;
using DispCtrl.Protocols.SdapSdcp.Sdcp;

namespace DispCtrl;

public static class Program
{
    private static readonly SdapListener Listener = new();
    private static readonly List<SdapDevice> Devices = [];

    private static SdcpClient? client;

    public static async Task<SdapDevice> ScanDevices()
    {
        while (true)
        {
            Listener.EndDiscovery();
            Devices.Clear();
            Console.WriteLine("Scanning for devices...");
            await Listener.BeginDiscoveryAsync();
            var deviceLabels = Devices.Select(InspectDevice);
            var choice = TextUi.ChoicePrompt("Choose a device or refresh:", deviceLabels.Prepend("Refresh").ToArray());
            if (choice == 1) continue;
            var idx = choice - 2;
            return Devices[idx];
        }
    }

    public static string InspectDevice(SdapDevice device)
    {
        return
            $"[{device.IpAddress}] {device.ProductName} (v{device.Version}, {device.GroupId}:{device.UnitId}, s/n {device.SerialNumber})";
    }

    public static string InspectBytes(byte[] bytes)
    {
        return string.Join(' ', bytes.Select((b) => $"0x{b:X}"));
    }

    public static async Task DoConsole()
    {
        if (client is null) return;
        Console.WriteLine($"Connecting to {client.Address}...");
        await client.ConnectAsync();
        while (true)
        {
            Console.Write($"{client.Address}> ");
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) continue;
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) return;

            SdcpResponse resp;
            try
            {
                resp = await client.SendPacketAsync(Encoding.ASCII.GetBytes(input));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                continue;
            }

            var msg = resp.Success ? "OK" : "ERROR";
            Console.WriteLine($"{msg}, Data = {{{InspectBytes(resp.Data)}}} ({Encoding.ASCII.GetString(resp.Data)})");
            
            Console.WriteLine();
        }
    }

    public static async Task Main()
    {
        Listener.DeviceFound += (_, args) =>
        {
            Console.WriteLine(InspectDevice(args.Device));
            Devices.Add(args.Device);
        };
        var device = await ScanDevices();
        client?.Dispose();

        client = new SdcpClient(device.IpAddress);
        await DoConsole();
        client.Dispose();
        Listener.Dispose();
    }

}


