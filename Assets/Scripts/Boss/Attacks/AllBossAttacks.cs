using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data/Attacks/Overview", fileName = "AttackOverview")]
public class AllBossAttacks : ScriptableObject
{
    public List<MetaAttack> attacks = new List<MetaAttack>();
}

[System.Serializable]
public class MetaAttack
{
    public BossAttack attack;
    public int runsToUnlock;
    public int minimumPlayerKillsToUnlock;
    public int minimumHighScoreToUnlock;
}