using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TowerPlacement { NotPlaced, Placed };
public enum TowerState { Idle, Attacking }
public enum TowerTargeting { First, Last, Strong }

public class Tower : MonoBehaviour
{
    private GameManager _gameManagerScript;

    private TowerBaseStatsScriptableObject _towerBaseStats;
    public List<TowerStat> Keys { get; private set; } = new();
    public List<float> Values { get; private set; } = new();
    private Dictionary<TowerStat, float> _towerStats = new();

    [field: SerializeField] public TowerScriptableObject TowerScriptableObject { get; private set; }
    [field: SerializeField] public GameObject TowerRange { get; private set; }
    private GameObject _towerWindow;
    public List<int> UpgradeIndexes { get; private set; } = new();

    [field: SerializeField] public TowerTargeting TowerTargeting { get; private set; } = TowerTargeting.First;

    private TowerState _towerState = TowerState.Idle;
    private TowerPlacement _towerPlacement = TowerPlacement.NotPlaced;
    private float _towerValue;

    public List<GameObject> EnemiesInRange { get; set; }
    //private bool _isMouseOnObject;
    private bool _validPlacement;
    protected Animator _animator;

    public void RefreshKeysValues()
    {
        _towerStats = new();
        for(int i = 0; i < Mathf.Min(Keys.Count, Values.Count); i++)
            _towerStats.Add(Keys[i], Values[i]);
    }

    private void UpdateTowerStats()
    {
        Keys.Clear();
        Values.Clear();
        foreach(KeyValuePair<TowerStat, float> keyValue in _towerStats)
        {
            Keys.Add(keyValue.Key);
            Values.Add(keyValue.Value);
        }
    }

    private void SetInitialStats()
    {
        _towerBaseStats = TowerScriptableObject.TowerBaseStats;
        Keys.Clear();
        Values.Clear();
        for (int i = 0; i < Mathf.Min(_towerBaseStats.Keys.Count, _towerBaseStats.Values.Count); i++)
        {
            Keys.Add(_towerBaseStats.Keys[i]);
            Values.Add(_towerBaseStats.Values[i]);
            _towerStats.Add(_towerBaseStats.Keys[i], _towerBaseStats.Values[i]);
        }
    }

    void Start()
    {
        _gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _towerWindow = GameObject.Find("Canvas").transform.Find("Main UI").Find("Bottom Panel").Find("Selected Tower").gameObject;

        SetInitialStats();

        SetTransperancy(0.7f);
        TowerRange.SetActive(true);

        SetProjectileStats();
        _animator = GetComponentInChildren<Animator>();

        UpgradeIndexes.AddRange(TowerScriptableObject.UpgradePaths.Select(path => 0));

        _towerValue += TowerScriptableObject.Cost;
    }

    void Update()
    {
        TowerRange.transform.localScale = new(2 * GetStatValue(TowerStat.Range), 2 * GetStatValue(TowerStat.Range), 0);

        if (_towerPlacement == TowerPlacement.NotPlaced)
        {
            if(Input.GetMouseButton(1) || Input.GetKeyDown(KeyCode.Escape))
                Destroy(gameObject);

            if (_validPlacement)
                SetPlacementColor(Color.black);
            else
                SetPlacementColor(Color.red);

            FollowMouse();
        }
        else if (_towerPlacement == TowerPlacement.Placed)
        {
            EnemiesInRange = GetEnemiesInRange();
            SortEnemiesByTargeting();
            if (EnemiesInRange.Count > 0 && _towerState == TowerState.Idle)
            {
                StartCoroutine(StartAttacking());
            }
        }
    }

    protected virtual void SetProjectileStats()
    {

    }

    protected float GetStatValue(TowerStat stat)
    {
        if (_towerStats.ContainsKey(stat))
            return _towerStats[stat];
        return 0;
    }

