using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public CanvasGroup ownGroup;
    public AudioMixer masterMixer;

    public Slider musicVolSlider;
    public Slider soundVolSlider;

    private void Start()
    {
        masterMixer.GetFloat("musicVol", out float m);
        masterMixer.GetFloat("soundVol", out float s);
        musicVolSlider.SetValueWithoutNotify(Mathf.Pow(10, m / 20));
        soundVolSlider.SetValueWithoutNotify(Mathf.Pow(10, s / 20));
    }

    public void Pause()
    {
        if (ownGroup.alpha == 1)
        {
            Resume();
            return;
        }

        if (BattleManager.state == BattleManager.BattleState.Cutscene || BattleManager.state == BattleManager.BattleState.Battle)
        {
            EnablePause();
        }
    }

    public void EnablePause()
    {
        ownGroup.blocksRaycasts = true;
        ownGroup.alpha = 1;
        Time.timeScale = 0;
        ownGroup.interactable = true;
    }

    public void DisablePause()
    {
        ownGroup.blocksRaycasts = false;
        ownGroup.alpha = 0;
        Time.timeScale = 1;
        ownGroup.interactable = false;
    }

    public void Resume()
    {
        DisablePause();
    }
    public void Retry()
    {
        DisablePause();
        GlobalManager.instance.StartSingleplayer();
    }

    public void SetMusicGroupVolume(float volume)
    {
        masterMixer.SetFloat("musicVol", Mathf.Log(volume) * 20);
    }

    public void SetSFXGroupVolume(float volume)
    {
        masterMixer.SetFloat("soundVol", Mathf.Log(volume) * 20);
    }

    public void GiveUp()
    {
        DisablePause();
        GlobalManager.instance.LoadMainMenu();
    }
}
