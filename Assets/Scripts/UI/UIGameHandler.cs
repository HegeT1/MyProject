using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameHandler : MonoBehaviour
{
    private GameManager _gameManagerScript;

    void Start()
    {
        _gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    public void StartGame()
    {
        _gameManagerScript.StartGame();
    }

    public void RestartGame()
    {
        _gameManagerScript.RestartGame();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
