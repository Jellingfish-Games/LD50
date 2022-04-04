using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SFX
{
    BossDashHit,
    BossDashMiss,
    BossPunchHit,
    BossPunchSwing,
    BossSlashHit,
    BossSlashSwing,
    BossRoll,
    BossStep,
    BossLightBeamCharge,
    BossLightBeam,
    BossTripleSlashCharge,
    BossTripleSlash,
    BossGetHit,
    BossRoar,
    UIConfirm,
    UISelectHover,
    UIStartGame,
    GuyBombExplode,
    GuyBombFuse,
    GuyBombThrow,
    GuyDaggerHit,
    GuyDaggerSwing,
    GuyHammerHit,
    GuyHammerSwing,
    GuySwordHit,
    GuySwordSwing,
    GuyPotionDrink,
    GuyProjectileHit,
    GuyProjectileMiss,
    GuySpellCast,
    GuySpellShot,
    GuySpellMiss,
    GuyGetHit
}

public enum BGM
{
    None = -1,
    BossIntro,
    BossLoop,
    MainMenu
}


public class AudioManager : MonoBehaviour
{
    public AudioSource SFXSource;
    public AudioSource MusicSource;

    public AudioDefinition data;

    public AudioMixerGroup musicGroup;
    public AudioMixerGroup filteredMusicGroup;

    private static AudioManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public static async void ChangeMusic(AudioClip newClip)
    {
        if (newClip != instance.MusicSource.clip)
        {
            if (instance.MusicSource.clip != null)
            {
                float t = 1;
                while (t > 0)
                {
                    t -= Time.unscaledDeltaTime * 0.5f;
                    instance.MusicSource.volume = t;
                    await System.Threading.Tasks.Task.Yield();
                }
            }
            instance.MusicSource.clip = newClip;
            instance.MusicSource.Play();

            if (newClip != null)
            {
                float t = 0;
                while (t < 1)
                {
                    t += Time.unscaledDeltaTime * 0.5f;
                    instance.MusicSource.volume = Mathf.Min(t, 1);
                    await System.Threading.Tasks.Task.Yield();
                }
            }
        }
    }

    public static void ChangeMusic(BGM introClip, BGM loopingClip)
    {
        ChangeMusic(instance.data.GetMusic(introClip), instance.data.GetMusic(loopingClip));
    }

    public static void SetLowpassFilter(bool active)
    {
        instance.MusicSource.outputAudioMixerGroup = active ? instance.filteredMusicGroup : instance.musicGroup;
    }

    public static async void ChangeMusic(Music introClip, Music loopingClip)
    {
        if (((introClip != null && introClip.clip != instance.MusicSource.clip) || introClip == null) && loopingClip.clip != instance.MusicSource.clip)
        {
            if (introClip != null)
            {
                if (instance.MusicSource.clip != null)
                {
                    float t = 1;
                    while (t > 0)
                    {
                        t -= Time.unscaledDeltaTime * 0.5f;
                        instance.MusicSource.volume = t;
                        await System.Threading.Tasks.Task.Yield();
                    }
                }
                instance.MusicSource.clip = introClip.clip;
                instance.MusicSource.Play();
                instance.MusicSource.volume = introClip.volume;
                instance.MusicSource.loop = false;
                while (instance.MusicSource.isPlaying)
                {
                    await System.Threading.Tasks.Task.Yield();
                }
            }

            instance.MusicSource.clip = loopingClip.clip;
            instance.MusicSource.loop = true;
            instance.MusicSource.Play();
            instance.MusicSource.volume = loopingClip.volume;
        }
    }

    public static void PlaySound(SFX sfx, float volumeModifier = 1, AudioSource sourceToPlayFrom = null)
    {
        if (sourceToPlayFrom == null)
        {
            sourceToPlayFrom = instance.SFXSource;
        }
        Sound sound = instance.data.GetSound(sfx);
        if (sound != null)
        {
            sourceToPlayFrom.PlayOneShot(sound.clip, sound.volume * volumeModifier);
        }
    }

    public static void ToggleMusic()
    {
        instance.MusicSource.mute = !instance.MusicSource.mute;
        instance.SFXSource.mute = !instance.SFXSource.mute;
    }
}
