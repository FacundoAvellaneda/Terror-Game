using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public GameObject bulletPrefab;
    public int poolSize = 20;

    private List<GameObject> pool = new List<GameObject>();

    void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform);
            bullet.SetActive(false);
            pool.Add(bullet);
        }
    }

    public GameObject GetBullet()
    {
        foreach (var bullet in pool)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }

        GameObject newBullet = Instantiate(bulletPrefab, transform);
        newBullet.SetActive(false);
        pool.Add(newBullet);
        return newBullet;
    }
}
