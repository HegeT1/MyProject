using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [SerializeField] private Slider _healthBarSlider;
    private HealthBar _healthBarScritp;
    [SerializeField] private GameObject _damageTakenText;
    private Animator _animator;
    private bool _canMove;

    void Start()
    {
        _gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _healthBarScritp = GetComponentInChildren<HealthBar>();
        _animator = GetComponentInChildren<Animator>();
        
        _canMove = true;
        transform.position = _gameManagerScript.Points[_currentPointIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentPointIndex < _gameManagerScript.Points.Count && _canMove)
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
        ShowDamageTaken(damage);
        _healthBarScritp.UpdateHealthBar(CurrentHealthPoints, _stats.MaxHealthPoints);
        if (CurrentHealthPoints <= 0)
        {
            _healthBarSlider.gameObject.SetActive(false);
            _canMove = false;
            _animator.SetTrigger("Dead");
            CurrentHealthPoints = 0;
            GameObject.Find("Spawn Manager").GetComponent<SpawnManager>().RemoveEnemyFromList(gameObject);
            Destroy(gameObject, 0.5f);
        }
        else
        {
            _animator.SetTrigger("TakeDamage");
        }
    }

    private void ShowDamageTaken(float damage)
    {
        Vector3 offset = new(0.3f, 0.3f, 0);
        GameObject damageText = Instantiate(_damageTakenText, transform.position + offset, _damageTakenText.transform.rotation);
        damageText.GetComponent<DamageTaken>().SetText(damage, DamageType.Normal, 0.4f);
    }
}
