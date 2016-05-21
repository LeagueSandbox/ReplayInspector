using System.Collections.Generic;

namespace PcapDecrypt.Json
{
    public class Replay
    {
        public string replayName { get; set; }
        public int accountId { get; set; }
        public List<Player> players { get; set; }
        public string serverAddress { get; set; }
        public int serverPort { get; set; }
        public string encryptionKey { get; set; }
        public string clientVersion { get; set; }
        public List<Packet> packets { get; set; }
    }
}
