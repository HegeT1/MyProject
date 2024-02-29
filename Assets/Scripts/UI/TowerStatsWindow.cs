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

    public void SetStats(TowerStats stats)
    {
        _damage.SetText(stats.Damage.ToString());
        _attackSpeed.SetText(stats.AttackSpeed.ToString());
        _range.SetText(stats.Range.ToString());
        _criticalChance.SetText((stats.CriticalChance * 100).ToString() + "%");
        _criticalChanceDamage.SetText(stats.CriticalDamage.ToString() + "x");
        _targetableEnemies.SetText(stats.TargetableEnemies.ToString());
    }

    public void SetPosition(Vector2 position)
    {
        position.y = 0;
        gameObject.transform.localPosition = position + _offset;
    }
}
