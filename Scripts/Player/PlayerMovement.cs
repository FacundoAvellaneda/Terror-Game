using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour, ICanTrigger
{
    [Header("Movement")]
    public float speed = 4f;
    public float rotationSpeed = 10f;

    [Header("References")]
    [SerializeField] private StatsEntitty _myStats;
    public Animator animator;

    private CharacterController controller;
    private Vector2 input;
    private Vector3 moveDirection;

    private float gravity = -9.81f;
    private float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        speed = _myStats.Speed;
    }

    void Update()
    {
        HandleMovement();
        ApplyGravity();
    }

    public void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
    }
    void HandleMovement()
    {
        Vector3 desiredDirection = new Vector3(input.x, 0, input.y).normalized;

        Vector3 quantizedDir = QuantizeDirection(desiredDirection);

        if (quantizedDir.magnitude > 0)
        {
            moveDirection = quantizedDir;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        Vector3 velocity = moveDirection * speed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        float animX = Mathf.Round(input.x);
        float animY = Mathf.Round(input.y);

        animator.SetFloat("xAxis", animX);
        animator.SetFloat("yAxis", animY);
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;
    }

    Vector3 QuantizeDirection(Vector3 dir)
    {
        if (dir == Vector3.zero) return Vector3.zero;

        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(angle / 45f) * 45f;
        float rad = snappedAngle * Mathf.Deg2Rad;

        return new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad)).normalized;
    }

    public void OnTriggered()
    {
        Debug.Log("triggered an event!");
    }
}