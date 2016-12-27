using BlowFishCS;
using Newtonsoft.Json;
using PacketDotNet;
using PcapDecrypt.Json;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using static BlowFishCS.BlowFishCS;

namespace PcapDecrypt
{
    unsafe class Program
    {
        private static BlowFish* _blowfish;
        private static Dictionary<int, Dictionary<int, byte[]>> fragmentBuffer = new Dictionary<int, Dictionary<int, byte[]>>();
        private static List<string> toWrite = new List<string>();
        private static SortedSet<PacketCmd> knownPackets = new SortedSet<PacketCmd>();
        private static SortedSet<ExtendedPacketCmd> knownExtPackets = new SortedSet<ExtendedPacketCmd>();
        private static SortedSet<PacketCmd> unknownPackets = new SortedSet<PacketCmd>();
        private static int[] unknownPacketsCount = new int[256];
        private static SortedSet<ExtendedPacketCmd> unknownExtPackets = new SortedSet<ExtendedPacketCmd>();
        private static int[] unknownExtPacketsCount = new int[256];
        private static SortedSet<PacketCmd> unknownPacketsNotInBatch = new SortedSet<PacketCmd>();
        private static int[] unknownPacketsNotInBatchCount = new int[256];
        private static SortedSet<ExtendedPacketCmd> unknownExtPacketsNotInBatch = new SortedSet<ExtendedPacketCmd>();
        private static int[] unknownExtPacketsNotInBatchCount = new int[256];
        private static float lastPacketTime;
        private static bool firstPacketReceived;

        static void Main(string[] args)
        {
            if (args.Length < 1)
                return;

            Console.Write("running...");

            if (args[0].ToLower().EndsWith(".pcap"))
            {
                if (args.Length < 2)
                    return;

                initBlowfish(args[1]);

                var f = new CaptureFileReaderDevice(args[0]);
                f.Open();
                f.OnPacketArrival += F_OnPacketArrival;
                f.Capture();
                f.Close();
            }
            else if (args[0].ToLower().EndsWith(".json"))
            {
                var json = File.ReadAllText(args[0]);
                var replay = JsonConvert.DeserializeObject<Replay>(json);

                initBlowfish(replay.encryptionKey);
                parsePackets(replay.packets);
            }
            else
            {
                Console.WriteLine("unknown input file.");
                Console.ReadLine();
                return;
            }

            if (File.Exists("decrypted.txt"))
                File.Delete("decrypted.txt");

            toWrite.Add(Environment.NewLine);
            toWrite.Add("Number of known packets: " + knownPackets.Count);
            toWrite.Add("Number of known extended packets: " + knownExtPackets.Count);
            toWrite.Add("Number of unknown packets: " + unknownPackets.Count);
            toWrite.Add("Number of unknown extended packets: " + unknownExtPackets.Count);
            toWrite.Add("Number of unknown packets not in batch: " + unknownPacketsNotInBatch.Count);
            toWrite.Add("Number of unknown extended packets not in batch: " + unknownExtPacketsNotInBatch.Count);
            toWrite.Add("Unknown packets list:" + Environment.NewLine);
            foreach (var p in unknownPackets)
            {
                toWrite.Add(p.ToString() + " (Count: " + unknownPacketsCount[(byte)p] + ")");
            }
            toWrite.Add("Unknown extended packets list:" + Environment.NewLine);
            foreach (var p in unknownExtPackets)
            {
                toWrite.Add(p.ToString() + " (Count: " + unknownExtPacketsCount[(byte)p] + ")");
            }
            toWrite.Add("Unknown packets not in batch list:" + Environment.NewLine);
            foreach (var p in unknownPacketsNotInBatch)
            {
                toWrite.Add(p.ToString() + " (Count: " + unknownPacketsNotInBatchCount[(byte)p] + ")");
            }
            toWrite.Add("Unknown extended packets not in batch list:" + Environment.NewLine);
            foreach (var p in unknownExtPacketsNotInBatch)
            {
                toWrite.Add(p.ToString() + " (Count: " + unknownExtPacketsNotInBatchCount[(byte)p] + ")");
            }
            File.AppendAllLines("decrypted.txt", toWrite);
            Console.WriteLine("done");
            Console.ReadLine();
        }

