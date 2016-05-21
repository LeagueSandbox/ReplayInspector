using BlowFishCS;
using Newtonsoft.Json;
using PacketDotNet;
using PcapDecrypt.Json;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static BlowFishCS.BlowFishCS;
using System.Windows.Forms;
using PcapDecrypt.Packets;

namespace PcapDecrypt
{
    unsafe class Program
    {
        private static BlowFish* _blowfish;
        private static Dictionary<int, Dictionary<int, byte[]>> fragmentBuffer = new Dictionary<int, Dictionary<int, byte[]>>();
        private static List<string> toWrite = new List<string>();

        public static List<Packets.Packet> PacketList = new List<Packets.Packet>();
        public static List<Packets.Packet> BatchPacketList = new List<Packets.Packet>();
        public static bool filtering = false;
        public static byte filter = (byte)PacketCmdS2C.PKT_S2C_ChatBoxMessage;
        public static bool filteringSearchInBatch = false;
        public static bool printToFile = false;
        public static bool toAdd;

        static void Main(string[] args)
        {
            if (args.Length < 1)
                return;

            Console.WriteLine("Running...");

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
                Console.WriteLine("Parsing packets, please wait...");
                var watch = System.Diagnostics.Stopwatch.StartNew();
                parsePackets(replay.packets);
                watch.Stop();
                var elapsed = watch.Elapsed;
                Console.WriteLine("Packets parsed. ("+elapsed+" ms)");
            }
            else
            {
                Console.WriteLine("Unknown input file.");
                Console.ReadLine();
                return;
            }
            if (args.Length > 1)
            {
                if (args[1].ToLower() == "-printtofile")
                {
                    printToFile = true;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());

            if (printToFile)
            {
                if (File.Exists("decrypted.txt"))
                    File.Delete("decrypted.txt");

                File.AppendAllLines("decrypted.txt", toWrite);
                Console.WriteLine("Packet dump saved to decrypted.txt");
            }
            Console.WriteLine("Done.");
            Console.ReadLine();
        }

        internal /*private*/ static void parsePackets(List<Json.Packet> packets)
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
            //Console.Clear();
            //Console.WriteLine("Packets parsed: " + PacketList.Count);
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
            reader.ReadBytes(3); //Rest of enet command header
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
                    handleReliable(reader, e.Packet.Timeval.Miliseconds, sourcePort > destPort);
                    return;
                case EnetOpCodes.SEND_FRAGMENT:
                    handleFragment(reader, e.Packet.Timeval.Miliseconds, sourcePort > destPort);
                    return;
            }
            logLine("Unknown enet command " + opCode + " " + BitConverter.ToString(new byte[] { (byte)opCode }));
            printPacket(e.Packet.Data, e.Packet.Timeval.Miliseconds, sourcePort > destPort);

