using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.EditorUtilities;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public class EnemyWavePart
    {
        public int numberOfEnemies { get; set; }
        public float spacing { get; set; }
        public float delayForNextPart { get; set; }
        public GameObject enemyPrefab { get; set; }
    }

    public class EnemyWave
    {
        public int waveNumber { get; set; }
        public List<EnemyWavePart> waveParts { get; set; } = new();
    }

    private GameManager gameManagerScript;

    [SerializeField]
    private int enemyCount;
    public GameObject startPoint;
    public GameObject enemy;
    public GameObject toughEnemy;

    private List<EnemyWave> waves = new();

    void Start()
    {
        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();

        EnemyWavePart part1 = new() { numberOfEnemies = 2, spacing = 0.5f, delayForNextPart = 0.1f, enemyPrefab = enemy };
        EnemyWavePart part2 = new() { numberOfEnemies = 5, spacing = 1f, delayForNextPart = 8f, enemyPrefab = enemy };

        EnemyWavePart part3 = new() { numberOfEnemies = 20, spacing = 0.1f, delayForNextPart = 2f, enemyPrefab = enemy };
        EnemyWavePart part4 = new() { numberOfEnemies = 5, spacing = 0.1f, delayForNextPart = 0.5f, enemyPrefab = toughEnemy };

        waves.Add(new() { waveNumber = 1, waveParts = new() { part1, part2 } });
        waves.Add(new() { waveNumber = 2, waveParts = new() { part3, part4, part1, part1 } });
    }

    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;

        if(enemyCount == 0)
        {
            if(gameManagerScript.waveNumber <= waves.Count)
            {
                gameManagerScript.UpdateWaveText(1);
                StartCoroutine(SpawnEnemyWave(gameManagerScript.waveNumber));
            }
        }
    }

    IEnumerator SpawnEnemyWave(int waveNumber)
    {
        if (waveNumber <= waves.Count)
        {
            foreach(EnemyWavePart wavePart in waves[waveNumber - 1].waveParts)
            {
                StartCoroutine(SpawnEnemyWavePart(wavePart));
                yield return new WaitForSeconds(wavePart.delayForNextPart);
            }
        }
        yield break;
    }

    IEnumerator SpawnEnemyWavePart(EnemyWavePart enemyWavePart)
    {
        for(int i = 0; i < enemyWavePart.numberOfEnemies; i++)
        {
            Instantiate(enemyWavePart.enemyPrefab, startPoint.transform.position, enemyWavePart.enemyPrefab.transform.rotation);
            yield return new WaitForSeconds(enemyWavePart.spacing);
        }

        yield break;
    }
}