        private static void parsePackets(List<Json.Packet> packets)
        {
            foreach (var packet in packets)
            {
                //File.AppendAllText("wireshark.txt", "000000 " + BitConverter.ToString(fakeUDPHeader(packet.Bytes, packet.Length).ToArray()).Replace('-', ' ') + Environment.NewLine);
                if (packet.Length < 45)
                    continue;
               // try
                {
                    var eventArgs = new CaptureEventArgs(new RawCapture(LinkLayers.Null, null, null), null);
                    eventArgs.Packet.Data = fakeUDPHeader(packet.Bytes, packet.Length).ToArray();
                    eventArgs.Packet.Timeval = new PosixTimeval(packet.Time);
                    F_OnPacketArrival(null, eventArgs);
                }
               // catch { }
            }
        }

        private static void initBlowfish(string key)
        {
            var decodeKey = Convert.FromBase64String(key);
            if (decodeKey.Length <= 0)
                return;

            fixed (byte* s = decodeKey)
                _blowfish = BlowFishCreate(s, new IntPtr(16));
        }

        private static List<byte> fakeUDPHeader(byte[] temp, int count)
        {
            var totalLen = BitConverter.GetBytes((ushort)(count + 28));
            var payloadLen = BitConverter.GetBytes((ushort)count + 8);
            var ret = new List<byte>()
            {
                0x45, 0x00, totalLen[1], totalLen[0], 0x03, 0x3f, 0x00, 0x00, 0x80, 0x11, 0x00, 0x00, 0x7f, 0x00, 0x00, 0x01, 0x7f, 0x00, 0x00, 0x01,
                0x13, 0xf7, 0xf6, 0x26, payloadLen[1], payloadLen[0], 0x31, 0xa7
            };
            ret.AddRange(temp);

            return ret;
        }

        private static void F_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var reader = new BinaryReader(new MemoryStream(e.Packet.Data));
            reader.ReadBytes(20); // IPv4 data

            var sourcePort = reader.ReadUInt16(true);
            var destPort = reader.ReadUInt16(true);
            var len = reader.ReadUInt16(true);

            if (len - 8 < 2)
                return;

            reader.ReadBytes(2); // Rest of UDP data
            reader.ReadBytes(8); // Enet protocol header

            var opCode = (EnetOpCodes)(reader.ReadByte() & 0x0F); // Enet opcode
            var channel = reader.ReadByte();
            reader.ReadBytes(2); //Rest of enet command header
            switch (opCode)
            {
                case EnetOpCodes.NONE:
                case EnetOpCodes.ACKNOWLEDGE:
                case EnetOpCodes.CONNECT:
                case EnetOpCodes.VERIFY_CONNECT:
                case EnetOpCodes.DISCONNECT:
                case EnetOpCodes.PING:
                case EnetOpCodes.THROTTLE_CONFIGURE:
                    return;
                case EnetOpCodes.SEND_RELIABLE:
                    handleReliable(reader, e.Packet.Timeval.Miliseconds, sourcePort > destPort, channel);
                    return;
                case EnetOpCodes.SEND_FRAGMENT:
                    handleFragment(reader, e.Packet.Timeval.Miliseconds, sourcePort > destPort, channel);
                    return;
            }
            logLine("Unknown enet command " + opCode + " " + BitConverter.ToString(new byte[] { (byte)opCode }));
            printPacket(e.Packet.Data, e.Packet.Timeval.Miliseconds, sourcePort > destPort, channel);
        }

        private static byte[] decrypt(byte[] packet)
        {
            var temp = packet.ToArray();
            if (temp.Length >= 8)
                fixed (byte* ptr = temp)
                    Decrypt1(_blowfish, ptr, new IntPtr(temp.Length - (temp.Length % 8)));

            return temp;
        }

