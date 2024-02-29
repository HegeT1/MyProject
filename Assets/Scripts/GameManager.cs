using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { None, Active, Paused, Victory, Defeat }

public class GameManager : MonoBehaviour
{
    [field: SerializeField] public List<GameObject> Points { get; set; }

    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private GameObject _defeatPanel;

    [field: SerializeField] public int WaveNumber { get; private set; }
    [field: SerializeField] public int PlayerHealth { get; private set; } = 100;
    [field: SerializeField] public float Money { get; private set; } = 200;
    [field: SerializeField] public float ReselPercent { get; private set; } = 0.7f;

    public GameState GameState = GameState.None;

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

    public void UpdateMoney(float moneyToAdd)
    {
        Money += moneyToAdd;
        _moneyText.SetText(Money.ToString());
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

    public void StartGame()
    {
        GameState = GameState.Active;
    }

    public void Victory()
    {
        Time.timeScale = 0;
        GameState = GameState.Victory;
        _victoryPanel.SetActive(true);
    }

    public void Defeat()
    {
        Time.timeScale = 0;
        GameState = GameState.Defeat;
        _defeatPanel.SetActive(true);
    }

    public void SetGameSpeed(float speed)
    {
        Time.timeScale = speed;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Main");
    }
}
