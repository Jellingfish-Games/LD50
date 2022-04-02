using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Boss/Properties", fileName ="BossProperties")]
public class BossPropertyData : ScriptableObject
{
    public BossProperties properties;
}

[System.Serializable]
public struct BossProperties
{
    public float maxHP;
    public float damage;
    public float turnRate;
    public float movementSpeed;
    public float attackSpeed;
    public float armor;

    [Range(0f, 1f)]
    public List<float> phaseTransitionTheresholds;

    public void Merge(BossProperties other)
    {
        damage += other.damage;
        turnRate += other.turnRate;
        movementSpeed += other.movementSpeed;
        attackSpeed += other.movementSpeed;
        armor += other.armor;
    }
}