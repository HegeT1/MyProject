using System.Collections;
using UnityEngine;

public class Orc : Enemy
{
    public override void TakeDamage(float damage, DamageType damageType)
    {
        StartCoroutine(Stun(0.2f));
        base.TakeDamage(damage / 2, damageType);
    }

    IEnumerator Stun(float seconds)
    {
        _canMove = false;
        yield return new WaitForSeconds(seconds);
        _canMove = true;
    }
}
