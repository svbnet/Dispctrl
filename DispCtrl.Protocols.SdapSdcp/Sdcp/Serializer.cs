using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DispCtrl.Protocols.SdapSdcp.Sdcp
{
    /**
     * |---------|--------------------|-----------------------|----------|---------|----------|---------|------------------|--------|
     * | Version | Category           | Community             | Group ID | Unit ID | Req/Resp | Item no | Data length      | Data   |
     * | byte    | byte (always 0x0b) | char[4] (always SONY) | byte     | byte    | byte     | ushort  | ushort (max 499) | byte[] |
     * |---------|--------------------|-----------------------|----------|---------|----------|---------|------------------|--------|
     * - Group/Unit is configured on the monitor
     */
    internal static class Serializer
    {
        private const byte Category = 0x0b;
        private const string Community = "SONY";

        // As above with no data
        internal const int MinPacketLength = 1 + 1 + 4 + 1 + 1 + 1 + 2 + 2;

        public static void Write(PacketSerializer writer, SdcpPacket packet)
        {
            // Version
            writer.Write(packet.Version);
            // Category
            writer.Write(Category);
            // Community
            writer.Write(Community);
            // Group ID
            writer.Write(packet.GroupId);
            // Unit ID
            writer.Write(packet.UnitId);
            // Req/resp
            writer.Write(packet.RequestResponse);
            // Item no
            writer.Write((ushort) packet.ItemNo);
            // Length
            if (packet.Data.Length > 499) throw new ArgumentException("Data too long (<= 499)");
            writer.Write((ushort) packet.Data.Length);
            // Data
            writer.Write(packet.Data);
        }

        public static void Read(PacketSerializer reader, SdcpPacket packet)
        {
            packet.Version = reader.ReadByte();
            var category = reader.ReadByte();
            if (category != Category)
                throw new SerializationException(
                    $"SDCP serializer read: Category: expected {Category} but got {category}");
            var community = reader.ReadFixedAsciiCString(Community.Length);
            if (community != Community)
                throw new SerializationException(
                    $"SDCP serializer read: Community: expected {Community} but got {community}");
            packet.GroupId = reader.ReadByte();
            packet.UnitId = reader.ReadByte();
            packet.RequestResponse = reader.ReadByte();
            packet.ItemNo = (MonitorCommandKind) reader.ReadUInt16();
            var dataLength = reader.ReadUInt16();
            packet.Data = reader.ReadBytes(dataLength);
        }
    }
}