        private static void printPacket(byte[] packet, float time, bool C2S, byte channel, bool addSeparator = true)
        {
            var tSent = TimeSpan.FromMilliseconds(time);
            var tt = tSent.ToString("mm\\:ss\\.ffff");
            if (Enum.IsDefined(typeof(PacketCmd), packet[0]))
            {
                if (!knownPackets.Contains((PacketCmd)packet[0]))
                {
                    knownPackets.Add((PacketCmd)packet[0]);
                }
            }
            else
            {
                if (!unknownPackets.Contains((PacketCmd)packet[0]))
                {
                    unknownPackets.Add((PacketCmd)packet[0]);
                }
                unknownPacketsCount[packet[0]]++;
            }

            if (packet[0] == 0xFE)
            {
                if (Enum.IsDefined(typeof(ExtendedPacketCmd), packet[5]))
                {
                    if (!knownExtPackets.Contains((ExtendedPacketCmd)packet[5]))
                    {
                        knownExtPackets.Add((ExtendedPacketCmd)packet[5]);
                    }
                }
                else
                {
                    if (!unknownExtPackets.Contains((ExtendedPacketCmd)packet[5]))
                    {
                        unknownExtPackets.Add((ExtendedPacketCmd)packet[5]);
                    }
                    unknownExtPacketsCount[packet[5]]++;
                }
                tt += C2S ? " C2S: " + (PacketCmd)(packet[0]) : " S2C: " + (PacketCmd)(packet[0]) + " : " + (ExtendedPacketCmd)(packet[5]);
            }
            else
            {
                tt += C2S ? " C2S: " + (PacketCmd)(packet[0]) : " S2C: " + (PacketCmd)(packet[0]);
            }
            tt += " Length:" + packet.Length + " Channel: " + (Channel)channel;
            if (!firstPacketReceived)
            {
                lastPacketTime = time;
                firstPacketReceived = true;
                tt += Environment.NewLine;
            }
            else
            {
                tt += " Time since last packet: " + (time - lastPacketTime) + "ms" + Environment.NewLine;
                lastPacketTime = time;
            }
            int i = 0;
            if (packet.Length > 15)
            {
                for (i = 16; i <= packet.Length; i += 16)
                {
                    for (var j = 16; j > 0; j--)
                        tt += packet[i - j].ToString("X2") + " ";
                    for (var j = 16; j > 0; j--)
                    {
                        if (packet[i - j] >= 32 && packet[i - j] <= 126)
                            tt += Encoding.Default.GetString(new byte[] { packet[i - j] });
                        else
                            tt += ".";
                    }
                    tt += Environment.NewLine;
                }
            }

            var temp = i;
            if (temp != packet.Length + 16)
            {
                if (temp > 15)
                    temp -= 16;
                var ssss = packet.Length - temp;
                while (temp < packet.Length)
                {
                    tt += packet[temp].ToString("X2") + " ";
                    temp++;
                }
                for (var j = temp % 16; j < 16; j++)
                    tt += "   ";

                temp = i > 15 ? i - 16 : i;
                for (var j = 0; j < ssss; j++)
                {
                    if (packet[temp + j] >= 32 && packet[temp + j] <= 126)
                        tt += Encoding.Default.GetString(new byte[] { packet[temp + j] });
                    else
                        tt += ".";
                }
            }
            logLine(tt);

            if (addSeparator)
                logLine("----------------------------------------------------------------------------");

        }

        private static void handleReliable(BinaryReader reader, float time, bool C2S, byte channel)
        {
            var len = reader.ReadUInt16(true);
            //if (reader.BaseStream.Length - reader.BaseStream.Position < len)
             //   return;

            var packet = reader.ReadBytes(len);
            if (packet.Length < 1)
                return;

            var decrypted = decrypt(packet);
            printPacket(decrypted, time, C2S, channel, false);

            if (decrypted[0] == 0xFF)
            {
                logLine(Environment.NewLine + "===Printing batch===");
                try
                {
                    decodeBatch(decrypted, time, C2S, channel);
                }
                catch
                {
                    logLine("Batch parsing threw an exception.");
                }
                logLine("======================end batch==========================" + Environment.NewLine);
            }
            else if (decrypted[0] == 0xFE)
            {
                if (!Enum.IsDefined(typeof(ExtendedPacketCmd), decrypted[5]))
                {
                    if (!unknownExtPacketsNotInBatch.Contains((ExtendedPacketCmd)decrypted[5]))
                    {
                        unknownExtPacketsNotInBatch.Add((ExtendedPacketCmd)decrypted[5]);
                    }
                    unknownExtPacketsNotInBatchCount[decrypted[5]]++;
                }
            }
            else
            {
                if (!Enum.IsDefined(typeof(PacketCmd), decrypted[0]))
                {
                    if (!unknownPacketsNotInBatch.Contains((PacketCmd)decrypted[0]))
                    {
                        unknownPacketsNotInBatch.Add((PacketCmd)decrypted[0]);
                    }
                    unknownPacketsNotInBatchCount[decrypted[0]]++;
                }
            }
        }

        private static void handleFragment(BinaryReader reader, float time, bool C2S, byte channel)
        {
            var fragmentGroup = reader.ReadUInt16(); // Fragment start number
            var len = reader.ReadUInt16(true);

            if (reader.BaseStream.Length - reader.BaseStream.Position < len + 16)
                return;

            var totalFragments = reader.ReadInt32(true);
            var currentFragment = reader.ReadInt32(true);
            var totalLen = reader.ReadInt32(true);
            reader.ReadInt32(); // Offset
            var payload = reader.ReadBytes(len);

            if (!fragmentBuffer.ContainsKey(fragmentGroup))
                fragmentBuffer.Add(fragmentGroup, new Dictionary<int, byte[]>());

            var buff = fragmentBuffer[fragmentGroup];
            if (buff.ContainsKey(currentFragment))
                buff[currentFragment] = payload;
            else
                buff.Add(currentFragment, payload);

            if (buff.Count == totalFragments)
            {
                var packet = new List<byte>();
                var temp = buff.OrderBy(x => x.Key);
                foreach (var t in temp)
                    packet.AddRange(t.Value);

                if (totalLen != packet.Count)
                    return;// logLine("Fragment's fishy. " + totalLen + "!=" + packet.Count);

                var decrypted = decrypt(packet.ToArray());
                printPacket(decrypted, time, C2S, channel);
                fragmentBuffer.Remove(fragmentGroup);
            }
        }

