using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum TowerState { Idle, Fireing }
public enum TowerTargeting { First, Last, Strong }

public class Tower : MonoBehaviour
{
    [SerializeField] private GameObject _towerRange;
    [SerializeField] private TowerScriptableObject _towerScriptableObject;
    [SerializeField] private TowerStats _towerStats;
    [SerializeField] private ProjectileStats _projectileStats;
    [SerializeField] private TowerTargeting _towerTargeting = TowerTargeting.First;

    private TowerState _towerState = TowerState.Idle;
    [field: SerializeField] public List<GameObject> EnemiesInRange { get; set; }
    [SerializeField] private bool _isMouseOnObject;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        SetStats(_towerScriptableObject.BaseStats);
        SetProjectileStats(_towerScriptableObject.Projectile.BaseStats);
        _animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _towerRange.transform.localScale = new(2 * _towerStats.Range, 2 * _towerStats.Range, 0);

        if (Input.GetMouseButtonDown(0) && !_isMouseOnObject)
        {
            _towerRange.SetActive(false);
        }

        EnemiesInRange = GetEnemiesInRange();
        SortEnemiesByTargeting();
        if (EnemiesInRange.Count > 0 && _towerState == TowerState.Idle)
        {
            StartCoroutine(StartShootingProjectiles());
        }
    }

    private void OnMouseDown()
    {
        if(_towerRange.activeSelf)
        {
            _towerRange.SetActive(false);
        }
        else
        {
            _towerRange.SetActive(true);
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
}
