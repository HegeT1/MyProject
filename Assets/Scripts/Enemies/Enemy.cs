using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private GameManager _gameManagerScript;
    private SpawnManager _spawnManagerScript;

    private int _currentPointIndex = 0;

    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float MaxHealthPoints { get; private set; }
    [field: SerializeField] public float CurrentHealthPoints { get; private set; }
    [field: SerializeField] public int DamageToPlayer { get; private set; }
    private HealthBar _healthBarScritp;

    [SerializeField] private EnemyScriptableObject _stats;

    void Start()
    {
        CurrentHealthPoints = MaxHealthPoints;
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
            transform.position = Vector2.MoveTowards(transform.position, _gameManagerScript.Points[_currentPointIndex].transform.position, MoveSpeed * Time.deltaTime);

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
            _gameManagerScript.UpdatePlayerHealth(-DamageToPlayer);
            _spawnManagerScript.RemoveEnemyFromList(gameObject);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        CurrentHealthPoints -= damage;
        if(CurrentHealthPoints <= 0)
        {
            CurrentHealthPoints = 0;
            _healthBarScritp.UpdateHealthBar(CurrentHealthPoints, MaxHealthPoints);
            GameObject.Find("Spawn Manager").GetComponent<SpawnManager>().RemoveEnemyFromList(gameObject);
            Destroy(gameObject);
        }
        else
        {
            _healthBarScritp.UpdateHealthBar(CurrentHealthPoints, MaxHealthPoints);
        }
    }
}
