using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TowerStatsWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _damage;
    [SerializeField] private TextMeshProUGUI _attackSpeed;
    [SerializeField] private TextMeshProUGUI _range;
    [SerializeField] private TextMeshProUGUI _criticalChance;
    [SerializeField] private TextMeshProUGUI _criticalChanceDamage;
    [SerializeField] private TextMeshProUGUI _targetableEnemies;

    private Vector2 _offset = new(0, 80);

    public void SetStats(Dictionary<TowerStat, float> stats)
    {
        _damage.SetText(stats.GetValueOrDefault(TowerStat.Damage).ToString());
        _attackSpeed.SetText(stats.GetValueOrDefault(TowerStat.AttackSpeed).ToString());
        _range.SetText(stats.GetValueOrDefault(TowerStat.Range).ToString());
        _criticalChance.SetText((stats.GetValueOrDefault(TowerStat.CriticalChance) * 100).ToString() + "%");
        _criticalChanceDamage.SetText(stats.GetValueOrDefault(TowerStat.CriticalDamage).ToString() + "x");
        _targetableEnemies.SetText(stats.GetValueOrDefault(TowerStat.TargetableEnemies).ToString());
    }

    public void SetPosition(Vector2 position)
    {
        position.y = 0;
        gameObject.transform.localPosition = position + _offset;
    }
}
