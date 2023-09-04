using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [field: SerializeField] public float Speed { get; set; } = 5;
    [field: SerializeField] public GameObject _target { get; set; }
    [field: SerializeField] public float TimeTilAlive { get; private set; }
    private float _damage = 2;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartProjectileAliveTimer());
    }

    // Update is called once per frame
    void Update()
    {
        if(_target == null)
            Destroy(gameObject);

        if (_target != null)
        {
            Vector2 direction = (_target.transform.position - transform.position).normalized;
            transform.Translate(Time.deltaTime * Speed * direction);

            transform.LookAt(_target.transform);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(_damage);
            Destroy(gameObject);
        }
    }

    public void SetTarget(GameObject enemy)
    {
        if (enemy == null)
            Destroy(gameObject);
        else
            _target = enemy;
    }

    IEnumerator StartProjectileAliveTimer()
    {
        yield return new WaitForSeconds(TimeTilAlive);
        Destroy(gameObject);
    }
}
