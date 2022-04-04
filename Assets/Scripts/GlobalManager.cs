using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalManager : MonoBehaviour
{
    public enum GameMode { Singleplayer, Multiplayer }

    public GameMode gameMode;

    public CanvasGroup blackCanvas;

    private static GlobalManager _instance;

    public static GlobalManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject().AddComponent<GlobalManager>();
                Canvas c = _instance.gameObject.AddComponent<Canvas>();
                c.sortingOrder = 999;
                c.sortingLayerName = "UIForeground";
                c.renderMode = RenderMode.ScreenSpaceOverlay;
                RectTransform child = new GameObject().AddComponent<RectTransform>();
                Image i = child.gameObject.AddComponent<Image>();
                i.color = Color.black;
                child.SetParent(_instance.transform);
                _instance.blackCanvas = child.gameObject.AddComponent<CanvasGroup>();
                _instance.blackCanvas.alpha = 0;
                _instance.blackCanvas.blocksRaycasts = false;
                _instance.blackCanvas.interactable = false;
                child.anchorMin = Vector2.zero;
                child.anchorMax = Vector2.one;
                child.anchoredPosition = Vector2.zero;
                child.sizeDelta = Vector2.one;
            }

            return _instance;
        } set
        {
            _instance = value;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(this);
            _instance = this;
        } else
        {
            Destroy(this);
        }

    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadMain());
    }

    public void StartSingleplayer()
    {
        gameMode = GameMode.Singleplayer;
        StartCoroutine(LoadGame());
    }

    public void StartMultiplayer()
    {
        gameMode = GameMode.Multiplayer;
        StartCoroutine(LoadGame());
    }

    public void GiveUp()
    {
        StartCoroutine(LoadMain());
    }

    private IEnumerator LoadMain()
    {
        yield return FadeOut(0);
    }

    private IEnumerator LoadGame()
    {
        yield return FadeOut(1);
    }

    private IEnumerator FadeOut(int idxToLoad)
    {
        AudioManager.ChangeMusic(null);
        blackCanvas.interactable = true;
        blackCanvas.blocksRaycasts = true;
        while (blackCanvas.alpha < 1)
        {
            blackCanvas.alpha += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(idxToLoad);

        blackCanvas.interactable = false;
        blackCanvas.blocksRaycasts = false;
        while (blackCanvas.alpha > 0)
        {
            blackCanvas.alpha -= Time.deltaTime;
            yield return null;
        }
    }
}
