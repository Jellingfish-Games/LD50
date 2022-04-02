using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleGuyAnimationMessenger : MonoBehaviour
{
	private LittleGuyAI ai;

	private void Start()
	{
		ai = transform.parent.GetComponent<LittleGuyAI>();
	}

	void MessageParentAI(string animationName)
	{
		ai.OnAnimationEnd(animationName);
	}
}