            if (!filtering)
            {
                PacketList.Add(new Packets.Packet(e.Packet.Data));
            }
            if (filtering && e.Packet.Data[0] == filter)
            {
                PacketList.Add(new Packets.Packet(e.Packet.Data));
            }
        }

        private static byte[] decrypt(byte[] packet)
        {
            var temp = packet.ToArray();
            if (temp.Length >= 8)
                fixed (byte* ptr = temp)
                    Decrypt1(_blowfish, ptr, new IntPtr(temp.Length - (temp.Length % 8)));

            return temp;
        }

        private static void printPacket(byte[] packet, float time, bool C2S, bool addSeparator = true)
        {
            if (printToFile)
            {
                var tSent = TimeSpan.FromMilliseconds(time);
                var tt = tSent.ToString("mm\\:ss\\.ffff");
                tt += C2S ? " C2S: " + (PacketCmdS2C)(packet[0]) : " S2C: " + (PacketCmdS2C)(packet[0]);
                tt += " Length:" + packet.Length + Environment.NewLine;
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
                logLine(tt + Environment.NewLine);

                if (addSeparator)
                    logLine("----------------------------------------------------------------------------");
            }
        }

        private static void handleReliable(BinaryReader reader, float time, bool C2S)
        {
            var len = reader.ReadUInt16(true);
            //if (reader.BaseStream.Length - reader.BaseStream.Position < len)
            //   return;

            var packet = reader.ReadBytes(len);
            if (packet.Length < 1)
                return;

            var decrypted = decrypt(packet);
            printPacket(decrypted, time, C2S, false);

            /*PacketCmdS2C cmd = (PacketCmdS2C)decrypted[0];
            string cmdString = "Packets." + cmd.ToString();
            //Console.Write(cmdString);
            Packets.Packets caca = GetInstance(cmdString);
            */
            if (!filtering)
            {
                PacketList.Add(new Packets.Packet(decrypted));
            }
            if (filtering && (decrypted[0] == filter))
            {
                PacketList.Add(new Packets.Packet(decrypted));
            }

            if (decrypted[0] == 0xFF)
            {
                logLine(Environment.NewLine + "===Printing batch===");
                try
                {
                    decodeBatch(decrypted, time, C2S);
                    if (toAdd)
                    {
                        PacketList.Add(new Packets.Packet(decrypted));
                    }
                }
                catch
                {
                    logLine("Batch parsing threw an exception.");
                }
                logLine("======================end batch==========================" + Environment.NewLine);
            }
        }

        public static Packets.Packet CreatePacket(byte[] bytes)
        {
            switch ((PacketCmdS2C)bytes[0]) // This looks horrible, don't hate me please :3
            {
                case PacketCmdS2C.PKT_S2C_AddBuff:
                    return new PKT_S2C_AddBuff(bytes);
                case PacketCmdS2C.PKT_S2C_Announce:
                    return new PKT_S2C_Announce(bytes);
                case PacketCmdS2C.PKT_S2C_AttentionPing:
                    return new PKT_S2C_AttentionPing(bytes);
                case PacketCmdS2C.PKT_S2C_Batch:
                    return new PKT_S2C_Batch(bytes);
                case PacketCmdS2C.PKT_S2C_BeginAutoAttack:
                    return new PKT_S2C_BeginAutoAttack(bytes);
                case PacketCmdS2C.PKT_S2C_BuyItemAns:
                    return new PKT_S2C_BuyItemAns(bytes);
                case PacketCmdS2C.PKT_S2C_CastSpellAns:
                    return new PKT_S2C_CastSpellAns(bytes);
                case PacketCmdS2C.PKT_S2C_ChampionDie:
                    return new PKT_S2C_ChampionDie(bytes);
                case PacketCmdS2C.PKT_S2C_ChampionRespawn:
                    return new PKT_S2C_ChampionRespawn(bytes);
                case PacketCmdS2C.PKT_S2C_CharStats:
                    //return new PKT_S2C_CharStats(bytes);
                case PacketCmdS2C.PKT_S2C_ChatBoxMessage:
                    return new PKT_C2S_ChatBoxMessage(bytes);
                case PacketCmdS2C.PKT_S2C_DamageDone:
                    return new PKT_S2C_DamageDone(bytes);
                case PacketCmdS2C.PKT_S2C_Dash:
                    return new PKT_S2C_Dash(bytes);
                case PacketCmdS2C.PKT_S2C_DebugMessage:
                    return new PKT_S2C_DebugMessage(bytes);
                case PacketCmdS2C.PKT_S2C_DeleteObject:
                    return new PKT_S2C_DeleteObject(bytes);
                case PacketCmdS2C.PKT_S2C_DestroyProjectile:
                    return new PKT_S2C_DestroyProjectile(bytes);
                case PacketCmdS2C.PKT_S2C_EditBuff:
                    return new PKT_S2C_EditBuff(bytes);
                case PacketCmdS2C.PKT_S2C_Emotion:
                    return new PKT_S2C_Emotion(bytes);
                case PacketCmdS2C.PKT_S2C_EndSpawn:
                    return new PKT_S2C_EndSpawn(bytes);
                case PacketCmdS2C.PKT_S2C_Extended:
                    return new PKT_S2C_Extended(bytes);
                case PacketCmdS2C.PKT_S2C_FaceDirection:
                    return new PKT_S2C_FaceDirection(bytes);
                case PacketCmdS2C.PKT_S2C_FloatingText:
                    return new PKT_S2C_FloatingText(bytes);
                case PacketCmdS2C.PKT_S2C_FogUpdate2:
                    //return new PKT_S2C_FogUpdate2(bytes);
                case PacketCmdS2C.PKT_S2C_GameEnd:
                    return new PKT_S2C_GameEnd(bytes);
                case PacketCmdS2C.PKT_S2C_GameTimer:
                    return new PKT_S2C_GameTimer(bytes);
                case PacketCmdS2C.PKT_S2C_GameTimerUpdate:
                    return new PKT_S2C_GameTimerUpdate(bytes);
                case PacketCmdS2C.PKT_S2C_HeroSpawn:
                    return new PKT_S2C_HeroSpawn(bytes);
                case PacketCmdS2C.PKT_S2C_KeyCheck:
                    return new PKT_S2C_KeyCheck(bytes);
                case PacketCmdS2C.PKT_S2C_LeaveVision:
                    return new PKT_S2C_LeaveVision(bytes);
                case PacketCmdS2C.PKT_S2C_LevelPropSpawn:
                    return new PKT_S2C_LevelPropSpawn(bytes);
                case PacketCmdS2C.PKT_S2C_LevelUp:
                    return new PKT_S2C_LevelUp(bytes);
                /*case PacketCmdS2C.PKT_S2C_LoadHero:
                    return new PKT_S2C_LoadHero(bytes);*/
                case PacketCmdS2C.PKT_S2C_LoadName:
                    return new PKT_S2C_LoadName(bytes);
                case PacketCmdS2C.PKT_S2C_LoadScreenInfo:
                    return new PKT_S2C_LoadScreenInfo(bytes);
                case PacketCmdS2C.PKT_S2C_MoveAns:
                    return new PKT_S2C_MoveAns(bytes);
                case PacketCmdS2C.PKT_S2C_NextAutoAttack:
                    return new PKT_S2C_NextAutoAttack(bytes);
                case PacketCmdS2C.PKT_S2C_NPC_Hide:
                    //return new PKT_S2C_NPC_Hide(bytes);
                case PacketCmdS2C.PKT_S2C_ObjectSpawn:
                    return new PKT_S2C_ObjectSpawn(bytes);
                case PacketCmdS2C.PKT_S2C_Ping_Load_Info:
                    return new PKT_S2C_Ping_Load_Info(bytes);
                case PacketCmdS2C.PKT_S2C_PlayerInfo:
                    return new PKT_S2C_PlayerInfo(bytes);
                case PacketCmdS2C.PKT_S2C_QueryStatusAns:
                    return new PKT_S2C_QueryStatusAns(bytes);
                case PacketCmdS2C.PKT_S2C_RemoveBuff:
                    return new PKT_S2C_RemoveBuff(bytes);
                case PacketCmdS2C.PKT_S2C_RemoveItem:
                    return new PKT_S2C_RemoveItem(bytes);
                case PacketCmdS2C.PKT_S2C_SetAnimation:
                    return new PKT_S2C_SetAnimation(bytes);
                case PacketCmdS2C.PKT_S2C_SetCooldown:
                    return new PKT_S2C_SetCooldown(bytes);
                case PacketCmdS2C.PKT_S2C_SetHealth:
                    return new PKT_S2C_SetHealth(bytes);
                case PacketCmdS2C.PKT_S2C_SetTarget:
                    return new PKT_S2C_SetTarget(bytes);
                case PacketCmdS2C.PKT_S2C_SetTarget2:
                    return new PKT_S2C_SetTarget2(bytes);
                case PacketCmdS2C.PKT_S2C_ShowProjectile:
                    return new PKT_S2C_ShowProjectile(bytes);
                case PacketCmdS2C.PKT_S2C_SkillUp:
                    return new PKT_S2C_SkillUp(bytes);
                case PacketCmdS2C.PKT_S2C_SpawnParticle:
                    return new PKT_S2C_SpawnParticle(bytes);
                case PacketCmdS2C.PKT_S2C_SpawnProjectile:
                    return new PKT_S2C_SpawnProjectile(bytes);
                case PacketCmdS2C.PKT_S2C_SpellAnimation:
                    return new PKT_S2C_SpellAnimation(bytes);
                case PacketCmdS2C.PKT_S2C_StartGame:
                    return new PKT_S2C_StartGame(bytes);
                case PacketCmdS2C.PKT_S2C_StartSpawn:
                    return new PKT_S2C_StartSpawn(bytes);
                case PacketCmdS2C.PKT_S2C_StopAutoAttack:
                    return new PKT_S2C_StopAutoAttack(bytes);
                case PacketCmdS2C.PKT_S2C_Surrender:
                    return new PKT_S2C_Surrender(bytes);
                case PacketCmdS2C.PKT_S2C_SurrenderResult:
                    return new PKT_S2C_SurrenderResult(bytes);
                case PacketCmdS2C.PKT_S2C_SwapItems:
                    return new PKT_S2C_SwapItems(bytes);
                case PacketCmdS2C.PKT_S2C_SynchVersion:
                    return new PKT_S2C_SynchVersion(bytes);
                case PacketCmdS2C.PKT_S2C_TurretSpawn:
                    return new PKT_S2C_TurretSpawn(bytes);
                case PacketCmdS2C.PKT_S2C_UpdateModel:
                    return new PKT_S2C_UpdateModel(bytes);
                case PacketCmdS2C.PKT_S2C_ViewAns:
                    return new PKT_S2C_ViewAns(bytes);
                case PacketCmdS2C.PKT_S2C_World_SendGameNumber:
                    return new PKT_S2C_World_SendGameNumber(bytes);
                default:
                    return new PKT_S2C_Unknown(bytes);
            }
        }

        public static Packets.Packet GetInstance(string strFullyQualifiedName)
        {
            Type type = Type.GetType(strFullyQualifiedName);
            if (type != null)
                return (Packets.Packet)Activator.CreateInstance(type);
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(strFullyQualifiedName);
                if (type != null)
                    return (Packets.Packet)Activator.CreateInstance(type);
            }
            return null;
        }

        private static void handleFragment(BinaryReader reader, float time, bool C2S)
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
                if (!filtering)
                {
                    PacketList.Add(new Packets.Packet(decrypted));
                }
                if (filtering && decrypted[0] == filter)
                {
                    PacketList.Add(new Packets.Packet(decrypted));
                }
                printPacket(decrypted, time, C2S);
                fragmentBuffer.Remove(fragmentGroup);
            }
        }

        //FE [ 00 00 00 00 00 ] 07 ...
        //0x107 [NET ID] ...
        public static void decodeBatch(byte[] decrypted, float time, bool C2S)
        {
            toAdd = false;

            var reader = new BinaryReader(new MemoryStream(decrypted));
            reader.ReadByte();

            var packetCount = reader.ReadByte();
            var size = reader.ReadByte();
            var opCode = reader.ReadByte();
            var netId = reader.ReadUInt32(true);
            var firstPacket = new List<byte>();

            firstPacket.Add(opCode);
            firstPacket.AddRange(BitConverter.GetBytes(netId).Reverse());

            if (size > 5)
                firstPacket.AddRange(reader.ReadBytes(size - 5));

            logLine("Packet 1, Length " + size);
            printPacket(firstPacket.ToArray(), time, C2S, false);
            if (!filtering)
            {
                toAdd = true;
                BatchPacketList.Clear();
                BatchPacketList.Add(new Packets.Packet(firstPacket.ToArray()));
            }
            if (filtering && firstPacket.ToArray()[0] == filter)
            {
                toAdd = true;
                BatchPacketList.Clear();
                BatchPacketList.Add(new Packets.Packet(firstPacket.ToArray()));
            }

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
                    reader.ReadByte();
                }
                else
                {
                    command = reader.ReadByte();
                    if ((flagsAndLength & 0x02) > 0)
                        reader.ReadByte();
                    else
                        newId = reader.ReadUInt32(true);
                }

                if (size == 0x3F)
                    size = reader.ReadByte(); // size is too big to be on 6 bits, so instead it's stored later

                logLine("Packet " + i + ", Length " + (size + 5));
                buffer.Add(command);
                if (newId > 0)
                    buffer.AddRange(BitConverter.GetBytes(newId).Reverse());
                else
                    buffer.AddRange(BitConverter.GetBytes(netId).Reverse());
                buffer.AddRange(reader.ReadBytes(size));
                if (!filtering)
                {
                    toAdd = true;
                    BatchPacketList.Add(new Packets.Packet(buffer.ToArray()));
                }
                if (filtering && buffer.ToArray()[0] == filter)
                {
                    toAdd = true;
                    BatchPacketList.Add(new Packets.Packet(buffer.ToArray()));
                }
                printPacket(buffer.ToArray(), time, C2S, false);

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
