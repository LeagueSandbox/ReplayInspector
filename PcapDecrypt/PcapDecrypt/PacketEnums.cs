using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcapDecrypt
{
    public enum EnetOpCodes : byte
    {
        NONE = 0x00,
        ACKNOWLEDGE = 0x01,
        CONNECT = 0x02,
        VERIFY_CONNECT = 0x03,
        DISCONNECT = 0x04,
        PING = 0x05,
        SEND_RELIABLE = 0x06,
        SEND_UNRELIABLE = 0x07,
        SEND_FRAGMENT = 0x08,
        SEND_UNSEQUENCED = 0x09,
        BANDWIDTH_LIMIT = 0x0A,
        THROTTLE_CONFIGURE = 0x0B,
        COUNT = 0x0C
    }
    public enum PacketCmdS2C : byte
    {
        PKT_S2C_MoveAns = 0x61, //?
        PKT_S2C_KeyCheck = 0x00,
        PKT_S2C_RemoveItem = 0x0B,
        PKT_S2C_NextAutoAttack = 0x0C,
        PKT_S2C_SurrenderState = 0x0E,
        PKT_S2C_EndSpawn = 0x11,
        PKT_S2C_SkillUp = 0x15,
        PKT_S2C_BeginAutoAttack = 0x1A,
        PKT_S2C_AddGold = 0x22,
        PKT_S2C_FogUpdate2 = 0x23,
        PKT_S2C_EditBuff = 0x1C,
        PKT_S2C_PlayerInfo = 0x2A,
        PKT_S2C_ViewAns = 0x2C,
        PKT_S2C_ChampionRespawn = 0x2F,
        PKT_S2C_StopAutoAttack = 0x34,
        PKT_S2C_DeleteObject = 0x35, // not sure what this is, happens when turret leaves vision
        PKT_S2C_SpawnProjectile = 0x3B,
        PKT_S2C_SwapItems = 0x3E,
        PKT_S2C_LevelUp = 0x3F,
        PKT_S2C_AttentionPing = 0x40,
        PKT_S2C_Emotion = 0x42,
        PKT_S2C_Announce = 0x45,
        PKT_S2C_HeroSpawn = 0x4C,
        // Packet 0xC0 format is [Net ID 1] [Net ID 2], purpose still unknown
        PKT_S2C_GameTimer = 0xC1,
        PKT_S2C_GameTimerUpdate = 0xC2,
        PKT_S2C_FaceDirection = 0x50,
        PKT_S2C_LeaveVision = 0x51,
        PKT_S2C_SynchVersion = 0x54,
        PKT_S2C_DestroyProjectile = 0x5A,
        PKT_S2C_StartGame = 0x5C,
        PKT_S2C_ChampionDie = 0x5E,
        PKT_S2C_StartSpawn = 0x62,
        PKT_S2C_Dash = 0x64,
        PKT_S2C_DamageDone = 0x65,
        PKT_S2C_LoadHero = 0x65,
        PKT_S2C_LoadName = 0x66,
        PKT_S2C_LoadScreenInfo = 0x67,
        PKT_S2C_ChatBoxMessage = 0x68,
        PKT_S2C_SetTarget = 0x6A,
        PKT_S2C_SetAnimation = 0x6B,
        PKT_S2C_ShowProjectile = 0x6E,
        PKT_S2C_BuyItemAns = 0x6F,
        PKT_S2C_AddBuff = 0xB7,
        PKT_S2C_RemoveBuff = 0x7B,
        PKT_S2C_SetCooldown = 0x85,
        PKT_S2C_SpawnParticle = 0x87,
        PKT_S2C_QueryStatusAns = 0x88,
        PKT_S2C_World_SendGameNumber = 0x92,
        PKT_S2C_Ping_Load_Info = 0x95,
        PKT_S2C_UpdateModel = 0x97,
        PKT_S2C_TurretSpawn = 0x9D,
        PKT_S2C_NPC_Hide = 0x9E, // (4.18) not sure what this became
        PKT_S2C_SurrenderResult = 0xA5,
        PKT_S2C_SetHealth = 0xAE,
        PKT_S2C_SpellAnimation = 0xB0,
        PKT_S2C_CastSpellAns = 0xB5,
        PKT_S2C_ObjectSpawn = 0xBA,
        PKT_S2C_SetTarget2 = 0xC0,
        PKT_S2C_CharStats = 0xC4,
        PKT_S2C_GameEnd = 0xC6,
        PKT_S2C_Surrender = 0xC9,

        PKT_S2C_LevelPropSpawn = 0xD0,
        PKT_S2C_DebugMessage = 0xF7,
        PKT_S2C_Extended = 0xFE,
        PKT_S2C_Batch = 0xFF
    };

    public enum PacketCmdC2S
    {
        PKT_C2S_ChatBoxMessage = 0x68,
        PKT_C2S_KeyCheck = 0x00,
        PKT_C2S_World_SendGameNumber = 0x92,

        PKT_C2S_HeartBeat = 0x08,
        PKT_C2S_SellItem = 0x09,
        PKT_C2S_QueryStatusReq = 0x14,
        PKT_C2S_Ping_Load_Info = 0x16,
        PKT_C2S_SwapItems = 0x20,
        PKT_C2S_ViewReq = 0x2E,
        PKT_C2S_SkillUp = 0x39,
        PKT_C2S_AutoAttackOption = 0x47,
        PKT_C2S_Emotion = 0x48,
        PKT_C2S_StartGame = 0x52,
        PKT_C2S_ScoreBord = 0x56,
        PKT_C2S_AttentionPing = 0x57,
        PKT_C2S_ClientReady = 0x64,
        PKT_C2S_MoveReq = 0x72,
        PKT_C2S_MoveConfirm = 0x77,
        PKT_C2S_LockCamera = 0x81,
        PKT_C2S_BuyItemReq = 0x82,
        PKT_C2S_Exit = 0x8F,
        PKT_C2S_CastSpell = 0x9A,
        PKT_C2S_PauseReq = 0xA1,
        PKT_C2S_Surrender = 0xA4,
        PKT_C2S_StatsConfirm = 0xA8,
        PKT_C2S_Click = 0xAF,
        PKT_C2S_SynchVersion = 0xBD,
        PKT_C2S_CharLoaded = 0xBE,
        PKT_C2S_Batch = 0xFF
    }

}
