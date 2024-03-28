using UnityEngine;

public class Archer : Tower
{
    [SerializeField] protected ProjectileStats _projectileStats;

    protected override void SetProjectileStats()
    {
        _projectileStats = TowerScriptableObject.Projectile.BaseStats;
    }

    protected override void Attack()
    {
        for (int i = 0; i < GetStatValue(TowerStat.TargetableEnemies); i++)
        {
            GameObject target = GetTargetedEnemy(i);
            if (target != null)
            {
                SetCritical(out float damage, out DamageType damageType);

                RotateTowardsTarget(target);

                _animator.SetFloat("SpeedMultiplier", GetStatValue(TowerStat.AttackSpeed));
                _animator.SetTrigger("Attack");
                GameObject projectile = Instantiate(TowerScriptableObject.Projectile.Prefab, 
                                                    gameObject.transform.position, 
                                                    TowerScriptableObject.Projectile.Prefab.transform.rotation);
                Projectile projectileScript = projectile.GetComponent<Projectile>();
                projectileScript.SetTarget(target);

                projectileScript.SetCharacteristics(_projectileStats, TowerScriptableObject.Projectile.Type, damage, damageType);
            }
        }
    }
}
