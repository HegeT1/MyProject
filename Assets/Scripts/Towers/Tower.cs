using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum TowerState { Idle, Fireing }

public class Tower : MonoBehaviour
{
    [field: SerializeField] public float Radius { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }

    [field: SerializeField] public List<GameObject> EnemiesInRange { get; private set; }
    public int _targetableEnemies = 1;

    public GameObject Projectile;
    private bool _isMouseOnObject;
    [field: SerializeField] public TowerState TowerState { get; private set; } = TowerState.Idle;

    // Start is called before the first frame update
    void Start()
    {
        
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
        gameObject.transform.GetChild(0).gameObject.transform.localScale = new(2 * Radius, 2 * Radius, 0);
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
            if(GetDistanceGetweenTowerEnemy(enemy) <= Radius)
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

            for(int i = 0; i < _targetableEnemies; i++)
            {
                GameObject enemy = GetTargetedEnemy(i);
                if (enemy != null)
                {
                    GameObject projectile = Instantiate(Projectile, gameObject.transform.position, Projectile.transform.rotation, gameObject.transform);
                    projectile.GetComponent<Projectile>().SetTarget(GetTargetedEnemy(i));
                }
            }
            yield return new WaitForSeconds(Speed);
        }
    }

    private GameObject GetTargetedEnemy(int enemyPosition)
    {
        if(enemyPosition < EnemiesInRange.Count)
            return EnemiesInRange[enemyPosition];
        else
            return null;
    }
}
