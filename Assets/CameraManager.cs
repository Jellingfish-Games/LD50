using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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
        BattleManager.instance.player.RestrictControls();
        BattleManager.instance.player.LockInPlace();
        var guy = BattleManager.instance.SpawnLittleGuy();
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
}
