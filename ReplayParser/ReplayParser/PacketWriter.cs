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
            var players = new List<Dictionary<string, object>>();
            foreach (var player in Replay.players)
            {
                var temp = new Dictionary<string, object>();
                temp.Add("name", player.summoner);
                temp.Add("champion", player.champion);
                temp.Add("team", player.team);
                players.Add(temp);
            }
            buffer.Add("replayName", Replay.name);
            buffer.Add("accountId", Replay.accountID);
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
