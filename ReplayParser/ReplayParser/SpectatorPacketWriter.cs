using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayParser
{
    class SpectatorPacketWriter
    {
        private List<SpectatorPacket> packets;

        public SpectatorPacketWriter(List<SpectatorPacket> _packets)
        {
            packets = _packets;
        }
        public void write(string file)
        {
            string output = "";
            foreach (SpectatorPacket packet in packets)
            {
                output += "Packet type = " + packet.packetType + " ,Length = " + packet.length + " ,Time = " + packet.time + " ,Netid = " + packet.blockparam + "\n";
                output += BitConverter.ToString(packet.packetData).Replace('-', ' ') + "\n";
            }
            file = Path.GetFileNameWithoutExtension(file);
            file += ".txt";

            if (File.Exists(file))
                File.Delete(file);
            File.AppendAllText(file, output);
        }
    }
}
