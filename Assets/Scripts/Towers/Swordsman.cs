using System.Linq;
using UnityEngine;

public class Swordsman : Tower
{
    protected override void Attack()
    {
        GameObject target = EnemiesInRange.First();

        SetCritical(out float damage, out DamageType damageType);
        RotateTowardsTarget(target);

        _animator.SetFloat("SpeedMultiplier", GetStatValue(TowerStat.AttackSpeed));
        _animator.SetTrigger("Attack");

        foreach (Enemy enemyScript in EnemiesInRange.Select(x => x.GetComponent<Enemy>()))
            enemyScript.TakeDamage(damage, damageType);
    }
}
