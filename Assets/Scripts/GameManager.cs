using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [field: SerializeField] public List<GameObject> Points { get; private set; }

    [field: SerializeField] public TextMeshProUGUI WaveText { get; private set; }
    [field: SerializeField] public TextMeshProUGUI HealthText { get; private set; }

    [field: SerializeField] public int WaveNumber { get; private set; }
    [field: SerializeField] public int PlayerHealth { get; private set; } = 100;
    [field: SerializeField] public bool IsGameActive { get; private set; } = false;
    [field: SerializeField] public bool IsGamePaused { get; private set; } = false;

    private void Start()
    {
        UpdateWaveText(0);
        UpdatePlayerHealth(0);
    }

    public void UpdateWaveText(int waveToAdd)
    {
        WaveNumber += waveToAdd;
        WaveText.SetText("Wave: " + WaveNumber);
    }

    public void UpdatePlayerHealth(int healthToAdd)
    {
        PlayerHealth += healthToAdd;
        if (PlayerHealth < 0)
            PlayerHealth = 0;
        HealthText.SetText("Health: " +  PlayerHealth);
        if(PlayerHealth <= 0)
        { 
            IsGameActive = false;
        }
    }

    public void StartGame()
    {
        IsGameActive = true;
    }
}
