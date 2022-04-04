using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    public static CameraManager i;

    public GameObject cameraObj;
    public GameObject cameraBrain;
    public CinemachineVirtualCamera vc;
    public CinemachineTargetGroup targetGroup;
    public Transform doorLookAt;
    public Transform doorFollow;

    void Awake()
    {
        if (i == null)
        {
            i = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public void Cutscene()
    {
        StartCoroutine(CustsceneCoroutine());
    }

    IEnumerator CustsceneCoroutine()
    {
        var guy = BattleManager.instance.SpawnLittleGuy();

        if (!BattleManager.instance.encounteredLittleGuyStatPackages.Contains(guy.info.StatPackage))
		{
            BattleManager.instance.player.RestrictControls();
            BattleManager.instance.player.LockInPlace();
            vc.LookAt = guy.transform;
            vc.Follow = doorFollow;
            yield return guy.EnterCoroutine();
            //yield return new WaitForSeconds(4);
            BattleManager.instance.player.EnableControls();
            BattleManager.instance.player.UnlockPlace();
            vc.LookAt = targetGroup.transform;
            vc.Follow = targetGroup.transform;
            BattleManager.SwitchToNewState(BattleManager.BattleState.Battle);
        }
        else
		{
            yield return guy.ShortEnterCoroutine();
            BattleManager.SwitchToNewState(BattleManager.BattleState.Battle);
        }
    }

    public void Shake(float duration, float amplitude = 1)
    {
        StartCoroutine(ShakeCoroutine(duration, amplitude));
    }

    IEnumerator ShakeCoroutine(float duration, float amplitude = 1)
    {
        var perlin = vc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = amplitude;
        //perlin.enabled = true;
        yield return new WaitForSeconds(duration);
        perlin.m_AmplitudeGain = 0;
        //perlin.enabled = false;
    }
}
