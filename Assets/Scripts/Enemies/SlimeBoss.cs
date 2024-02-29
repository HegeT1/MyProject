using UnityEngine;

public class SlimeBoss : Enemy
{
    [Range(0f, 1f)]
    [SerializeField] private float _dodgeChance;

    public override void TakeDamage(float damage, DamageType damageType)
    {
        if (Random.value > _dodgeChance)
            base.TakeDamage(damage, damageType);
        else
            base.TakeDamage(0, damageType);
    }
}
