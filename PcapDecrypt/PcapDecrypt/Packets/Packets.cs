using System.IO;

namespace PcapDecrypt.Packets
{
    public class PKT_S2C_Unknown : Packet
    {
        public PKT_S2C_Unknown(byte[] data, float time) : base(data, time)
        {
            readByte("cmd");
            close();
        }
    }

    public class PKT_C2S_ClientReady : Packet
    {
        public PKT_C2S_ClientReady(byte[] data, float time) : base(data, time)
        {
            readByte("cmd");
            readInt("playerId");
            readInt("teamId");
            close();
        }
    }

    public class PKT_S2C_SynchVersion : Packet
    {

        public PKT_S2C_SynchVersion(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("unk(9)");
            readInt("mapId");
            for (var i = 0; i < 12; i++)
            {
                readLong("userId");
                readShort("unk(0x1E)");
                readInt("summonerSpell1");
                readInt("summonerSpell2");
                readByte("isBot");
                readInt("teamId");
                readFill(64, "formerName");
                readFill(64, "none");
                readString(24, "rank");
                readInt("icon");
                readShort("ribbon");
            }

            readString(256, "version");
            readString(128, "gameMode");
            readString(3, "serverLocale");
            readFill(2333, "unk");
            readInt("gameFeatures"); // gameFeatures (turret range indicators, etc.)
            readFill(256, "unk");
            readInt("unk");
            readFill(19, "unk(1)");
            close();
        }
    }

    public class PKT_S2C_Ping_Load_Info : Packet
    {
        public PacketCmdS2C cmd;
        public int netId;
        public int unk1;
        public long userId;
        public float loaded;
        public float ping;
        public short unk2;
        public short unk3;
        public byte unk4;

        public PKT_S2C_Ping_Load_Info(byte[] data, float time) : base(data, time)
        {
            readByte("cmd");
            readInt("netId");
            readUInt("unk");
            readULong("userId");
            readFloat("loaded");
            readFloat("ping");
            readShort("unk");
            readShort("unk");
            readByte("unk");
            close();
        }
    }
    public class PKT_C2S_Ping_Load_Info : PKT_S2C_Ping_Load_Info
    {
        public PKT_C2S_Ping_Load_Info(byte[] data, float time) : base(data, time)
        {

        }
    }

    public class PKT_S2C_LoadScreenInfo : Packet
    {
        public PKT_S2C_LoadScreenInfo(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            //Zero this complete buffer
            readUInt("blueMax");
            readUInt("blueMax");

            for (var i = 0; i < 6; i++)
                readULong("userId");

            readFill(144, "unk");

            for (int i = 0; i < 6; i++)
                readULong("userId");

            readFill(144, "unk");
            readInt("currentBlue");
            readInt("currentPurple");
            close();
        }
    }

    public class PKT_S2C_KeyCheck : Packet
    {
        public PKT_S2C_KeyCheck(byte[] data, float time) : base(data, time)
        {
            readByte("cmd");
            readByte("unk(0x2A?)");
            readByte("unk");
            readByte("unk(0xFF?)");
            readInt("playerNo");
            readLong("userId");
            readInt("unk(0)");
            readLong("unk(0)");
            readInt("unk(0)");
            close();
        }
    }

    public class PKT_C2S_KeyCheck : PKT_S2C_KeyCheck
    {
        public PKT_C2S_KeyCheck(byte[] data, float time) : base(data, time)
        {

        }
    }

    public class PKT_C2S_LockCamera : Packet
    {
        public PKT_C2S_LockCamera(byte[] data, float time) : base(data, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("isLock");
            readInt("padding");
            close();
        }
    }

