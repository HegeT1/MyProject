using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditorInternal;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private GameObject _target;
    private ProjectileType _projectileType;
    private float _damageToCause;
    public ProjectileStats ProjectileStats;
    private bool _isRotationSet;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartProjectileUpTimer());
    }

    // Update is called once per frame
    void Update()
    {
        switch (_projectileType)
        {
            case ProjectileType.Normal:
                NormalProjectile();
                break;
            case ProjectileType.Homing:
                HomingProjectile();
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(_damageToCause);
            HandlePierce();
            if(ProjectileStats.Pierce <= 0)
                Destroy(gameObject);
        }
    }

    private void HandlePierce()
    {
        ProjectileStats.Pierce--;
    }

    public void SetTarget(GameObject enemy)
    {
        if (enemy == null)
            Destroy(gameObject);
        else
            _target = enemy;
    }

    public void SetCharacteristics(ProjectileStats projectileStats, ProjectileType projectileType, float damageToCause)
    {
        ProjectileStats = projectileStats;
        _projectileType = projectileType;
        _damageToCause = damageToCause;
    }

    IEnumerator StartProjectileUpTimer()
    {
        yield return new WaitForSeconds(ProjectileStats.UpTime);
        Destroy(gameObject);
    }

    private void NormalProjectile()
    {
        // Sets the rotation of the projectile to point at the target
        if(_target != null && !_isRotationSet)
        {
            Vector3 targetPos = _target.transform.position;
            targetPos.z = 0f;

            Vector3 projectilePos = transform.position;
            targetPos.x -= projectilePos.x;
            targetPos.y -= projectilePos.y;

            float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            _isRotationSet = true;
        }

        if(_isRotationSet)
        {
            transform.Translate(Time.deltaTime * ProjectileStats.Speed * Vector2.right);
        }
    }

    private void HomingProjectile()
    {
        if (_target == null)
        {
            Destroy(gameObject);
        }
        if (_target != null)
        {
            Vector2 direction = (_target.transform.position - transform.position).normalized;
            transform.Translate(Time.deltaTime * ProjectileStats.Speed * direction);
        }
    }
}
