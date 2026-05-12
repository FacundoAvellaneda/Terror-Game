using IA2;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using static Personaje;

public class EnemyFSM : MonoBehaviour
{
    [SerializeField] public enum EnemyState { Idle, Chase, Attack, Damage, Die }
    [SerializeField] private EventFSM<EnemyState> _enemyFsm;

    [Header("Variables")]
    [SerializeField] private Transform _player;
    [SerializeField] private Rigidbody _myRig;
    [SerializeField] private Animator _myAnim;

    [Header("Attack Settings")]
    [SerializeField] private float _attackRange;
    [SerializeField] private float _chaseRange;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private bool _canAttack;

    private bool _isTakingDamage = false;
    [SerializeField] private float _damageDuration = 0.5f;


    private void Awake()
    {
        _myRig = GetComponent<Rigidbody>();
        _myAnim = GetComponent<Animator>();

        var idle = new State<EnemyState>("IDLE");
        var chase = new State<EnemyState>("CHASE");
        var attack = new State<EnemyState>("ATTACK");
        var die = new State<EnemyState>("DIE");
        var damage = new State<EnemyState>("DAMAGE");

        StateConfigurer.Create(idle)
            .SetTransition(EnemyState.Chase, chase)
            .SetTransition(EnemyState.Attack, attack)
            .SetTransition(EnemyState.Die, die)
            .SetTransition(EnemyState.Damage, damage)
            .Done();

        StateConfigurer.Create(chase)
            .SetTransition(EnemyState.Idle, idle)
            .SetTransition(EnemyState.Attack, attack)
            .SetTransition(EnemyState.Die, die)
            .SetTransition(EnemyState.Damage, damage)
            .Done();

        StateConfigurer.Create(attack)
            .SetTransition(EnemyState.Idle, idle)
            .SetTransition(EnemyState.Chase, chase)
            .SetTransition(EnemyState.Die, die)
            .SetTransition(EnemyState.Damage, damage)
            .Done();

        StateConfigurer.Create(damage)
            .SetTransition(EnemyState.Idle, idle)
            .SetTransition(EnemyState.Chase, chase)
            .SetTransition(EnemyState.Attack, attack)
            .SetTransition(EnemyState.Die, die)
             .Done();


        StateConfigurer.Create(die).Done();

        idle.OnEnter += x =>
        {
            _myAnim.SetBool("isMoving", false);
            Debug.Log("ENTRE A IDLE");

        };

        idle.OnUpdate += () =>
        {
            if (_isTakingDamage) return;

            float dist = Vector3.Distance(transform.position, _player.position);
            if (dist < _chaseRange && dist > _attackRange)
                SendInputToFSM(EnemyState.Chase);
            else if (dist <= _attackRange)
                SendInputToFSM(EnemyState.Attack);
        };

        chase.OnEnter += x =>
        {
            Debug.Log("ENTRE A CHASE");
            _myAnim.SetBool("isMoving", true);
        };

        chase.OnUpdate += () =>
        {
            if (_isTakingDamage) return;

            float dist = Vector3.Distance(transform.position, _player.position);

            if (dist >= _chaseRange)
                SendInputToFSM(EnemyState.Idle);
            else if (dist <= _attackRange)
                SendInputToFSM(EnemyState.Attack);
            else
            {
                Vector3 dir = (_player.position - transform.position);
                dir.y = 0f;

                dir = dir.normalized;

                _myRig.MovePosition(transform.position + dir * Time.deltaTime);

                if (dir != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
                }
            }
        };

        attack.OnEnter += x =>
        {
            Debug.Log("ENTRE A ATTACK");

            int attackIndex = Random.Range(0, 2);
            _myAnim.SetInteger("AttackIndex", attackIndex);
            _myAnim.SetTrigger("Attack");
            _canAttack = true;
        };

        attack.OnUpdate += () =>
        {
            if (_isTakingDamage) return;

            float dist = Vector3.Distance(transform.position, _player.position);
            if (dist > _attackRange && dist < _chaseRange)
                SendInputToFSM(EnemyState.Chase);
            else if (dist >= _chaseRange)
                SendInputToFSM(EnemyState.Idle);
        };

        damage.OnEnter += x =>
        {
            _isTakingDamage = true;

            // frenar movimiento
            _myRig.linearVelocity = Vector3.zero;

            // animaci�n
            _myAnim.SetTrigger("Damage");

            // bloquear otros estados por tiempo
            StartCoroutine(DamageRoutine());
        };


        _enemyFsm = new EventFSM<EnemyState>(idle);

    }

    private void Update()
    {
        _enemyFsm.Update();
    }

    private void SendInputToFSM(EnemyState inp) => _enemyFsm.SendInput(inp);    

    IEnumerator DamageRoutine()
    {
        yield return new WaitForSeconds(_damageDuration);

        _isTakingDamage = false;

        // volver a l�gica normal (decide seg�n distancia)
        float dist = Vector3.Distance(transform.position, _player.position);

        if (dist <= _attackRange)
            SendInputToFSM(EnemyState.Attack);
        else if (dist < _chaseRange)
            SendInputToFSM(EnemyState.Chase);
        else
            SendInputToFSM(EnemyState.Idle);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _chaseRange);
    }
}