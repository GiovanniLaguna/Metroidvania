using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HpEnemy))]
public class BossController : MonoBehaviour
{
    private enum BossState
    {
        Idle,
        Moving,
        Attacking,
        Enraged,
        Dead
    }

    [Header("Movement")]
    [SerializeField] private Transform leftLimit;
    [SerializeField] private Transform rightLimit;
    [SerializeField] private float moveSpeed = 2f;

    [Header("Attacks")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private int projectilesPerBurst = 3;
    [SerializeField] private float timeBetweenProjectiles = 0.2f;

    [Header("Phase 2 (Enrage)")]
    [Tooltip("Cuando la vida actual sea menor o igual a este valor, entra en modo Enraged.")]
    [SerializeField] private int enragedHpThreshold = 10;
    [SerializeField] private float enragedMoveSpeedMultiplier = 1.5f;
    [SerializeField] private float enragedAttackCooldownMultiplier = 0.7f;

    [Header("Animator")]
    [SerializeField] private Animator animator;
    [SerializeField] private string walkBoolName = "IsWalking";
    [SerializeField] private string attackTriggerName = "Attack";
    [SerializeField] private string enragedBoolName = "IsEnraged";

    [Header("Eventos del jefe")]
    public UnityEvent onBossFightStart;
    public UnityEvent onBossPhase2;
    public UnityEvent onBossDefeated;

    private HpEnemy hpEnemy;
    private BossState currentState = BossState.Idle;
    private bool bossActive = false;
    private bool facingRight = true;
    private float nextAttackTime = 0f;
    private bool isEnraged = false;

    private void Awake()
    {
        hpEnemy = GetComponent<HpEnemy>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        // Nos suscribimos a los eventos de vida del enemigo
        hpEnemy.onDeathEvent.AddListener(HandleBossDeath);
        //hpEnemy.onModifyHpEvent.AddListener(OnHpChanged);
    }

    private void OnDisable()
    {
        hpEnemy.onDeathEvent.RemoveListener(HandleBossDeath);
        //hpEnemy.onModifyHpEvent.RemoveListener(OnHpChanged);
    }

    private void Update()
    {
        if (!bossActive || currentState == BossState.Dead)
            return;

        HandleStateLogic();
    }

    /// <summary>
    /// Llama a esto desde un trigger cuando el jugador entra al cuarto del jefe.
    /// </summary>
    public void StartBossFight()
    {
        if (bossActive) return;

        bossActive = true;
        currentState = BossState.Moving;
        nextAttackTime = Time.time + attackCooldown;

        onBossFightStart?.Invoke();
    }

    private void HandleStateLogic()
    {
        switch (currentState)
        {
            case BossState.Moving:
                DoMovement();
                TryAttack();
                break;

            case BossState.Attacking:
                // El ataque se maneja en una corrutina, solo esperamos aquí.
                break;

            case BossState.Enraged:
                DoMovement();
                TryAttack();
                break;
        }
    }

    private void DoMovement()
    {
        if (leftLimit == null || rightLimit == null)
            return;

        // Determinar hacia dónde caminar
        Transform target = facingRight ? rightLimit : leftLimit;

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        // Activar animación de caminar
        if (animator != null)
            animator.SetBool(walkBoolName, true);

        // Voltear sprite
        if ((facingRight && transform.position.x >= rightLimit.position.x) ||
            (!facingRight && transform.position.x <= leftLimit.position.x))
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime)
            return;

        // Comenzar ataque
        StartCoroutine(AttackRoutine());
        nextAttackTime = Time.time + attackCooldown;
    }

    private IEnumerator AttackRoutine()
    {
        currentState = isEnraged ? BossState.Enraged : BossState.Attacking;

        // Animación
        if (animator != null && !string.IsNullOrEmpty(attackTriggerName))
            animator.SetTrigger(attackTriggerName);

        yield return new WaitForSeconds(0.1f); // pequeño delay antes de disparar

        

        // Volvemos al estado de movimiento
        currentState = isEnraged ? BossState.Enraged : BossState.Moving;
    }

    

    // ======================
    //   HP / FASE 2 / MUERTE
    // ======================

    /// <summary>
    /// Este método se llama cuando HpEnemy dispara onModifyHpEvent.
    /// Lo conectamos automáticamente en OnEnable.
    /// </summary>
    private void OnHpChanged(int currentHp)
    {
        if (isEnraged) return;

        if (currentHp <= enragedHpThreshold)
        {
            EnterEnragedPhase();
        }
    }

    private void EnterEnragedPhase()
    {
        isEnraged = true;
        currentState = BossState.Enraged;

        moveSpeed *= enragedMoveSpeedMultiplier;
        attackCooldown *= enragedAttackCooldownMultiplier;

        if (animator != null && !string.IsNullOrEmpty(enragedBoolName))
            animator.SetBool(enragedBoolName, true);

        onBossPhase2?.Invoke();
    }

    private void HandleBossDeath()
    {
        if (currentState == BossState.Dead)
            return;

        currentState = BossState.Dead;
        bossActive = false;

        // Podemos apagar animación de caminar
        if (animator != null)
            animator.SetBool(walkBoolName, false);

        onBossDefeated?.Invoke();
    }
}
