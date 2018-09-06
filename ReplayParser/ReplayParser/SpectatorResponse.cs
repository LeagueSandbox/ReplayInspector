using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlowFishCS;
using System.IO.Compression;
using System.IO;

namespace ReplayParser
{
    class SpectatorResponse
    {
        private static BlowFish _blowfish;
        private List<SpectatorPacket> packetBuffer = new List<SpectatorPacket>();
        public SpectatorResponse(byte[] data)
        {
            string text = Encoding.UTF8.GetString(data);
            if (text.Contains("application/octet-stream"))
            {
                string endRequest = "Connection: close\r\n\r\n";
                int responseIndex = text.IndexOf(endRequest) + endRequest.Length;
                byte[] resp = new byte[data.Length - responseIndex];
                Buffer.BlockCopy(data, responseIndex, resp, 0, resp.Length);
                // blowfish decrypt
                resp = _blowfish.Decrypt_ECB(resp);
                // gzip decompress
                byte[] block = GzipDecompress(resp);
                using (BinaryReader reader = new BinaryReader(new MemoryStream(block)))
                {
                    Console.WriteLine("Parsing next response...");
                    byte lastPacketType = 0;
                    float lastTime = 0;
                    int lastParam = 0;
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    { 
                        SpectatorPacket packet = new SpectatorPacket(reader,lastPacketType,lastTime,lastParam);
                        lastPacketType = packet.packetType;
                        lastTime = packet.time;
                        lastParam = packet.blockparam;
                        lock(packetBuffer)
                            packetBuffer.Add(packet);    
                    }
                }
            }
        }
        public List<SpectatorPacket> GetPackets()
        {
            return packetBuffer;
        }
        public static byte[] getDecryptionKey(string encryptionKey, string gameId)
        {
            var decodeKey = Convert.FromBase64String(encryptionKey);
            var key = new byte[0x10];
            Buffer.BlockCopy(decodeKey, 0, key, 0, key.Length);
            BlowFish b = new BlowFish(Encoding.ASCII.GetBytes(gameId));
            Buffer.BlockCopy(b.Decrypt_ECB(decodeKey), 0, key, 0, key.Length);
            return key;

        }
        public static void initBlowfish(string key)
        {
            var decodeKey = Encoding.ASCII.GetBytes(key);
            //var decodeKey = Convert.FromBase64String(key);
            initBlowfish(decodeKey);
        }
        public static void initBlowfish(byte[] key)
        {
            _blowfish = new BlowFish(key);
        }
        static byte[] GzipDecompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip),
                CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
    }
}
