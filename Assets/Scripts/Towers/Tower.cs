using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum TowerPlacement { NotPlaced, Placed };
public enum TowerState { Idle, Fireing }
public enum TowerTargeting { First, Last, Strong }

public class Tower : MonoBehaviour
{
    private GameManager _gameManagerScript;

    [SerializeField] private GameObject TowerRange;
    [SerializeField] private TowerScriptableObject _towerScriptableObject;
    [SerializeField] private GameObject _towerWindow;

    [SerializeField] private TowerStats _towerStats;
    [SerializeField] private ProjectileStats _projectileStats;
    [SerializeField] private TowerTargeting _towerTargeting = TowerTargeting.First;

    public TowerState _towerState = TowerState.Idle;
    public TowerPlacement _towerPlacement = TowerPlacement.NotPlaced;

    [field: SerializeField] public List<GameObject> EnemiesInRange { get; set; }
    [SerializeField] private bool _isMouseOnObject;
    [SerializeField] private bool _validPlacement;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _towerWindow = GameObject.Find("Canvas").transform.Find("Main UI").Find("Panel").Find("Selected Tower").gameObject;

        SetTransperancy(0.7f);
        TowerRange.SetActive(true);

        SetStats(_towerScriptableObject.BaseStats);
        SetProjectileStats(_towerScriptableObject.Projectile.BaseStats);
        _animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_towerPlacement == TowerPlacement.NotPlaced)
        {
            if(Input.GetMouseButton(1) || Input.GetKeyDown(KeyCode.Escape))
                Destroy(gameObject);

            if (_validPlacement)
            {
                SetPlacementColor(Color.black);
            }
            else
            {
                SetPlacementColor(Color.red);
            }

            FollowMouse();
        }

        TowerRange.transform.localScale = new(2 * _towerStats.Range, 2 * _towerStats.Range, 0);

        if(_towerPlacement == TowerPlacement.Placed)
        {
            if (Input.GetMouseButtonDown(0) && !_isMouseOnObject)
            {
                TowerRange.SetActive(false);
            }

            _towerWindow.SetActive(AnySelectedTowers());

            EnemiesInRange = GetEnemiesInRange();
            SortEnemiesByTargeting();
            if (EnemiesInRange.Count > 0 && _towerState == TowerState.Idle)
            {
                StartCoroutine(StartShootingProjectiles());
            }
        }
    }

    private bool AnySelectedTowers()
    {
        if (FindObjectsOfType<Tower>().Where(x => x._towerPlacement == TowerPlacement.Placed).Any(x => x.TowerRange.activeSelf))
             return true;
        return false;
    }

    private void FollowMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
        mousePosition.z = -9;
        transform.position = mousePosition;
    }

    private void SetTransperancy(float multiplier)
    {
        Color towerRangeColor = TowerRange.GetComponent<SpriteRenderer>().color;
        towerRangeColor.a *= multiplier;
        TowerRange.GetComponent<SpriteRenderer>().color = towerRangeColor;

        Color spriteColor = transform.GetChild(0).GetComponent<SpriteRenderer>().color;
        spriteColor.a *= multiplier;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = spriteColor;
    }

    private void SetPlacementColor(Color color)
    {
        color.a = 0.4f;
        TowerRange.GetComponent<SpriteRenderer>().color = color;
    }

    public bool PlaceTower()
    {
        if(_validPlacement)
        {
            _towerPlacement = TowerPlacement.Placed;
            TowerRange.SetActive(false);
            SetTransperancy(1.7f);

            _gameManagerScript.UpdateMoney(-_towerScriptableObject.Cost);

            return true;
        }
        return false;
    }

    private void OnMouseDown()
    {
        if (_towerPlacement == TowerPlacement.Placed)
        {
            if (TowerRange.activeSelf)
            {
                TowerRange.SetActive(false);
                _towerWindow.SetActive(false);
            }
            else
            {
                _towerWindow.transform.GetChild(0).GetComponent<TowerStatsWindow>().SetStats(_towerStats);
                TowerRange.SetActive(true);
                _towerWindow.SetActive(true);
            }
        }

        if(_towerPlacement == TowerPlacement.NotPlaced)
        {
            if(PlaceTower())
                GameObject.Find("Shop").GetComponent<ShopManager>().Tower = null;
        }
    }

    private void OnMouseEnter()
    {
        _isMouseOnObject = true;
    }

    private void OnMouseExit()
    {
        _isMouseOnObject = false;
    }

    public List<GameObject> GetEnemiesInRange()
    {
        List<GameObject> enemies = new();
        foreach(GameObject enemy in GameObject.Find("Spawn Manager").GetComponent<SpawnManager>().Enemies)
        {
            if(GetDistanceBetweenTowerAndEnemy(enemy) <= _towerStats.Range)
            {
                enemies.Add(enemy);
            }
        }

        return enemies;
    }

    public float GetDistanceBetweenTowerAndEnemy(GameObject enemy)
    {
        return Vector2.Distance(enemy.transform.position, transform.position);
    }

    private void SortEnemiesByTargeting() 
    {
        switch(_towerTargeting)
        {
            case TowerTargeting.First:
                EnemiesInRange = EnemiesInRange.OrderByDescending(x => x.GetComponent<Enemy>().DistanceTravelled).ToList();
                break;
            case TowerTargeting.Last:
                EnemiesInRange = EnemiesInRange.OrderBy(x => x.GetComponent<Enemy>().DistanceTravelled).ToList();
                break;
            case TowerTargeting.Strong:
                EnemiesInRange = EnemiesInRange.OrderByDescending(x => x.GetComponent<Enemy>().Stats.Strength).ToList();
                break;
            default:
                break;
        }
    }

    IEnumerator StartShootingProjectiles()
    {
        _towerState = TowerState.Fireing;

        while(_towerState == TowerState.Fireing)
        {
            if (EnemiesInRange.Count <= 0)
            {
                _towerState = TowerState.Idle;
                yield break;
            }

            for(int i = 0; i < _towerStats.TargetableEnemies; i++)
            {
                //SortEnemiesByDistance();
                GameObject target = GetTargetedEnemy(i);
                if (target != null)
                {
                    SetCritical(out float damage, out DamageType damageType);

                    RotateTowardsTarget(target);

                    _animator.SetFloat("SpeedMultiplier", _towerStats.AttackSpeed);
                    _animator.SetTrigger("Shoot");
                    GameObject projectile = Instantiate(_towerScriptableObject.Projectile.Prefab, gameObject.transform.position, _towerScriptableObject.Projectile.Prefab.transform.rotation);
                    Projectile projectileScript = projectile.GetComponent<Projectile>();
                    projectileScript.SetTarget(GetTargetedEnemy(i));

                    projectileScript.SetCharacteristics(_projectileStats, _towerScriptableObject.Projectile.Type, damage, damageType);
                }
            }
            // Higher AttackSpeed means faster attacking
            yield return new WaitForSeconds(1 / _towerStats.AttackSpeed);
        }
    }

    private void SetCritical(out float damage, out DamageType damageType)
    {
        damage = _towerStats.Damage;
        damageType = DamageType.Normal;

        float randomValue = Random.value;

        if (randomValue < _towerStats.CriticalChance)
        {
            damage *= _towerStats.CriticalDamage;
            damageType = DamageType.Critical;
        }
    }

    private GameObject GetTargetedEnemy(int enemyPosition)
    {
        if(enemyPosition < EnemiesInRange.Count)
            return EnemiesInRange[enemyPosition];
        else
            return null;
    }

    private void RotateTowardsTarget(GameObject target)
    {
        // Rotates the tower to face the target
        Vector3 targetPos = target.transform.position;
        targetPos.z = 0f;

        Vector3 towerPos = transform.position;
        targetPos.x -= towerPos.x;
        targetPos.y -= towerPos.y;

        float angle = Mathf.Abs(Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg);
        if (angle < 90)
            angle = 0;
        else
            angle = 180;

        transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
    }

    public void SetStats(TowerStats towerStats)
    {
        _towerStats = towerStats;
    }

    public void SetProjectileStats(ProjectileStats projectileStats)
    {
        _projectileStats = projectileStats;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Road") || collision.gameObject.CompareTag("MainPanel"))
        {
            _validPlacement = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Road") || collision.gameObject.CompareTag("MainPanel"))
        {
            _validPlacement = true;
        }
    }
}
