using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditorInternal;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private SpawnManager _spawnManagerScript;

    [SerializeField] private GameObject _target;
    private ProjectileType _projectileType;
    public ProjectileStats ProjectileStats;
    private DamageType _damageType;
    private float _damageToCause;
    private bool _isRotationSet;
    private bool _isAtMaxPierce; // To prevent the projectile hitting multiple targets if they are on top of eachother
    [SerializeField] private bool _targetHit;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManagerScript = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        StartCoroutine(StartProjectileUpTimer());
    }

    // Update is called once per frame
    void Update()
    {
        switch (_projectileType)
        {
            case ProjectileType.Linear:
                LinearProjectile();
                break;
            case ProjectileType.Homing:
                HomingProjectile();
                break;
            case ProjectileType.LinearBounce:
                LinearBounceProjectile();
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !_isAtMaxPierce)
        {
            _targetHit = true;
            HandlePierce();
            if(ProjectileStats.Pierce <= 0)
            {
                _isAtMaxPierce = true;
                Destroy(gameObject);
            }

            if (_projectileType == ProjectileType.LinearBounce)
            {
                GetNextTarget(collision.gameObject);
            }

            collision.gameObject.GetComponent<Enemy>().TakeDamage(_damageToCause, _damageType);
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

    public void SetCharacteristics(ProjectileStats projectileStats, ProjectileType projectileType, float damageToCause, DamageType damageType)
    {
        ProjectileStats = projectileStats;
        _projectileType = projectileType;
        _damageToCause = damageToCause;
        _damageType = damageType;
    }

    IEnumerator StartProjectileUpTimer()
    {
        yield return new WaitForSeconds(ProjectileStats.UpTime);
        Destroy(gameObject);
    }

    private void LookAtTarget()
    {
        // Sets the rotation of the projectile to point at the target
        if (_target != null)
        {
            Vector3 targetPos = _target.transform.position;
            targetPos.z = 0f;

            Vector3 projectilePos = transform.position;
            targetPos.x -= projectilePos.x;
            targetPos.y -= projectilePos.y;

            float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }

    private void MoveProjectile()
    {
        transform.Translate(Time.deltaTime * ProjectileStats.Speed * Vector2.right);
    }

    private void LinearProjectile()
    {
        if(!_isRotationSet)
            LookAtTarget();
        _isRotationSet = true;
        
        MoveProjectile();
    }

    private void HomingProjectile()
    {
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        LookAtTarget();
        MoveProjectile();
    }

    private void LinearBounceProjectile()
    {
        if (!_targetHit)
            LinearProjectile();
        else
            HomingProjectile();
    }

    private void GetNextTarget(GameObject currentTargetHit)
    {
        int targetIndex = _spawnManagerScript.Enemies.IndexOf(currentTargetHit);

        // Set next target to behind the initial one
        int nextTargetIndex = targetIndex + 1;

        // Sets next target to first if there are no targets behind the initial one
        if (nextTargetIndex >= _spawnManagerScript.Enemies.Count)
            nextTargetIndex = targetIndex - 1;

        if(targetIndex == - 1 || nextTargetIndex >= _spawnManagerScript.Enemies.Count || nextTargetIndex == - 1)
        {
            _target = null;
            return;
        }

        _target = _spawnManagerScript.Enemies[nextTargetIndex];
    }
}
