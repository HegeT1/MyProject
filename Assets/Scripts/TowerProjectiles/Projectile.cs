using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

public class ProjectileStats
{
    public float Speed { get; set; }
    public float TimeTilAlive { get; set; }
    public float Damage { get; set; }
    public GameObject Target { get; set; }
}

public class Projectile : MonoBehaviour
{
    [field: SerializeField] public ProjectileStats Stats { get; private set; }

    //[field: SerializeField] public float Speed { get; set; } = 5;
    //[field: SerializeField] public GameObject _target { get; set; }
    //[field: SerializeField] public float TimeTilAlive { get; private set; }
    //private float _damage;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartProjectileAliveTimer());
    }

    // Update is called once per frame
    void Update()
    {
        if(Stats.Target == null)
            Destroy(gameObject);

        if (Stats.Target != null)
        {
            Vector2 direction = (Stats.Target.transform.position - transform.position).normalized;
            transform.Translate(Time.deltaTime * Stats.Speed * direction);

            transform.LookAt(Stats.Target.transform);
        }
    }

    public void SetStats(ProjectileStats stats)
    {
        Stats = stats;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(Stats.Damage);
            Destroy(gameObject);
        }
    }

    //public void SetTarget(GameObject enemy)
    //{
    //    if(enemy == null)
    //        Destroy(gameObject);
    //    else
    //        ProjectileStats.Target = enemy;
    //}

    //public void SetDamage(float damage)
    //{
    //    _damage = damage;
    //}

    IEnumerator StartProjectileAliveTimer()
    {
        yield return new WaitForSeconds(Stats.TimeTilAlive);
        Destroy(gameObject);
    }
}
