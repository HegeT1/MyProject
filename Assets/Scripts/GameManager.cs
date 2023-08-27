using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> points;

    public TextMeshProUGUI waveText;

    public int waveNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateWaveText(int waveToAdd)
    {
        waveNumber += waveToAdd;
        waveText.SetText("Wave: " + waveNumber);
    }
}
