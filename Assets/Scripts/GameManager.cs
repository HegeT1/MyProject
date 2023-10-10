using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState { None, Active, Paused, Victory, Defeat }

public class GameManager : MonoBehaviour
{
    [field: SerializeField] public List<GameObject> Points { get; set; }

    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _moneyText;

    [field: SerializeField] public int WaveNumber { get; private set; }
    [field: SerializeField] public int PlayerHealth { get; private set; } = 100;

    public GameState GameState = GameState.None;
    [field: SerializeField] public float ReselValue { get; private set; } = 0.3f;

    [field: SerializeField] public float Money { get; private set; } = 300;

    private void Start()
    {
        UpdateWave(0);
        UpdatePlayerHealth(0);
        UpdateMoney(0);
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
            Defeat();
        }
    }

    public void UpdateMoney(float moneyToAdd)
    {
        Money += moneyToAdd;
        _moneyText.SetText(Money.ToString());
    }

    public void StartGame()
    {
        GameState = GameState.Active;
    }

    public void SetGameSpeed(float speed)
    {
        Time.timeScale = speed;
    }

    public void Victory()
    {
        GameState = GameState.Victory;
        Debug.Log("Victory!");
    }

    public void Defeat()
    {
        Time.timeScale = 0;
        GameState = GameState.Defeat;
        Debug.Log("Defeat!");
    }
}