    public class PKT_S2C_ObjectSpawn : Packet //Minion Spawn
    {
        public PKT_S2C_ObjectSpawn(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            if (getBufferLength() == 8) //MinionSpawn2
            {
                readUInt("netId");
                readFill(3, "unk");
                close();
                return;
            }
            if (isHeroSpawn()) //HeroSpawn2
            {
                readFill(15, "unk");
                readByte("unk(0x80)");
                readByte("unk(0x3F)");
                readFill(13, "unk");
                readByte("unk(3)");
                readInt("unk(1)");
                readFloat("x");
                readFloat("y");
                readFloat("z?");
                readFloat("rotation?");
                close();
                return;
            }
            if (isEnterVisionPacket())
            {
                readFill(13, "unk");
                readFloat("unk(1)");
                readFill(13, "unk");
                readByte("unk(0x02)");
                readInt("tickCount");

                var coordsCount = readByte("coordCount");
                readInt("netId");
                readByte("movementMask(0)");
                for (int i = 0; i < coordsCount / 2; i++)
                {
                    readShort("x");
                    readShort("y");
                }
                close();
                return;
            }
            readInt("unk"); // unk
            readByte("spawnType(3)"); // SpawnType - 3 = minion
            readInt("netId");
            readInt("netId");
            readInt("spawnPos");
            readByte("unk(255)");
            readByte("unk(1)");
            readByte("type");
            readByte("minionType(0=melee)");
            readByte("unk");
            readInt("minionSpawnType");
            readInt("unk");
            readInt("unk");
            readShort("unk");
            readFloat("unk(1)");
            readInt("unk");
            readInt("unk");
            readInt("unk");
            readShort("unk(512)");
            readInt("tickCount");

            var count = readByte("coordCount");
            readInt("netId");
            readByte("movementMask(0)");
            for (int i = 0; i < count / 2; i++)
            {
                readShort("x");
                readShort("y");
            }
            close();
        }
    }

    class PKT_S2C_SpellAnimation : Packet
    {
        public PKT_S2C_SpellAnimation(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("unk(5)");
            readInt("unk");
            readInt("unk");
            readFloat("unk(1)");
            readZeroTerminatedString("animationName");
            close();
        }
    }

    class PKT_S2C_SetAnimation : Packet
    {
        public PKT_S2C_SetAnimation(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            var count = readByte("animationPairsCount");

            for (int i = 0; i < count; i++)
            {
                var strLen = readInt("animationPair1Length");
                readString(strLen, "animationPair1");
                strLen = readInt("animationPair2Length");
                readString(strLen, "animationPair2");
            }
            close();
        }
    }

    public class PKT_S2C_FaceDirection : Packet
    {
        public PKT_S2C_FaceDirection(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readFloat("relativeX");
            readFloat("relativeZ");
            readFloat("relativeY");
            readByte("unk");
            readFloat("unk(0.0833)");
            close();
        }
    };

    public class PKT_S2C_FloatingText : Packet
    {
        public PKT_S2C_FloatingText(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            close();
        }
    };

    public class PKT_S2C_Dash : Packet
    {
        public PKT_S2C_Dash(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("tickCount");
            readShort("numUpdates?(1)");
            readByte("unk(5)");
            readInt("netId");
            readByte("unk(0)");
            readFloat("dashSpeed");
            readInt("unk");
            readFloat("x");
            readFloat("y");
            readInt("unk(0)");
            readByte("unk(0)");
            readInt("unk");
            readUInt("unk");
            readInt("unk");
            readByte("vectorBitmask"); // Vector bitmask on whether they're int16 or byte
            readShort("fromX");
            readShort("fromY");
            readShort("toX");
            readShort("toY");
            close();
        }
    }

    public class PKT_S2C_LeaveVision : Packet
    {
        public PKT_S2C_LeaveVision(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            close();
        }
    }

    public class PKT_S2C_DeleteObject : Packet
    {
        public PKT_S2C_DeleteObject(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            close();
        }
    }

    public class AddGold : Packet
    {
        public AddGold(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("killerNetId");
            readInt("killedNetId");
            readFloat("gold");
            close();
        }
    }

    public class PKT_C2S_MoveReq : Packet
    {
        public PKT_C2S_MoveReq(byte[] data, float time) : base(data, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("moveType");
            readFloat("x");
            readFloat("y");
            readInt("targetNetId");
            readByte("coordsCount");
            readInt("netId");
            readFill((int)(Reader.BaseStream.Length - Reader.BaseStream.Position), "moveData");
            close();
        }
    }

    public class PKT_S2C_MoveAns : Packet
    {
        public PKT_S2C_MoveAns(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("tickCount");
            readShort("updatesCount");          //count
            int coordsCount = readByte("coordsCount");
            if (isTeleport())
            {
                readByte("teleportId?");
                readInt("netId");
                if ((Reader.BaseStream.Length - Reader.BaseStream.Position) > 4)
                {
                    readByte("unk(1)");
                }
                readShort("x");
                readShort("y");
                close();
                return;
            }
            readInt("actorNetId");
            for (var i = 0; i < (coordsCount + 5) / 8; i++)
                readByte("coordsMask");

            for (int i = 0; i < coordsCount / 2; i++)
            {
                readShort("coord" + i + "x");
                readShort("coord" + i + "y");
            }
            close();
        }
    }

