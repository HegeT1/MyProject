using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameHandler : MonoBehaviour
{
    private GameManager _gameManagerScritp;

    // Start is called before the first frame update
    void Start()
    {
        _gameManagerScritp = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        _gameManagerScritp.StartGame();
    }
}
