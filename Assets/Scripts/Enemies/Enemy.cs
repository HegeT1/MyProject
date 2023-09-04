using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private GameManager _gameManagerScript;
    private SpawnManager _spawnManagerScript;

    private int _currentPointIndex = 0;

    [SerializeField] private EnemyStats _stats;
    [field: SerializeField] public float CurrentHealthPoints { get; private set; }

    private HealthBar _healthBarScritp;

    void Start()
    {
        _gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _healthBarScritp = GetComponentInChildren<HealthBar>();

        transform.position = _gameManagerScript.Points[_currentPointIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentPointIndex < _gameManagerScript.Points.Count)
        {
            transform.position = Vector2.MoveTowards(transform.position, _gameManagerScript.Points[_currentPointIndex].transform.position, _stats.MoveSpeed * Time.deltaTime);

            if (transform.position == _gameManagerScript.Points[_currentPointIndex].transform.position)
            {
                _currentPointIndex++;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("End"))
        {
            _gameManagerScript.UpdatePlayerHealth(-_stats.DamageToPlayer);
            _spawnManagerScript.RemoveEnemyFromList(gameObject);
            Destroy(gameObject);
        }
    }

    public void SetStats(EnemyStats stats)
    {
        _stats = stats;
        CurrentHealthPoints = _stats.MaxHealthPoints;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealthPoints -= damage;
        if(CurrentHealthPoints <= 0)
        {
            CurrentHealthPoints = 0;
            _healthBarScritp.UpdateHealthBar(CurrentHealthPoints, _stats.MaxHealthPoints);
            GameObject.Find("Spawn Manager").GetComponent<SpawnManager>().RemoveEnemyFromList(gameObject);
            Destroy(gameObject);
        }
        else
        {
            _healthBarScritp.UpdateHealthBar(CurrentHealthPoints, _stats.MaxHealthPoints);
        }
    }
}