    public class PKT_S2C_QueryStatusAns : Packet
    {
        public PKT_S2C_QueryStatusAns(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("status(1=ok)");
            close();
        }
    }

    public class PKT_C2S_QueryStatusReq : Packet
    {
        public PKT_C2S_QueryStatusReq(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            close();
        }
    }

    public class PKT_C2S_SynchVersion : Packet
    {
        public PacketCmdS2C cmd;
        public int netId;
        public int unk1;
        private byte[] _version = new byte[256]; // version string might be shorter?

        public PKT_C2S_SynchVersion(byte[] data, float time) : base(data, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("unk");
            readString(256, "versionString");
            close();
        }
    }

    public class PKT_S2C_World_SendGameNumber : Packet
    {
        public PKT_S2C_World_SendGameNumber(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readLong("gameId");
            readString(128, "clientName");
            close();
        }
    }

    //app crash inc
    public class PKT_C2S_StatsConfirm : Packet
    {
        public PKT_C2S_StatsConfirm(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("tickCount");
            readByte("updateNo");
            readByte("masterMask");
            readInt("netId");
            var mask = readInt("mask");
            var size = readByte("size");
            for (int i = 0; i < size; i++)
            {
                if (mask == 0)
                    readShort("value");
                else
                    readByte("value");
            }
            close();
        }
    }

    public class PKT_C2S_ChatBoxMessage : Packet
    {
        public PKT_C2S_ChatBoxMessage(byte[] data, float time) : base(data, time)
        {
            var Reader = new BinaryReader(new MemoryStream(data));
            readByte("cmd");
            readInt("playerNetId");
            readInt("botNetId");
            readByte("idBotMsg");
            readInt("msgType");
            readInt("unk");
            var len = readInt("msgLen");
            readFill(32, "unk");
            readString(len, "msg");
            close();
        }
    }

    public class PKT_S2C_UpdateModel : Packet
    {
        public PKT_S2C_UpdateModel(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("netId");
            readByte("bOk");
            readInt("unk(-1)");
            readString(32, "modelName");
            close();
        }
    }

    public class PKT_S2C_EditBuff : Packet
    {
        public PKT_S2C_EditBuff(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            close();
        }
    }

    public class PKT_S2C_EndSpawn : Packet
    {
        public PKT_S2C_EndSpawn(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            close();
        }
    }
    public class PKT_S2C_StartGame : PKT_S2C_EndSpawn
    {
        public PKT_S2C_StartGame(byte[] bytes, float time) : base(bytes, time)
        {
        }
    }

    public class PKT_S2C_StartSpawn : Packet
    {
        public PKT_S2C_StartSpawn(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readShort("unk");
            close();
        }
    }
    /*
    public class FogUpdate2
    {
        PacketHeader header;
        float x;
        float y;
        int radius;
        short unk1;
        public FogUpdate2()
        {
            header = new PacketHeader();
            header.cmd = PacketCmdS2C.PKT_S2C_FogUpdate2;
            header.netId = 0x40000019;
        }
    }*/

    public class Click : Packet
    {
        public Click(byte[] data, float time) : base(data, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("zero");
            readInt("targetNetId");
            close();
        }
    }

    public class PKT_S2C_HeroSpawn : Packet
    {
        public PKT_S2C_HeroSpawn(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("playerNetId");
            readInt("playerId");
            readByte("netNodeID?(40)");
            readByte("botSkillLevel");
            readByte("teamId");
            readByte("isBot");
            readByte("spawnPosIndex");
            readInt("skinNo");
            readString(128, "playerName");
            readString(40, "championType");
            readFloat("deathDurationRemaining");
            readFloat("timeSinceDeath");
            readInt("unk");
            readByte("bitField");
            close();
        }
    }

    public class PKT_S2C_TurretSpawn : Packet
    {
        public PKT_S2C_TurretSpawn(byte[] b, float time) : base(b, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("netId");
            readString(64, "name");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            close();
        }
    }

