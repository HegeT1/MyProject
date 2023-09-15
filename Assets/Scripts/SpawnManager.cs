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
    private List<bool> _allPartsFinished = new();

    void Start()
    {
        _gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _waves = Resources.LoadAll<WaveScriptableObject>("Waves").ToList();
    }

    void Update()
    {
        if (Enemies.Count == 0 && _gameManagerScript.GameState == GameState.Active && CheckCanSpawnNextWave())
        {
            if (_gameManagerScript.WaveNumber <= _waves.Count)
            {
                _gameManagerScript.UpdateWave(1);
                StartCoroutine(SpawnEnemyWave(_gameManagerScript.WaveNumber));
            }
        }
    }

    IEnumerator SpawnEnemyWave(int waveNumber)
    {
        _allPartsFinished = new();
        if (waveNumber <= _waves.Count)
        {
            WaveScriptableObject wave = _waves.Find(x => x.Number == waveNumber);

            for(int i = 0; i < wave.Parts.Count; i++) 
                _allPartsFinished.Add(false);

            for (int i = 0; i < wave.Parts.Count; i++)
            {
                //if (!_gameManagerScript.IsGameActive)
                if (_gameManagerScript.GameState == GameState.Loss)
                    yield break;
                StartCoroutine(SpawnEnemyWavePart(wave.Parts[i], i));
                yield return new WaitForSeconds(wave.Parts[i].DelayForNextPart);
            }
        }
        yield break;
    }

    IEnumerator SpawnEnemyWavePart(WavePart wavePart, int partIndex)
    {
        for (int i = 0; i < wavePart.EnemyCount; i++)
        {
            //if (!_gameManagerScript.IsGameActive)
            if (_gameManagerScript.GameState == GameState.Loss)
                yield break;
            SpawnEnemy(wavePart.EnemyObject.Prefab, _gameManagerScript.Points[0].transform.position, wavePart.EnemyObject.Prefab.transform.rotation, wavePart.EnemyObject.BaseStats);
            yield return new WaitForSeconds(wavePart.EnemySpacing);
        }

        _allPartsFinished[partIndex] = true;
        yield break;
    }

    void SpawnEnemy(GameObject enemyPrefab, Vector2 position, Quaternion rotation, EnemyStats stats)
    {
        GameObject enemy = Instantiate(enemyPrefab, position, rotation);
        enemy.GetComponent<Enemy>().SetStats(stats);
        Enemies.Add(enemy);
    }

    private bool CheckCanSpawnNextWave()
    {
        if (_allPartsFinished.Contains(false))
        {
            return false;
        }
        return true;
    }

    public void RemoveEnemyFromList(GameObject enemy)
    {
        Enemies.Remove(enemy);
    }
}
