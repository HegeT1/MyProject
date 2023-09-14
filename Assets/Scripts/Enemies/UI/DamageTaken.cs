using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum DamageType
{
    Normal,
    Critical
}

public class DamageTaken : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _damageTakenText;

    public void SetText(float damage, DamageType damageType, float time)
    {
        //StartCoroutine(DestroyText(time));

        _damageTakenText.SetText(damage.ToString());
        switch (damageType)
        {
            case DamageType.Normal:
                _damageTakenText.color = Color.black;
                break;
            case DamageType.Critical:
                _damageTakenText.color = Color.yellow;
                _damageTakenText.fontSize = 50;
                break;
            default:
                break;
        }

        Destroy(gameObject, time);
    }

    private IEnumerator DestroyText(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