    public class PKT_S2C_GameEnd : Packet
    {
        public PKT_S2C_GameEnd(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("0:lose, 1:win");
            close();
        }
    }

    public class PKT_S2C_GameTimer : Packet
    {
        public PKT_S2C_GameTimer(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readFloat("time");
            close();
        }
    }

    public class PKT_S2C_GameTimerUpdate : Packet
    {
        public PKT_S2C_GameTimerUpdate(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readFloat("time");
        }
    }

    public class PKT_C2S_HeartBeat : Packet
    {
        public PKT_C2S_HeartBeat(byte[] data, float time) : base(data, time)
        {
            readByte("cmd");
            readInt("netId");
            readFloat("receiveTime");
            readFloat("ackTime");
            close();
        }
    }

    /* public class SpellSet
     {
         public PacketHeader header;
         public int spellID;
         public int level;
         public SpellSet(int netID, int _spellID, int _level)
         {
             header = new PacketHeader();
             header.cmd = (PacketCmdS2C)0x5A;
             header.netId = netID;
             spellID = _spellID;
             level = _level;
         }
     }*/

    public class PKT_C2S_SkillUp : Packet
    {
        public PacketCmdC2S cmd;
        public int netId;
        public byte skill;
        public PKT_C2S_SkillUp(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("skillId");
            close();
        }
    }
    public class PKT_S2C_SkillUp : Packet
    {
        public PKT_S2C_SkillUp(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("skill");
            readByte("level");
            readByte("pointsLeft");
            close();
        }
    }

    public class PKT_S2C_Batch : Packet
    {
        public PKT_S2C_Batch(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readByte("packetCount");
            readByte("size");
            readByte("opCode");
            readUInt("netId");
            readFill((int)(Reader.BaseStream.Length - Reader.BaseStream.Position), "Packet");
            close();
        }
    }

    public class PKT_C2S_BuyItemReq : Packet
    {
        public PKT_C2S_BuyItemReq(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("id");
            close();
        }
    }

    public class PKT_S2C_BuyItemAns : Packet
    {
        public PKT_S2C_BuyItemAns(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("itemId");
            readByte("itemSlot");
            readByte("itemStack");
            readByte("unk");
            readByte("unk");
            close();
        }
    }

    public class PKT_C2S_SellItem : Packet
    {
        public PKT_C2S_SellItem(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("slot");
            close();
        }
    }


    public class PKT_S2C_RemoveItem : Packet
    {
        public PKT_S2C_RemoveItem(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("slot");
            readByte("remaining");
        }
    }

    public class PKT_C2S_Emotion : Packet
    {
        public PKT_C2S_Emotion(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("id");
            close();
        }
    }
    public class PKT_S2C_Emotion : PKT_C2S_Emotion
    {
        public PKT_S2C_Emotion(byte[] bytes, float time) : base(bytes, time)
        {

        }
    }

    public class PKT_S2C_Extended : Packet
    {
        public PKT_S2C_Extended(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("eCmd");
            readInt("netId");
            readFill((int)(Reader.BaseStream.Length - Reader.BaseStream.Position), "eCmdInfo"); // TODO: Analyze extended Packet
        }
    }

    public class PKT_C2S_SwapItems : Packet
    {
        public PKT_C2S_SwapItems(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("slotFrom");
            readByte("slotTo");
            close();
        }
    }

    public class PKT_S2C_SwapItems : PKT_C2S_SwapItems
    {
        public PKT_S2C_SwapItems(byte[] bytes, float time) : base(bytes, time)
        {
        }
    }

    class PKT_S2C_Announce : Packet
    {
        public PKT_S2C_Announce(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("msgId");
            readLong("unk");
            if (Reader.BaseStream.Position < Reader.BaseStream.Length)
                readInt("mapId");
            close();
        }
    }

    public class PKT_S2C_AddBuff : Packet
    {
        public PKT_S2C_AddBuff(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("targetNetId");
            readByte("slot");
            readByte("type");
            readByte("stacks");
            readByte("visible/not visible");
            readInt("buffHash");
            readFill(8, "unk");
            readFloat("duration");
            readFill(4, "unk");
            readInt("sourceNetId");
            close();
        }
    }

    public class PKT_S2C_RemoveBuff : Packet
    {
        public PKT_S2C_RemoveBuff(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readShort("unk");
            readInt("buffHash");
            readInt("unk");
            close();
        }
    }

