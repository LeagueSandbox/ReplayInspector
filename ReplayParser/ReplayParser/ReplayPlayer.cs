using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;

namespace ReplayParser
{
  [DataContract]
  public class ReplayPlayer 
  {
    [DataMember]
    public string summoner { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int wins { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int losses { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int leaves { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int summonerLevel { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int profileIconId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int elo { get; set; }

    [DataMember]
    public string champion { get; set; }

    [DataMember]
    public bool won { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool leaver { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int eloChange { get; set; }

    [DataMember]
    public int team { get; set; }

    [DataMember]
    public long accountID { get; set; }

    [DataMember]
    public int kills { get; set; }

    [DataMember]
    public int deaths { get; set; }

    [DataMember]
    public int assists { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int minions { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int turrets { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int barracks { get; set; }

    [DataMember]
    public int level { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int timeDead { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int damageTaken { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int physicalDamageTaken { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int magicDamageTaken { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int damageDealt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int largestCrit { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int physicalDamageDealt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int magicDamageDealt { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int killingSpree { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int largestMultiKill { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int healed { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int gold { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int spell1 { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int spell2 { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int item1 { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int item2 { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int item3 { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int item4 { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int item5 { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int item6 { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int item7 { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int neutralMinionsKilled { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int nodeNeutralizes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int nodeNeutralizeAssists { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int nodeCaptures { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int nodeCaptureAssists { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int totalPlayerScore { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int objectivePlayerScore { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int victoryPointTotal { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int totalScoreRank { get; set; }
  }
}
