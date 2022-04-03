using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YouDiedMenu : MonoBehaviour
{
    public Transform youDiedTransform;
    public Transform enemyDiedTransform;

    public IEnumerator DoSequence(bool didYouDie)
    {
        youDiedTransform.gameObject.SetActive(didYouDie);
        enemyDiedTransform.gameObject.SetActive(!didYouDie);

        Transform targetTransform = didYouDie ? youDiedTransform : enemyDiedTransform;

        targetTransform.localScale = Vector3.one * 0.5f;

        while (targetTransform.localScale.magnitude < Vector3.one.magnitude * 1.5f)
        {
            targetTransform.localScale += Vector3.one * Time.deltaTime;
            yield return null;
        }
    }
}