    public class PKT_S2C_DamageDone : Packet
    {
        public PKT_S2C_DamageDone(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readShort("type");
            readShort("unk");
            readFloat("dmgDone");
            readInt("targetNetId");
            readInt("sourceNetId");
            close();
        }
    }

    public class EPKT_S2C_NPC_Die : Packet
    {
        public EPKT_S2C_NPC_Die(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("killedNetId");
            readByte("ecmd");
            readByte("unk");
            readInt("unk");
            readShort("unk");
            readInt("killerNetId");
            readShort("unk");
            readShort("unk");
            readShort("unk");
            close();
        }
    }

    public class PKT_S2C_LoadName : Packet
    {
        public PKT_S2C_LoadName(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readLong("userId");
            readInt("unk(0)");
            readInt("strLen");
            readZeroTerminatedString("name");
            close();
        }
    }

    public class PKT_S2C_LoadHero : Packet
    {
        public PKT_S2C_LoadHero(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readLong("userId");
            readInt("skinNo");
            readInt("strLen");
            readZeroTerminatedString("name");
            close();
        }
    }

    public class PKT_C2S_AttentionPing : Packet
    {
        public PKT_C2S_AttentionPing(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("unk");
            readFloat("x");
            readFloat("y");
            readInt("targetNetId");
            readByte("pingType");
            close();
        }
    }

    public class PKT_S2C_AttentionPing : Packet
    {
        public PKT_S2C_AttentionPing(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readFloat("x");
            readFloat("y");
            readInt("targetNetId");
            readInt("championNetId");
            readByte("pingType");
            readByte("unk");
            close();
        }
    }

    public class PKT_S2C_BeginAutoAttack : Packet
    {
        public PKT_S2C_BeginAutoAttack(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("attackerNetId");
            readInt("attackedNetId");
            readByte("unk");
            readInt("futureProjNetId");
            readByte("isCritical(0x49=true");
            readByte("unk");
            readByte("unk");
            readShort("attackedX");
            readByte("unk");
            readByte("unk");
            readShort("attackedY");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readFloat("attackerX");
            readFloat("attackerY");
            close();
        }
    }

    public class PKT_S2C_NextAutoAttack : Packet
    {
        public PKT_S2C_NextAutoAttack(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("attackedNetID");
            readByte("isInitial(0x80=1)");
            readInt("futureProjNetId");
            readByte("isCritical(0x49=1)");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            close();
        }
    }

    public class PKT_S2C_StopAutoAttack : Packet
    {
        public PKT_S2C_StopAutoAttack(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("unk");
            readByte("unk");
            close();
        }
    }

    public class PKT_S2C_Surrender : Packet
    {
        public PKT_S2C_Surrender(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("unk");
            readInt("surrenderNetworkId");
            readByte("yesVotes");
            readByte("noVotes");
            readByte("maxVotes");
            readByte("team");
            close();
        }
    }
    public class PKT_S2C_SurrenderResult : Packet
    {
        public PKT_S2C_SurrenderResult(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("early");
            readByte("yesVotes");
            readByte("noVotes");
            readByte("team");
            close();
        }
    }

    public class EPKT_S2C_OnAttack : Packet
    {
        public EPKT_S2C_OnAttack(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("ecmd");
            readByte("unk");
            readByte("attackType");
            readFloat("x");
            readFloat("z");
            readFloat("y");
            readInt("attackerNetId");
            close();
        }
    }

    public class PKT_S2C_SetCooldown : Packet
    {
        public PKT_S2C_SetCooldown(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("slotId");
            readByte("unk(0xF8)");
            readFloat("totalCd");
            readFloat("currentCd");
            close();
        }

    }

    public class PKT_S2C_SetTarget : Packet
    {
        public PKT_S2C_SetTarget(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("attackerNetId");
            readInt("atackedNetId");
            close();
        }

    }

    public class PKT_S2C_SetTarget2 : PKT_S2C_SetTarget
    {
        public PKT_S2C_SetTarget2(byte[] bytes, float time) : base(bytes, time)
        {

        }

    }

    public class PKT_S2C_ChampionDie : Packet
    {

        public PKT_S2C_ChampionDie(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("killedNetId");
            readInt("goldFromKill");
            readByte("unk");
            readInt("killerNetId");
            readByte("unk");
            readByte("unk");
            readFloat("respawnTimer");
            close();
        }
    }