        //FE [ 00 00 00 00 00 ] 07 ...
        //0x107 [NET ID] ...
        private static void decodeBatch(byte[] decrypted, float time, bool C2S, byte channel)
        {
            var reader = new BinaryReader(new MemoryStream(decrypted));
            reader.ReadByte();

            var packetCount = reader.ReadByte();
            var size = reader.ReadByte();
            var opCode = reader.ReadByte();
            var firstNetId = reader.ReadUInt32(true);
            var firstPacket = new List<byte>();

            firstPacket.Add(opCode);
            firstPacket.AddRange(BitConverter.GetBytes(firstNetId).Reverse());

            if (size > 5)
                firstPacket.AddRange(reader.ReadBytes(size - 5));

            logLine("Packet 1, Length " + size);
            printPacket(firstPacket.ToArray(), time, C2S, channel, false);

            for (int i = 2; i < packetCount + 1; i++)
            {
                var buffer = new List<byte>();
                uint newId = 0;
                byte command;

                var flagsAndLength = reader.ReadByte(); // 6 first bits = size (if not 0xFC), 2 last bits = flags
                size = (byte)(flagsAndLength >> 2);

                if ((flagsAndLength & 0x01) > 0)
                { // additionnal byte, skip command
                    command = opCode;
                    if ((flagsAndLength & 0x02) > 0)
                    {
                        var netIdDifference = reader.ReadByte();
                        var netId = BitConverter.ToUInt32(BitConverter.GetBytes(firstNetId).Reverse().ToArray(), 0);
                        newId = netId + netIdDifference;
                    }
                    else
                    {
                        newId = reader.ReadUInt32(true);
                    }
                }
                else
                {
                    command = reader.ReadByte();
                    if ((flagsAndLength & 0x02) > 0)
                    {
                        var netIdDifference = reader.ReadByte();
                        var netId = BitConverter.ToUInt32(BitConverter.GetBytes(firstNetId).Reverse().ToArray(), 0);
                        newId = netId + netIdDifference;
                    }
                    else
                    {
                        newId = reader.ReadUInt32(true);
                    }
                }

                if (size == 0x3F)
                {
                    size = reader.ReadByte(); // size is too big to be on 6 bits, so instead it's stored later
                }

                logLine("Packet " + i + ", Length " + (size + 5));
                buffer.Add(command);
                buffer.AddRange(BitConverter.GetBytes(newId));
                buffer.AddRange(reader.ReadBytes(size));
                printPacket(buffer.ToArray(), time, C2S, channel, false);

                opCode = command;
            }
        }
        private void generateWiresharkImport()
        {
            var packets = new List<byte[]>();
            foreach (var packet in packets)
            {
                var tt = "";
                int i = 0;
                for (i = 8; i <= packet.Length; i += 8)
                {
                    tt += (i - 8).ToString("D6");
                    tt += " ";
                    tt += (packet[i - 8] + " ");
                    tt += (packet[i - 7] + " ");
                    tt += (packet[i - 6] + " ");
                    tt += (packet[i - 5] + " ");
                    tt += (packet[i - 4] + " ");
                    tt += (packet[i - 3] + " ");
                    tt += (packet[i - 2] + " ");
                    tt += (packet[i - 1] + " ........") + Environment.NewLine;
                }

                if (i != packet.Length + 8)
                {
                    i -= 8;
                    var ssss = packet.Length - i;
                    tt += (i).ToString("D6");
                    tt += " ";
                    while (i < packet.Length)
                    {
                        tt += packet[i] + " ";
                        i++;
                    }
                    for (int j = 0; j < ssss; j++)
                        tt += ".";
                }
                File.AppendAllText("out2.txt", tt + Environment.NewLine + Environment.NewLine);
            }
        }
        private static void logLine(string line)
        {
            //System.Diagnostics.Debug.WriteLine(line);
            //Console.WriteLine(line);
            toWrite.Add(line + Environment.NewLine);
        }
    }
}
