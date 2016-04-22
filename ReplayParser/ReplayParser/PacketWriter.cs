using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace ReplayParser
{
    public class PacketWriter
    {
        private List<Packet> Packets;
        private ReplayFile Replay;

        public PacketWriter(List<Packet> packets, ReplayFile replay)
        {
            Packets = packets;
            Replay = replay;
        }

        public void writeJson(string file)
        {
            file = Path.GetFileNameWithoutExtension(file);
            file += ".json";

            if (File.Exists(file))
                File.Delete(file);

            var buffer = new Dictionary<string, object>();
            var players = new Dictionary<string, object>();
            foreach (var player in Replay.players)
            {
                players.Add("name", player.summoner);
                players.Add("champion", player.champion);
                players.Add("team", player.team);
            }
            buffer.Add("replayName", Replay.name);
            buffer.Add("players", players);
            buffer.Add("serverAddress", Replay.serverAddress);
            buffer.Add("serverPort", Replay.serverPort);
            buffer.Add("encryptionKey", Replay.encryptionKey);
            buffer.Add("clientVersion", Replay.clientVersion);
            buffer.Add("packets", Packets);

            var json = JsonConvert.SerializeObject(buffer, Formatting.Indented);
            File.AppendAllText(file, json);
        }
    }
}