    private void FollowMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
                _towerWindow.SetActive(true);
            }
        }
        else if(_towerPlacement == TowerPlacement.NotPlaced)
        {
            if(PlaceTower())
                GameObject.Find("Shop").GetComponent<ShopManager>().Tower = null;
        }
    }

    public void SetTowerWindowStats()
    {
        _towerWindow.transform.GetChild(0).GetComponent<TowerStatsWindow>().SetStats(_towerStats);
    }

    //private void OnMouseEnter()
    //{
    //    _isMouseOnObject = true;
    //}

    //private void OnMouseExit()
    //{
    //    _isMouseOnObject = false;
    //}

    public List<GameObject> GetEnemiesInRange()
    {
        List<GameObject> enemies = new();
        foreach(GameObject enemy in GameObject.Find("Spawn Manager").GetComponent<SpawnManager>().Enemies)
        {
            if(GetDistanceBetweenTowerAndEnemy(enemy) <= GetStatValue(TowerStat.Range))
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

    private IEnumerator StartAttacking()
    {
        _towerState = TowerState.Attacking;

        while(_towerState == TowerState.Attacking)
        {
            if (EnemiesInRange.Count <= 0)
            {
                _towerState = TowerState.Idle;
                yield break;
            }

            Attack();
            // Higher AttackSpeed means faster attacking
            yield return new WaitForSeconds(1 / GetStatValue(TowerStat.AttackSpeed));
        }
    }

    protected virtual void Attack()
    {
        
    }

    protected void SetCritical(out float damage, out DamageType damageType)
    {
        damage = GetStatValue(TowerStat.Damage);
        damageType = DamageType.Normal;

        float randomValue = Random.value;

        if (randomValue < GetStatValue(TowerStat.CriticalChance))
        {
            damage *= GetStatValue(TowerStat.CriticalDamage);
            damageType = DamageType.Critical;
        }
    }

    protected GameObject GetTargetedEnemy(int enemyPosition)
    {
        if(enemyPosition < EnemiesInRange.Count)
            return EnemiesInRange[enemyPosition];
        else
            return null;
    }

    protected void RotateTowardsTarget(GameObject target)
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

    public void ChangeTargeting(TowerTargeting targeting)
    {
        TowerTargeting = targeting;
        _towerWindow.GetComponent<TowerWindow>().SetupWindow(this);
    }

    public void SellTower()
    {
        _gameManagerScript.UpdateMoney(Mathf.Round(_towerValue * _gameManagerScript.ReselPercent));
        _towerWindow.SetActive(false);
        Destroy(gameObject);
    }

    public void UpgradeTower(int pathIndex)
    {
        if (pathIndex < TowerScriptableObject.UpgradePaths.Count && UpgradeIndexes[pathIndex] < TowerScriptableObject.UpgradePaths[pathIndex].Path.Count)
        {
            UpgradeScriptableObject upgrade = TowerScriptableObject.UpgradePaths[pathIndex].Path[UpgradeIndexes[pathIndex]];
            if (upgrade.Cost <= _gameManagerScript.Money)
            {
                DoUpgrade(upgrade);
                _gameManagerScript.UpdateMoney(-upgrade.Cost);
                _towerValue += upgrade.Cost;
                UpgradeIndexes[pathIndex]++;
                _towerWindow.GetComponent<TowerWindow>().SetupWindow(this);
            }
        }
    }

    private void DoUpgrade(UpgradeScriptableObject upgrade)
    {
        foreach (KeyValuePair<TowerStat, UpgradeStat> keyValue in upgrade.Upgrades)
        {
            if (TowerScriptableObject.TowerBaseStats.GetStatValue(keyValue.Key) == null) continue;

            float? upgradeValue;
            if(keyValue.Value.IsPercent)
            {
                if (keyValue.Value.IsBaseValue)
                    upgradeValue = TowerScriptableObject.TowerBaseStats.GetStatValue(keyValue.Key);
                else
                    upgradeValue = _towerStats[keyValue.Key];
                upgradeValue *= keyValue.Value.Value;
            }
            else
                upgradeValue = keyValue.Value.Value;

            switch(keyValue.Value.Arithemtic)
            {
                case Arithemtic.Add:
                    _towerStats[keyValue.Key] += upgradeValue.Value;
                    break;
                case Arithemtic.Subtract:
                    _towerStats[keyValue.Key] -= upgradeValue.Value;
                    break;
                case Arithemtic.Multiply:
                    _towerStats[keyValue.Key] *= upgradeValue.Value;
                    break;
                case Arithemtic.Divide:
                    _towerStats[keyValue.Key] /= upgradeValue.Value;
                    break;
                default:
                    break;
            }
        }

        SetTowerWindowStats();

        //UpdateTowerStats();
    }

    public int GetUpgradePathPosition(int upgradePathIndex)
    {
        if (upgradePathIndex < UpgradeIndexes.Count)
            return UpgradeIndexes[upgradePathIndex];
        return 0;
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
