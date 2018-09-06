using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static ReplayParser.ReplayFile;

namespace ReplayParser
{
    public class PacketReader
    {
        private List<Packet> PacketBuffer = new List<Packet>();
        private List<SpectatorPacket> specPackets = new List<SpectatorPacket>();
        private long ReadLength;
        private BinaryReader replayFile;
        public bool loaded;

        public PacketReader(ReplayStream reader, bool spectator)
        {
            loaded = Load(reader, spectator);
        }

        public bool Load(ReplayStream reader, bool spectator)
        {
            if (reader == null)
                return false;

            replayFile = reader.reader;
            ReadLength = reader.length + replayFile.BaseStream.Position;

            try
            {
                while (replayFile.BaseStream.Position < ReadLength)
                {
                    var file = replayFile;
                    var packet = new Packet(file);
                    if (spectator)
                    {
                        SpectatorResponse response = new SpectatorResponse(packet.Bytes);
                        lock (specPackets)
                            specPackets.AddRange(response.GetPackets());
                    }
                    else
                    {
                        lock (PacketBuffer)
                            PacketBuffer.Add(packet);
                    }
                }
            }
            catch { }

            return true;
        }

        public void writeToFile(string file)
        {
            if (File.Exists(file))
                File.Delete(file);

            foreach (var p in PacketBuffer)
            {
                var text = BitConverter.ToString(p.Bytes).Replace('-', ' ');
                File.AppendAllText(file, text + Environment.NewLine);
            }
        }

        public List<Packet> getPackets()
        {
            return PacketBuffer;
        }
        public List<SpectatorPacket> getSpectatorPackets()
        {
            return specPackets;
        }
    }
}
