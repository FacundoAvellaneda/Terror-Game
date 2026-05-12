using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [Header("References")]
    public BulletPool bulletPool;
    public Transform shootPoint;
    public Animator animator;
    public LineRenderer aimLine;
    [SerializeField] private GameObject muzzleFlash;

    [Header("Settings")]
    public float shootCooldown = 0.5f;

    private bool isAiming;
    private bool isShooting;
    private bool canShoot = false;

    private PlayerMovement movement;

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        aimLine.enabled = false;
    }

    void Update()
    {
        aimLine.enabled = isAiming;

        Debug.Log(isAiming);
        HandleAim();
    }

    public void OnAim(InputValue value)
    {
        isAiming = value.isPressed;
        if (isAiming)
        {
            canShoot = true;
        }
    }

    void HandleAim()
    {
        if (!isAiming) return;

        Vector3 start = shootPoint.position;
        Vector3 dir = transform.forward;

        aimLine.SetPosition(0, start);
        aimLine.SetPosition(1, start + dir * 10f);
    }

    public void OnShoot(InputValue value)
    {
        if (!value.isPressed) return;
        if (!canShoot || isShooting || !isAiming) return;

        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(true);
        }

        StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        isShooting = true;
        canShoot = false;

        movement.enabled = false;

        animator.SetTrigger("Shoot");

        yield return new WaitForSeconds(0.1f);

        FireBullet();

        yield return new WaitForSeconds(0.8f);

        movement.enabled = true;

        isShooting = false;

        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;

        muzzleFlash.SetActive(false);
    }

    void FireBullet()
    {
        GameObject bulletObj = bulletPool.GetBullet();

        bulletObj.transform.position = shootPoint.position;
        bulletObj.transform.rotation = Quaternion.LookRotation(transform.forward);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.Fire(transform.forward);

    }
}
