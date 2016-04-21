using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Script.Serialization;

namespace ReplayParser
{
    [DataContract]
    public class ReplayFile
    {
        private long dataStart;

        public ReplayFile.FileVersion version { get; set; }

        [DataMember]
        public string clientVersion { get; set; }

        [DataMember]
        public string clientHash { get; set; }

        [DataMember]
        public string replayVersion { get; set; }

        [DataMember]
        public int statsVersion { get; set; }

        [DataMember]
        public long timestamp { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string encryptionKey { get; set; }

        [DataMember]
        public string region { get; set; }

        [DataMember]
        public string serverAddress { get; set; }

        [DataMember]
        public int serverPort { get; set; }

        [DataMember]
        public long accountID { get; set; }

        [DataMember]
        public string summonerName { get; set; }

        [DataMember]
        public long matchID { get; set; }

        [DataMember]
        public int matchLength { get; set; }

        [DataMember]
        public string matchType { get; set; }

        [DataMember]
        public string gameMode { get; set; }

        [DataMember]
        public int winningTeam { get; set; }

        [DataMember]
        public int map { get; set; }

        [DataMember]
        public int replayID { get; set; }

        [DataMember(IsRequired = false)]
        public bool spectatorMode { get; set; }

        [DataMember(IsRequired = false)]
        public bool observerStream { get; set; }

        [DataMember]
        public string queueType { get; set; }

        [DataMember]
        public bool ranked { get; set; }

        [DataMember]
        public int firstWinBonus { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool isStream { get; set; }

        [DataMember]
        public List<ReplayPlayer> players { get; set; }

        [DataMember]
        public List<ReplayFile.TeamInfo> teams { get; set; }

        [DataMember]
        public Dictionary<string, ReplayFile.DataIndex> dataIndex { get; set; }

        [DataMember]
        public List<ReplayFile.ReplayScreenshot> screenshots { get; set; }

        public string path { get; set; }

        private ReplayFile()
        {

        }

        public static ReplayFile Open(string path)
        {
            if (!File.Exists(path))
                return null;

            using (Stream input = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ReplayFile replayFile = null;

                var binaryReader = new BinaryReader(input);
                var fileVersion = new FileVersion();
                fileVersion.fileVersion = binaryReader.ReadUInt32();
                if (fileVersion.fileVersion == 2816)
                {
                    var contractJsonSerializer = new DataContractJsonSerializer(typeof(ReplayFile));
                    var count = binaryReader.ReadInt32();
                    var numArray = binaryReader.ReadBytes(count);

                    var json = Encoding.ASCII.GetString(numArray);
                    if (!json.EndsWith("}"))
                        System.Diagnostics.Debug.WriteLine(json);

                    MemoryStream memoryStream = new MemoryStream(numArray);
                    replayFile = contractJsonSerializer.ReadObject(memoryStream) as ReplayFile;
                    memoryStream.Close();

                    replayFile.version = fileVersion;
                    replayFile.path = path;
                    replayFile.dataStart = binaryReader.BaseStream.Position;
                }

                input.Close();
                return replayFile;
            }
        }

        public ReplayStream GetReplayStream()
        {
            if (!dataIndex.ContainsKey("stream"))
            {
                System.Diagnostics.Debug.WriteLine("Unable to find data stream in replay file: " + path);
                return null;
            }
            var replayStream = new ReplayStream();
            replayStream.version = replayVersion;

            var offset = dataStart + dataIndex["stream"].offset;
            var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            fileStream.Seek(offset, SeekOrigin.Begin);
            var binaryReader = new BinaryReader(fileStream);

            try
            {
                var buffer = Compression.Decompress(binaryReader.ReadBytes((int)fileStream.Length));
                if (buffer.Length == 0)
                    throw new Exception();
                var memoryStream = new MemoryStream(buffer);
                replayStream.reader = new BinaryReader(memoryStream);
                replayStream.length = memoryStream.Length;
                fileStream.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Decompression Error: " + ex.ToString());
                fileStream.Seek(offset, SeekOrigin.Begin);
                replayStream.reader = binaryReader;
                replayStream.length = dataIndex["stream"].size;
            }

            return replayStream;
        }

        [StructLayout(LayoutKind.Explicit, Size = 4)]
        public struct FileVersion
        {
            [FieldOffset(0)]
            public byte unused;
            [FieldOffset(1)]
            public byte version;
            [FieldOffset(2)]
            public byte compressionLevel;
            [FieldOffset(3)]
            public byte remaining;
            [FieldOffset(0)]
            public uint fileVersion;
        }

        [DataContract]
        public struct DataIndex
        {
            [DataMember]
            public int offset;
            [DataMember]
            public int size;
        }

        [DataContract]
        public class TeamInfo
        {
            [DataMember]
            public string name;
            [DataMember]
            public string tag;
            [DataMember]
            public string id;
            [DataMember]
            public int team;
        }

        [DataContract]
        public struct ReplayScreenshot
        {
            [DataMember]
            public int timestamp;
            [DataMember]
            public string name;
            [DataMember]
            public string type;

            public ReplayScreenshot(int timestamp, string name, string type)
            {
                this.timestamp = timestamp;
                this.name = name;
                this.type = type;
            }
        }

        [Serializable]
        private class TChunk
        {
            public int chunkID;
            public int duration;
            public int dataCount;
            public byte[] data;

            public TChunk(BinaryReader r)
            {
                this.chunkID = r.ReadInt32();
                this.duration = r.ReadInt32();
                this.dataCount = r.ReadInt32() / 2;
                this.data = r.ReadBytes(this.dataCount);
            }
        }

        [Serializable]
        private class TKeyFrame
        {
            public int keyFrameID;
            public int dataCount;
            public byte[] data;

            public TKeyFrame(BinaryReader r)
            {
                this.keyFrameID = r.ReadInt32();
                this.dataCount = r.ReadInt32() / 2;
                this.data = r.ReadBytes(this.dataCount);
            }
        }

        public class ReplayStream
        {
            public BinaryReader reader;
            public long length;
            public string version;
        }
    }
}
