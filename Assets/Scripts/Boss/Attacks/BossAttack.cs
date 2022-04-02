using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Data/Boss/Attack", fileName = "AttackData")]
public abstract class BossAttack : ScriptableObject
{
    public string attackName;
    public string description;

    public float sweetspotDamage;
    public float sourspotDamage;

    public List<AnimationClip> animationClips;

    public float windupTime;
    public float backswingTime;

    public abstract IEnumerator PerformAttack(BossCharacter self);
}
