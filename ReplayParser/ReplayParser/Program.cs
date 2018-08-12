using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayParser
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
                return;
            if (!args[0].ToLower().EndsWith(".lrf"))
                return;

            // this is overkill lmao
            var file = ReplayFile.Open(args[0]);

            if (file == null)
                return;
            if (file.spectatorMode) 
                SpectatorResponse.initBlowfish(SpectatorResponse.getDecryptionKey(file.encryptionKey,file.matchID.ToString()));
                //SpectatorResponse.initBlowfish(file.encryptionKey);

            var reader = new PacketReader(file.GetReplayStream(),file.spectatorMode);
            if (!reader.loaded)
                return;
            if (file.spectatorMode)
                new SpectatorPacketWriter(reader.getSpectatorPackets()).write(args[0]);
            else
                new PacketWriter(reader.getPackets(), file).writeJson(args[0]);
        }
    }
}
