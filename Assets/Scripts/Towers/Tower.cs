using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum TowerState { Idle, Fireing }

public class Tower : MonoBehaviour
{
    [SerializeField] private TowerScriptableObject TowerScriptableObject;
    [SerializeField] private TowerStats TowerStats;
    [SerializeField] private ProjectileStats ProjectileStats;

    [SerializeField] private TowerState TowerState = TowerState.Idle;
    private List<GameObject> EnemiesInRange;
    private bool _isMouseOnObject;

    // Start is called before the first frame update
    void Start()
    {
        SetStats(TowerScriptableObject.BaseStats);
        SetProjectileStats(TowerScriptableObject.Projectile.BaseStats);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !_isMouseOnObject)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

        EnemiesInRange = GetEnemiesInRange();
        if(EnemiesInRange.Count > 0 && TowerState == TowerState.Idle)
        {
            StartCoroutine(StartShootingProjectiles());
        }
        gameObject.transform.GetChild(0).gameObject.transform.localScale = new(2 * TowerStats.Range, 2 * TowerStats.Range, 0);
    }

    private void OnMouseDown()
    {
        GameObject towerRadius = gameObject.transform.GetChild(0).gameObject;
        if(towerRadius.activeSelf)
        {
            towerRadius.SetActive(false);
        }
        else
        {
            towerRadius.SetActive(true);
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
            if(GetDistanceGetweenTowerEnemy(enemy) <= TowerStats.Range)
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
        TowerState = TowerState.Fireing;

        while(TowerState == TowerState.Fireing)
        {
            if (EnemiesInRange.Count <= 0)
            {
                TowerState = TowerState.Idle;
                yield break;
            }

            for(int i = 0; i < TowerStats.TargetableEnemies; i++)
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
                    Debug.Log(angle);
                    if (angle < 90)
                        angle = 0;
                    else
                        angle = 180;

                    transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));

                    GameObject projectile = Instantiate(TowerScriptableObject.Projectile.Prefab, gameObject.transform.position, TowerScriptableObject.Projectile.Prefab.transform.rotation, gameObject.transform);
                    Projectile projectileScript = projectile.GetComponent<Projectile>();
                    projectileScript.SetTarget(GetTargetedEnemy(i));

                    projectileScript.SetCharacteristics(ProjectileStats, TowerScriptableObject.Projectile.Type, TowerStats.Damage);
                }
            }
            // Higher AttackSpeed means faster attacking
            yield return new WaitForSeconds(1 / TowerStats.AttackSpeed);
        }
    }

    private GameObject GetTargetedEnemy(int enemyPosition)
    {
        if(enemyPosition < EnemiesInRange.Count)
            return EnemiesInRange[enemyPosition];
        else
            return null;
    }

    public void SetStats(TowerStats towerStats)
    {
        TowerStats = towerStats;
    }

    public void SetProjectileStats(ProjectileStats projectileStats)
    {
        ProjectileStats = projectileStats;
    }
}
