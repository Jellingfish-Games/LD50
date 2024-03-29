using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LittleGuySpeechBubble : MonoBehaviour
{
    LittleGuyController controller;
	Vector2 targetPosition;
	bool overPlayer;
	RectTransform rect;
	Canvas canvas;
	RectTransform speechCornerTransform;
	Text speechText;

	bool started = false;

	private void Awake()
	{
		rect = GetComponent<RectTransform>();

		canvas = GetComponentInParent<Canvas>();
		speechCornerTransform = transform.Find("SpeechCorner").GetComponent<RectTransform>();
		speechText = GetComponentInChildren<Text>();

		rect.localScale = Vector3.zero;
	}

	public void AttachToPlayer(LittleGuyController lgc, string text)
	{
		controller = lgc;
		speechText.text = "";
		started = true;

		StartCoroutine(ShowText(text));
	}

	private IEnumerator ShowText(string text)
	{
		rect.DOScale(1f, 0.2f);
		yield return new WaitForSeconds(0.2f);

		string chars = text;

		for (int i = 0; i < chars.Length; i++)
		{
			speechText.text += chars[i];

			if (i % 2 == 0) SFX.GuyTalking.Play(0.25f);

			yield return new WaitForSeconds(0.035f);
		}

		yield return new WaitForSeconds(3f);

		rect.DOScale(0f, 0.3f);

		yield return new WaitForSeconds(0.3f);

		Destroy(gameObject);
	}

	private void Update()
	{
		if (started)
		{
			if (controller == null || controller.info.BattleStats.HP < 0)
			{
				Destroy(gameObject);
			}

			Vector2 playerOnScreen = Camera.main.WorldToScreenPoint(controller.transform.position);
			Vector2 target = Vector2.zero;

			float widthScale = canvas.scaleFactor;
			Vector2 rectScreenSize = rect.rect.size * widthScale;
			//playerOnScreen /= widthScale;


			overPlayer = playerOnScreen.y < 80f * widthScale;


			Vector2 playerOnScreenFromCenter = playerOnScreen - new Vector2(canvas.pixelRect.width, canvas.pixelRect.height) / widthScale;

			if (overPlayer)
			{
				if (playerOnScreen.x < 0)
				{
					target = playerOnScreen + new Vector2(Mathf.Max(-rect.rect.width * widthScale, -playerOnScreenFromCenter.x * 0.25f), rectScreenSize.y / 2 + 32 * widthScale);
				}
				else
				{
					target = playerOnScreen + new Vector2(Mathf.Min(rect.rect.width * widthScale, -playerOnScreenFromCenter.x * 0.25f), rectScreenSize.y / 2 + 32 * widthScale);
				}
			}
			else
			{
				if (playerOnScreen.x < 0)
				{
					target = playerOnScreen + new Vector2(Mathf.Max(-rect.rect.width * widthScale, -playerOnScreenFromCenter.x * 0.25f), -rectScreenSize.y / 2 - 8 * widthScale);
				}
				else
				{
					target = playerOnScreen + new Vector2(Mathf.Min(rect.rect.width * widthScale, -playerOnScreenFromCenter.x * 0.25f), -rectScreenSize.y / 2 - 8 * widthScale);
				}
			}

			transform.position = Vector3.Lerp(transform.position, target, 0.05f);// - rect.rect.size * widthScale;

			Vector2 cornerPos = Vector2.zero;
			Vector3 cornerScale = Vector3.one;

			if (overPlayer)
			{
				cornerPos.y = -rect.rect.height / 2 + 1;
			}
			else
			{
				cornerPos.y = rect.rect.height / 2 - 1;
				cornerScale.y = -1;
			}

			cornerPos.x = Mathf.Clamp((playerOnScreen.x - transform.position.x) / widthScale, -rect.rect.width / 2, rect.rect.width / 2);

			if (cornerPos.x > 0)
			{
				cornerScale.x = -1;
			}
			else
			{
				cornerScale.x = 1;
			}

			speechCornerTransform.localPosition = cornerPos;
			speechCornerTransform.localScale = cornerScale;
		}
	}
}
