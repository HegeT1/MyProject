using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private List<WaveScriptableObject> _waves;

    private GameManager _gameManagerScript;

    [field: SerializeField] public List<GameObject> Enemies { get; private set; } = new();
    private bool _canSpawnNextWave;

    void Start()
    {
        _gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _waves = Resources.LoadAll<WaveScriptableObject>("Waves").ToList();

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
            WaveScriptableObject wave = _waves.Find(x => x.WaveNumber == waveNumber);
            for (int i = 0; i < wave.WaveParts.Count; i++)
            {
                if (!_gameManagerScript.IsGameActive)
                    yield break;
                StartCoroutine(SpawnEnemyWavePart(wave.WaveParts[i], i));
                yield return new WaitForSeconds(wave.WaveParts[i].DelayForNextPart);
            }
        }
        yield break;
    }

    IEnumerator SpawnEnemyWavePart(WavePart wavePart, int partIndex)
    {
        for (int i = 0; i < wavePart.EnemyCount; i++)
        {
            if (!_gameManagerScript.IsGameActive)
                yield break;
            SpawnEnemy(wavePart.EnemyObject.Prefab, _gameManagerScript.Points[0].transform.position, wavePart.EnemyObject.Prefab.transform.rotation, wavePart.EnemyObject.BaseStats);
            yield return new WaitForSeconds(wavePart.EnemySpacing);
        }
        _canSpawnNextWave = IsEnemyWavePartLast(partIndex);
        yield break;
    }

    void SpawnEnemy(GameObject enemyPrefab, Vector2 position, Quaternion rotation, EnemyStats stats)
    {
        GameObject enemy = Instantiate(enemyPrefab, position, rotation);
        enemy.GetComponent<Enemy>().SetStats(stats);
        Enemies.Add(enemy);
    }

    bool IsEnemyWavePartLast(int partIndex)
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
