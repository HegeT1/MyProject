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
}
