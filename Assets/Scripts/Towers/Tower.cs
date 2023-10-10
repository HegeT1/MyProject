using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;

public enum TowerPlacement { NotPlaced, Placed };
public enum TowerState { Idle, Fireing }
public enum TowerTargeting { First, Last, Strong }

public class Tower : MonoBehaviour
{
    private GameManager _gameManagerScript;

    [field: SerializeField] public TowerBaseStats _test { get; private set; }
    public List<Stat> Keys { get; private set; } = new();
    public List<float> Values { get; private set; } = new();
    private Dictionary<Stat, float> _towerStats = new();


    [SerializeField] private GameObject TowerRange;
    [field: SerializeField] public TowerScriptableObject TowerScriptableObject { get; private set; }
    private GameObject _towerWindow;
    public List<int> UgradeIndexes = new();

    [SerializeField] public TowerStats TowerStats;
    [SerializeField] private ProjectileStats _projectileStats;
    [field: SerializeField] public TowerTargeting TowerTargeting { get; private set; } = TowerTargeting.First;

    private TowerState _towerState = TowerState.Idle;
    private TowerPlacement _towerPlacement = TowerPlacement.NotPlaced;

    public List<GameObject> EnemiesInRange { get; set; }
    private bool _isMouseOnObject;
    private bool _validPlacement;
    private Animator _animator;

    public void RefreshStats()
    {
        _towerStats = new();
        for(int i = 0; i < Mathf.Min(Keys.Count, Values.Count); i++)
            _towerStats.Add(Keys[i], Values[i]);
    }

    private void Test()
    {
        Debug.Log(Values[Keys.IndexOf(Stat.Damage)]);
        //foreach (KeyValuePair<Stat, UpgradeStat> upgrade in _towerScriptableObject.UpgradePaths[0].Path[0].Upgrades)
        //{
        //    //Debug.Log("Before: " + _towerStats[upgrade.Key]);
        //    _towerStats[upgrade.Key] += upgrade.Value.Value;
            
        //    Debug.Log(Values[Keys.IndexOf(upgrade.Key)]);
        //    Keys.Clear();
        //    Values.Clear();
        //    foreach(KeyValuePair<Stat, float> kv in _towerStats)
        //    {
        //        Keys.Add(kv.Key);
        //        Values.Add(kv.Value);
        //    }
        //    Debug.Log(Values[Keys.IndexOf(upgrade.Key)]);
        //}
    }

    // Start is called before the first frame update
    void Start()
    {
        //_test = TowerScriptableObject.TowerBaseStats;
        //Keys.Clear();
        //Values.Clear();
        //for (int i = 0; i < Mathf.Min(_test.Keys.Count, _test.Values.Count); i++)
        //{
        //    Keys.Add(_test.Keys[i]);
        //    Values.Add(_test.Values[i]);
        //    _towerStats.Add(_test.Keys[i], _test.Values[i]);
        //}
        //InvokeRepeating(nameof(Test), 6, 10);


        _gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _towerWindow = GameObject.Find("Canvas").transform.Find("Main UI").Find("Panel").Find("Selected Tower").gameObject;

        SetTransperancy(0.7f);
        TowerRange.SetActive(true);

        SetStats(TowerScriptableObject.BaseStats);
        SetProjectileStats(TowerScriptableObject.Projectile.BaseStats);
        _animator = GetComponentInChildren<Animator>();

        foreach(UpgradePaths path in TowerScriptableObject.UpgradePaths)
        {
            UgradeIndexes.Add(0);
        }
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

        TowerRange.transform.localScale = new(2 * TowerStats.Range, 2 * TowerStats.Range, 0);

        if(_towerPlacement == TowerPlacement.Placed)
        {
            if (Input.GetMouseButtonDown(0) && !_isMouseOnObject)
            {
                //TowerRange.SetActive(false);
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

            _gameManagerScript.UpdateMoney(-TowerScriptableObject.Cost);

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
                SetTowerWindowStats();
                _towerWindow.GetComponent<TowerWindow>().SetupWindow(this);

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

    public void SetTowerWindowStats()
    {
        _towerWindow.transform.GetChild(0).GetComponent<TowerStatsWindow>().SetStats(TowerStats);
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
            if(GetDistanceBetweenTowerAndEnemy(enemy) <= TowerStats.Range)
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
        switch(TowerTargeting)
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

            for(int i = 0; i < TowerStats.TargetableEnemies; i++)
            {
                //SortEnemiesByDistance();
                GameObject target = GetTargetedEnemy(i);
                if (target != null)
                {
                    SetCritical(out float damage, out DamageType damageType);

                    RotateTowardsTarget(target);

                    _animator.SetFloat("SpeedMultiplier", TowerStats.AttackSpeed);
                    _animator.SetTrigger("Shoot");
                    GameObject projectile = Instantiate(TowerScriptableObject.Projectile.Prefab, gameObject.transform.position, TowerScriptableObject.Projectile.Prefab.transform.rotation);
                    Projectile projectileScript = projectile.GetComponent<Projectile>();
                    projectileScript.SetTarget(GetTargetedEnemy(i));

                    projectileScript.SetCharacteristics(_projectileStats, TowerScriptableObject.Projectile.Type, damage, damageType);
                }
            }
            // Higher AttackSpeed means faster attacking
            yield return new WaitForSeconds(1 / TowerStats.AttackSpeed);
        }
    }

    private void SetCritical(out float damage, out DamageType damageType)
    {
        damage = TowerStats.Damage;
        damageType = DamageType.Normal;

        float randomValue = Random.value;

        if (randomValue < TowerStats.CriticalChance)
        {
            damage *= TowerStats.CriticalDamage;
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
        TowerStats = towerStats;
    }

    public void SetProjectileStats(ProjectileStats projectileStats)
    {
        _projectileStats = projectileStats;
    }

    public void ChangeTargeting(TowerTargeting targeting)
    {
        TowerTargeting = targeting;
        _towerWindow.GetComponent<TowerWindow>().SetupWindow(this);
    }

    public void SellTower()
    {
        _gameManagerScript.UpdateMoney(TowerScriptableObject.Cost * _gameManagerScript.ReselValue);
        _towerWindow.SetActive(false);
        Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Road") || collision.gameObject.CompareTag("MainPanel") || collision.gameObject.CompareTag("Tower"))
        {
            _validPlacement = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Road") || collision.gameObject.CompareTag("MainPanel") || collision.gameObject.CompareTag("Tower"))
        {
            _validPlacement = false;
        }
    }
}
