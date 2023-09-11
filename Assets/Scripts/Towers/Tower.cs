using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum TowerState { Idle, Fireing }

public class Tower : MonoBehaviour
{
    [SerializeField] private GameObject _towerRange;
    [SerializeField] private TowerScriptableObject _towerScriptableObject;
    [SerializeField] private TowerStats _towerStats;
    [SerializeField] private ProjectileStats _projectileStats;

    [SerializeField] private TowerState _towerState = TowerState.Idle;
    private List<GameObject> _enemiesInRange;
    private bool _isMouseOnObject;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        SetStats(_towerScriptableObject.BaseStats);
        SetProjectileStats(_towerScriptableObject.Projectile.BaseStats);
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _towerRange.transform.localScale = new(2 * _towerStats.Range, 2 * _towerStats.Range, 0);

        if(Input.GetMouseButtonDown(0) && !_isMouseOnObject)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        _enemiesInRange = GetEnemiesInRange();
        if(_enemiesInRange.Count > 0 && _towerState == TowerState.Idle)
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
            if(GetDistanceGetweenTowerEnemy(enemy) <= _towerStats.Range)
            {
                enemies.Add(enemy);
            }
        }

        return enemies;
    }

    public float GetDistanceGetweenTowerEnemy(GameObject enemy)
    {
        return Vector2.Distance(enemy.transform.position, transform.position);
    }

    IEnumerator StartShootingProjectiles()
    {
        _towerState = TowerState.Fireing;

        while(_towerState == TowerState.Fireing)
        {
            if (_enemiesInRange.Count <= 0)
            {
                _towerState = TowerState.Idle;
                yield break;
            }

            for(int i = 0; i < _towerStats.TargetableEnemies; i++)
            {
                GameObject enemy = GetTargetedEnemy(i);
                if (enemy != null)
                {
                    // Rotates the tower to face the target
                    Vector3 targetPos = enemy.transform.position;
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

                    _animator.SetFloat("SpeedMultiplier", _towerStats.AttackSpeed);
                    _animator.SetTrigger("Shoot");
                    GameObject projectile = Instantiate(_towerScriptableObject.Projectile.Prefab, gameObject.transform.position, _towerScriptableObject.Projectile.Prefab.transform.rotation, gameObject.transform);
                    Projectile projectileScript = projectile.GetComponent<Projectile>();
                    projectileScript.SetTarget(GetTargetedEnemy(i));

                    projectileScript.SetCharacteristics(_projectileStats, _towerScriptableObject.Projectile.Type, _towerStats.Damage);
                }
            }
            // Higher AttackSpeed means faster attacking
            yield return new WaitForSeconds(1 / _towerStats.AttackSpeed);
        }
    }

    private GameObject GetTargetedEnemy(int enemyPosition)
    {
        if(enemyPosition < _enemiesInRange.Count)
            return _enemiesInRange[enemyPosition];
        else
            return null;
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
