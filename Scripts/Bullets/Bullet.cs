using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    [SerializeField] private StatsEntitty _myStats;

    private float timer;
    private Vector3 direction;

    public void Fire(Vector3 dir)
    {
        direction = dir.normalized;
        timer = 0f;
        gameObject.SetActive(true);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(_myStats.Damage);
        }

        gameObject.SetActive(false);
    }
}
