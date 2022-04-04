using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "Data/Boss/Attack", fileName = "AttackData")]
public abstract class BossAttack : ScriptableObject
{
    public string attackName;
    public string description;

    public Sprite icon;

    public float sweetspotDamage;
    public float sourspotDamage;

    public List<AnimationClip> animationClips;

    public float windupTime;
    public float backswingTime;

    public GameObject attackHitboxes;

    public int Range;
    public int Power;
    public int Speed;

    public abstract IEnumerator PerformAttack(BossCharacter self);
}
