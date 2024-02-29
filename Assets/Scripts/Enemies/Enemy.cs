using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    private GameManager _gameManagerScript;
    protected SpawnManager _spawnManagerScript;

    private int _currentPointIndex = 0;

    [field: SerializeField] public EnemyStats Stats { get; private set; }
    [SerializeField] private float _currentHealthPoints;

    [SerializeField] private Slider _healthBarSlider;
    private HealthBar _healthBarScritp;
    [SerializeField] private GameObject _damageTakenText;
    private Animator _animator;
    protected bool _canMove;

    private Vector2 _previousPosition;
    [field: SerializeField] public float DistanceTravelled { get; private set; }

    void Start()
    {
        _gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _healthBarScritp = GetComponentInChildren<HealthBar>();
        _animator = GetComponentInChildren<Animator>();
        
        _canMove = true;
        transform.position = _gameManagerScript.Points[_currentPointIndex].transform.position;
    }

    void Update()
    {
        CalculateDistance();
        if (_currentPointIndex < _gameManagerScript.Points.Count && _canMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, _gameManagerScript.Points[_currentPointIndex].transform.position, Stats.MoveSpeed * Time.deltaTime);

            if (transform.position == _gameManagerScript.Points[_currentPointIndex].transform.position)
            {
                _currentPointIndex++;
            }
        }
    }

    void CalculateDistance()
    {
        if (_previousPosition != Vector2.zero)
        {
            float distance = Vector2.Distance(transform.position, _previousPosition);
            DistanceTravelled += distance;
        }
        _previousPosition = transform.position;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("End"))
        {
            _gameManagerScript.UpdatePlayerHealth(-Stats.DamageToPlayer);
            _spawnManagerScript.RemoveEnemyFromList(gameObject);
            Destroy(gameObject);
        }
    }

    public void SetStats(EnemyStats stats)
    {
        Stats = stats;
        _currentHealthPoints = Stats.MaxHealthPoints;
    }

    public virtual void TakeDamage(float damage, DamageType damageType)
    {
        _currentHealthPoints -= damage;
        _healthBarScritp.UpdateHealthBar(_currentHealthPoints, Stats.MaxHealthPoints);
        ShowDamageTaken(damage, damageType);
        if (_currentHealthPoints <= 0)
        {
            GetComponent<Collider2D>().enabled = false;
            _healthBarSlider.gameObject.SetActive(false);
            _canMove = false;
            _animator.SetTrigger("Dead");
            _spawnManagerScript.RemoveEnemyFromList(gameObject);
            _gameManagerScript.UpdateMoney(Stats.MoneyWorth);

            DestroyEnemy();
        }
        else
        {
            _animator.SetTrigger("TakeDamage");
        }
    }

    public virtual void DestroyEnemy()
    {
        Destroy(gameObject, 0.5f);
    }

    void ShowDamageTaken(float damage, DamageType damageType)
    {
        // Make the displayed damage a random position
        float xOffset = Random.Range(0.2f, 0.5f);
        float yOffset = Random.Range(0.2f, 0.5f);
        Vector2 offset = new(xOffset, yOffset);
        
        GameObject damageText = Instantiate(_damageTakenText, (Vector2)transform.position + offset, _damageTakenText.transform.rotation);
        damageText.GetComponent<DamageTaken>().SetText(damage, damageType, 0.4f);
    }
}
