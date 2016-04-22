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
        private long ReadLength;
        private BinaryReader replayFile;
        public bool loaded;

        public PacketReader(ReplayStream reader)
        {
            loaded = Load(reader);
        }

        public bool Load(ReplayStream reader)
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
                    lock (PacketBuffer)
                        PacketBuffer.Add(packet);
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
    }
}