    public class EPKT_S2C_ChampionDeathTimer : Packet
    {

        public EPKT_S2C_ChampionDeathTimer(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("ecmd");
            readByte("unk");
            readFloat("respawnTimer");
            close();
        }
    }

    public class PKT_S2C_ChampionRespawn : Packet
    {
        public PKT_S2C_ChampionRespawn(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readFloat("x");
            readFloat("y");
            readFloat("z");
            close();
        }
    }

    public class PKT_S2C_ShowProjectile : Packet
    {
        public PKT_S2C_ShowProjectile(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("ownerNetId");
            readInt("projectileNetId");
            close();
        }
    }

    public class PKT_S2C_SetHealth : Packet
    {
        public PKT_S2C_SetHealth(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readUInt("netId/itemHash");
            readShort("unk");
            if (Reader.BaseStream.Position < Reader.BaseStream.Length)
            {
                readFloat("maxHP");
                readFloat("currentHp");
            }
            close();
        }
    }

    public class PKT_C2S_CastSpell : Packet
    {
        public PKT_C2S_CastSpell(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("spellSlotType");
            readByte("spellSlot");
            readFloat("x");
            readFloat("y");
            readFloat("x2");
            readFloat("y2");
            readInt("targetNetId");
            close();
        }
    }

    public class PKT_S2C_CastSpellAns : Packet
    {

        public PKT_S2C_CastSpellAns(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("tickCount");

            readByte("unk");
            readByte("unk");
            readByte("unk");
            readInt("spellHash");
            readInt("spellNetId");
            readByte("unk");
            readFloat("unk");
            readInt("ownerNetId");
            readInt("ownerNetId");
            readInt("ownerChampionHash");
            readInt("futureProjectileNetId");
            readFloat("x");
            readFloat("z");
            readFloat("y");
            readFloat("x");
            readFloat("z");
            readFloat("y");
            readByte("unk");
            readFloat("castTime");
            readFloat("unk");
            readFloat("unk");
            readFloat("cooldown");
            readFloat("unk");
            readByte("unk");
            readByte("spellSlot");
            readFloat("manacost");
            readFloat("ownerX");
            readFloat("ownerZ");
            readFloat("ownerY");
            readLong("unk");
            close();
        }
    }

    public class PKT_S2C_PlayerInfo : Packet
    {
        public PKT_S2C_PlayerInfo(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            #region wtf
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");

            readInt("summonerSpell1");
            readInt("summonerSpell2");

            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");

            #endregion
            close();
        }
    }

    public class PKT_S2C_SpawnProjectile : Packet
    {

        public PKT_S2C_SpawnProjectile(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");

            readFloat("x");
            readFloat("z");
            readFloat("y");
            readFloat("x");
            readFloat("z");
            readFloat("y");
            readLong("unk");
            readFloat("unk");
            readFloat("targetX");
            readFloat("targetZ");
            readFloat("targetY");
            readFloat("targetX");
            readFloat("targetZ");
            readFloat("targetY");
            readFloat("targetX");
            readFloat("targetZ");
            readFloat("targetY");
            readFloat("x");
            readFloat("z");
            readFloat("y");
            readInt("unk");
            readFloat("projectileMoveSpeed");
            readLong("unk");
            readInt("unk");
            readByte("unk");
            readByte("unk");
            readByte("unk");
            readInt("projectileId");
            readInt("unk");
            readByte("unk");
            readFloat("unk");
            readInt("ownerNetId");
            readInt("ownerNetId");
            readInt("championHash");
            readInt("projectileNetId");
            readFloat("targetX");
            readFloat("targetZ");
            readFloat("targetY");
            readFloat("targetX");
            readFloat("targetZ");
            readFloat("targetY");
            readUInt("unk");
            readInt("unk");
            readUInt("unk");
            readInt("unk");
            readInt("unk");
            readShort("unk");
            readByte("unk");
            readInt("unk");
            readFloat("x");
            readFloat("z");
            readFloat("y");
            readLong("unk");
            close();
        }

    }

