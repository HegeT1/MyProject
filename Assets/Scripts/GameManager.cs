using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState { None, Active, Paused, Victory, Loss }

public class GameManager : MonoBehaviour
{
    [field: SerializeField] public List<GameObject> Points { get; set; }

    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _moneyText;

    [field: SerializeField] public int WaveNumber { get; private set; }
    [field: SerializeField] public int PlayerHealth { get; private set; } = 100;

    [field: SerializeField] public GameState GameState { get; private set; } = GameState.None;

    //[field: SerializeField] public bool IsGameActive { get; private set; } = false;
    //[field: SerializeField] public bool IsGamePaused { get; private set; } = false;

    [SerializeField] private float _money;

    private void Start()
    {
        UpdateWave(0);
        UpdatePlayerHealth(0);
        UpdateMoney(100);
    }

    public void UpdateWave(int waveToAdd)
    {
        WaveNumber += waveToAdd;
        _waveText.SetText("Wave: " + WaveNumber);
    }

    public void UpdatePlayerHealth(int healthToAdd)
    {
        PlayerHealth += healthToAdd;
        if (PlayerHealth < 0)
            PlayerHealth = 0;
        _healthText.SetText(PlayerHealth.ToString());
        if(PlayerHealth <= 0)
        {
            //IsGameActive = false;
            GameState = GameState.Loss;
        }
    }

    public void UpdateMoney(float moneyToAdd)
    {
        _money += moneyToAdd;
        _moneyText.SetText(_money.ToString());
    }

    public void StartGame()
    {
        GameState = GameState.Active;
        //IsGameActive = true;
    }
}
