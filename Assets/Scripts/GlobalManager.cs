using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalManager : MonoBehaviour
{
    public enum GameMode { Singleplayer, Multiplayer }

    public GameMode gameMode;

    private static GlobalManager _instance;

    public static GlobalManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject().AddComponent<GlobalManager>();
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
        SceneManager.LoadScene(0);
    }

    public void StartSingleplayer()
    {
        gameMode = GameMode.Singleplayer;
        SceneManager.LoadScene(1);
    }

    public void StartMultiplayer()
    {
        gameMode = GameMode.Multiplayer;
        SceneManager.LoadScene(1);
    }
}