    public class PKT_S2C_SpawnParticle : Packet
    {
        public PKT_S2C_SpawnParticle(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("ownerNetId");
            readShort("numberOfParticles");
            readInt("championHash");
            readInt("particleHash");
            readInt("unk");
            readInt("unk");
            readShort("unk");
            readShort("numberOfTargets?");
            readInt("ownerNetId");
            readInt("particleNetId");
            readInt("ownerNetId");
            readInt("netId");
            readInt("unk");

            for (var i = 0; i < 3; ++i)
            {
                readShort("width");
                readFloat("unk");
                readShort("height");
            }

            readInt("unk");
            readInt("unk");
            readInt("unk");
            readInt("unk");
            readFloat("unk");
            close();
        }
    }

    public class PKT_S2C_DestroyProjectile : Packet
    {
        public PKT_S2C_DestroyProjectile(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
        }
    }
        /*
                public class PKT_S2C_CharStats : Packet
                {
                    public PKT_S2C_CharStats(byte[] bytes, float time) : base(bytes, time)
                    {
                        readByte("cmd");
                        readInt("netId");
                        readInt("tickCount");

                        readByte("numUpdates");
                        readByte("masterMask");
                        readInt("netId");

                        foreach (var m in masks)
                        {
                            int mask = 0;
                            byte size = 0;

                            var updatedStats = stats[m];
                            updatedStats.Sort();
                            foreach (var it in updatedStats)
                            {
                                size += u.getStats().getSize(m, it);
                                mask |= it;
                            }
                            //if (updatedStats.Contains((int)FieldMask.FM1_SummonerSpells_Enabled))
                            //  System.Diagnostics.Debugger.Break();
                            buffer.Write((int)mask);
                            buffer.Write((byte)size);

                            for (int i = 0; i < 32; i++)
                            {
                                int tmpMask = (1 << i);
                                if ((tmpMask & mask) > 0)
                                {
                                    if (u.getStats().getSize(m, tmpMask) == 4)
                                    {
                                        float f = u.getStats().getStat(m, tmpMask);
                                        var c = BitConverter.GetBytes(f);
                                        if (c[0] >= 0xFE)
                                        {
                                            c[0] = (byte)0xFD;
                                        }
                                        buffer.Write(BitConverter.ToSingle(c, 0));
                                    }
                                    else if (u.getStats().getSize(m, tmpMask) == 2)
                                    {
                                        short stat = (short)Math.Floor(u.getStats().getStat(m, tmpMask) + 0.5);
                                        buffer.Write(stat);
                                    }
                                    else
                                    {
                                        byte stat = (byte)Math.Floor(u.getStats().getStat(m, tmpMask) + 0.5);
                                        buffer.Write(stat);
                                    }
                                }
                            }
                        }
                    }
                }
                */
    public class PKT_S2C_LevelPropSpawn : Packet
    {
        public PKT_S2C_LevelPropSpawn(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("levelPropNetId");
            readInt("unk");
            readByte("unk");
            readFloat("x");
            readFloat("z");
            readFloat("y");
            readFloat("rotationZ");

            readFloat("directionX");
            readFloat("directionZ");
            readFloat("directionY");
            readFloat("unk");
            readFloat("unk");

            readFloat("unk");
            readFloat("unk");
            readFloat("unk");
            readInt("unk");
            readInt("propType");// nPropType [size 1 . 4] (4.18) -- if is a prop, become unselectable and use direction params
            readString(64, "levelPropName");
            readString(64, "levelPropType");
            close();
        }
    }

    public class PKT_C2S_ViewReq : Packet
    {
        public PKT_C2S_ViewReq(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readFloat("x");
            readFloat("zoom");
            readFloat("y");
            readFloat("y2");
            readInt("width");
            readInt("height");
            readInt("unk");
            readByte("requestNo");
            close();
        }
    }

    public class PKT_S2C_LevelUp : Packet
    {
        public PKT_S2C_LevelUp(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("level");
            readShort("skillPoints");
            close();
        }
    }

    public class PKT_S2C_ViewAns : Packet
    {
        public PKT_S2C_ViewAns(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("requestNo");
            close();
        }
    }

    public class PKT_S2C_DebugMessage : Packet
    {
        public PKT_S2C_DebugMessage(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readInt("unk");
            readString(512, "message");
            close();
        }
    }

    public class SetCooldown : Packet
    {
        public SetCooldown(byte[] bytes, float time) : base(bytes, time)
        {
            readByte("cmd");
            readInt("netId");
            readByte("slotId");
            readShort("unk");
            readFloat("totalCd");
            readFloat("currentCd");
            close();
        }
    }
}