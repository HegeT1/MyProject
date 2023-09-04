using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public class EnemyWavePart
    {
        public int NumberOfEnemies { get; set; }
        public float Spacing { get; set; }
        public float DelayForNextPart { get; set; }
        public GameObject EnemyPrefab { get; set; }
    }

    public class EnemyWave
    {
        public int WaveNumber { get; set; }
        public List<EnemyWavePart> WaveParts { get; set; } = new();
    }
    private List<EnemyWave> _waves = new();

    private GameManager _gameManagerScript;

    public GameObject EnemyPrefab;
    public GameObject ToughEnemyPrefab;

    [field: SerializeField] public List<GameObject> Enemies { get; private set; } = new();
    private bool _canSpawnNextWave;

    void Start()
    {
        _gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();

        EnemyWavePart part1 = new() { NumberOfEnemies = 1, Spacing = 0.5f, DelayForNextPart = 4f, EnemyPrefab = EnemyPrefab };
        EnemyWavePart part2 = new() { NumberOfEnemies = 5, Spacing = 1f, DelayForNextPart = 8f, EnemyPrefab = EnemyPrefab };

        EnemyWavePart part3 = new() { NumberOfEnemies = 20, Spacing = 0.1f, DelayForNextPart = 2f, EnemyPrefab = EnemyPrefab };
        EnemyWavePart part4 = new() { NumberOfEnemies = 5, Spacing = 0.2f, DelayForNextPart = 7f, EnemyPrefab = ToughEnemyPrefab };

        _waves.Add(new() { WaveNumber = 1, WaveParts = new() { part1, part2 } });
        _waves.Add(new() { WaveNumber = 2, WaveParts = new() { part3, part4, part4, part1 } });
        _waves.Add(new() { WaveNumber = 3, WaveParts = new() { part4, part4 } });

        _gameManagerScript.StartGame();
        _canSpawnNextWave = true;
    }

    void Update()
    {
        if (Enemies.Count == 0 && _gameManagerScript.IsGameActive && _canSpawnNextWave)
        {
            if (_gameManagerScript.WaveNumber <= _waves.Count)
            {
                _gameManagerScript.UpdateWaveText(1);
                StartCoroutine(SpawnEnemyWave(_gameManagerScript.WaveNumber));
            }
        }
    }

    IEnumerator SpawnEnemyWave(int waveNumber)
    {
        _canSpawnNextWave = false;
        if (waveNumber <= _waves.Count)
        {
            for (int i = 0; i < _waves[waveNumber - 1].WaveParts.Count; i++)
            {
                if (!_gameManagerScript.IsGameActive)
                    yield break;
                StartCoroutine(SpawnEnemyWavePart(_waves[waveNumber - 1].WaveParts[i], i));
                yield return new WaitForSeconds(_waves[waveNumber - 1].WaveParts[i].DelayForNextPart);
            }
        }
        yield break;
    }

    IEnumerator SpawnEnemyWavePart(EnemyWavePart enemyWavePart, int partIndex)
    {
        for (int i = 0; i < enemyWavePart.NumberOfEnemies; i++)
        {
            if (!_gameManagerScript.IsGameActive)
                yield break;
            Enemies.Add(Instantiate(enemyWavePart.EnemyPrefab, _gameManagerScript.Points[0].transform.position, enemyWavePart.EnemyPrefab.transform.rotation));
            yield return new WaitForSeconds(enemyWavePart.Spacing);
        }
        _canSpawnNextWave = IsEnemyWavePartLast(partIndex);
        yield break;
    }

    private bool IsEnemyWavePartLast(int partIndex)
    {
        if (partIndex == _waves[_gameManagerScript.WaveNumber - 1].WaveParts.Count - 1)
        {
            return true;
        }
        return false;
    }

    public void RemoveEnemyFromList(GameObject enemy)
    {
        Enemies.Remove(enemy);
    }
}
