//using System.Collections;
//using UnityEngine;
//using UnityEngine.Events;

//[RequireComponent(typeof(HpEnemy))]
//[RequireComponent(typeof(Rigidbody2D))]
//public class UnicornBossController : MonoBehaviour
//{
//    private enum BossState
//    {
//        Idle,
//        Walking,
//        Charging,
//        Jumping,
//        Dead
//    }

//    [Header("Refs")]
//    [SerializeField] private Transform leftLimit;
//    [SerializeField] private Transform rightLimit;
//    [SerializeField] private Transform player; // opcional, si no se asigna lo buscamos por tag
//    [SerializeField] private Animator animator;

//    [Header("Movimiento base")]
//    [SerializeField] private float walkSpeed = 1.5f;

//    [Header("Ataque de carga (Charge)")]
//    [SerializeField] private float chargeSpeed = 6f;
//    [SerializeField] private float chargeDuration = 1.0f;
//    [SerializeField] private float chargeWindupTime = 0.4f; // se agacha / telegraph

//    [Header("Ataque de salto (Jump)")]
//    [SerializeField] private float jumpHorizontalSpeed = 4f;
//    [SerializeField] private float jumpForce = 8f;
//    [SerializeField] private float jumpWindupTime = 0.4f;

//    [Header("Ciclo de ataques")]
//    [SerializeField] private float timeBetweenAttacksMin = 1.0f;
//    [SerializeField] private float timeBetweenAttacksMax = 2.0f;

//    [Header("Da�o al jugador")]
//    [SerializeField] private int contactDamage = 1;
//    [SerializeField] private float contactKnockbackForce = 7f;

//    [Header("Animator params")]
//    [SerializeField] private string walkBoolName = "IsWalking";
//    [SerializeField] private string chargeTriggerName = "Charge";
//    [SerializeField] private string jumpTriggerName = "Jump";
//    [SerializeField] private string damageTriggerName = "Damage";
//    [SerializeField] private string deadTriggerName = "Death";

//    [Header("Eventos")]
//    public UnityEvent onBossFightStart;
//    public UnityEvent onBossDefeated;

//    private HpEnemy hpEnemy;
//    private Rigidbody2D rb;
//    private BossState state = BossState.Idle;
//    private bool facingRight = false;
//    private bool fightStarted = false;
//    private bool isGrounded = true; // puedes conectar esto a tu ground check si quieres

//    private void Awake()
//    {
//        hpEnemy = GetComponent<HpEnemy>();
//        rb = GetComponent<Rigidbody2D>();

//        if (animator == null)
//            animator = GetComponentInChildren<Animator>();

//        if (player == null)
//        {
//            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
//            if (playerGO != null)
//                player = playerGO.transform;
//        }
//    }

//    private void OnEnable()
//    {
//        hpEnemy.onDeathEvent.AddListener(HandleBossDeath);
//        hpEnemy.onModifyHpEvent.AddListener(OnHpChanged);
//    }

//    private void OnDisable()
//    {
//        hpEnemy.onDeathEvent.RemoveListener(HandleBossDeath);
//        hpEnemy.onModifyHpEvent.RemoveListener(OnHpChanged);
//    }

//    private void Update()
//    {
//        if (!fightStarted || state == BossState.Dead)
//            return;

//        if (state == BossState.Walking)
//        {
//            DoWalk();
//        }
//    }

//    /// <summary>
//    /// Llama esto desde un trigger cuando el jugador entra al cuarto del jefe.
//    /// </summary>
//    public void StartBossFight()
//    {
//        if (fightStarted) return;

//        fightStarted = true;
//        state = BossState.Walking;

//        onBossFightStart?.Invoke();

//        // empezamos el ciclo de IA
//        StartCoroutine(AIBehaviourLoop());
//    }

//    private IEnumerator AIBehaviourLoop()
//    {
//        // ciclo infinito hasta morir
//        while (fightStarted && state != BossState.Dead)
//        {
//            // caminar un poco "como unicornio"
//            state = BossState.Walking;
//            float walkTime = Random.Range(1.0f, 2.0f);
//            float timer = 0f;

//            while (timer < walkTime && state == BossState.Walking)
//            {
//                timer += Time.deltaTime;
//                yield return null;
//            }

//            // decidir ataque: carga o salto
//            if (state == BossState.Dead) yield break;

//            bool doCharge = Random.value > 0.4f; // 60% carga, 40% salto

//            if (doCharge)
//            {
//                yield return ChargeAttack();
//            }
//            else
//            {
//                yield return JumpAttack();
//            }

//            // esperar un poco antes del siguiente ataque
//            float delay = Random.Range(timeBetweenAttacksMin, timeBetweenAttacksMax);
//            float t = 0f;
//            while (t < delay && state != BossState.Dead)
//            {
//                t += Time.deltaTime;
//                yield return null;
//            }
//        }
//    }

//    // =====================
//    //   MOVIMIENTO B�SICO
//    // =====================

//    private void DoWalk()
//    {
//        if (leftLimit == null || rightLimit == null)
//            return;

//        // seguir caminando hacia la direcci�n actual
//        float dir = facingRight ? 1f : -1f;
//        rb.linearVelocity = new Vector2(dir * walkSpeed, rb.linearVelocity.y);

//        if (animator != null)
//            animator.SetBool(walkBoolName, true);

//        // cambiar de direcci�n si llegamos a un l�mite
//        if (facingRight && transform.position.x >= rightLimit.position.x)
//            Flip(false);
//        else if (!facingRight && transform.position.x <= leftLimit.position.x)
//            Flip(true);
//    }

//    private void Flip(bool lookRight)
//    {
//        facingRight = lookRight;
//        Vector3 localScale = transform.localScale;
//        localScale.x = Mathf.Abs(localScale.x) * (facingRight ? 1f : -1f);
//        transform.localScale = localScale;
//    }

//    // =====================
//    //   ATAQUE: CARGA
//    // =====================

//    private IEnumerator ChargeAttack()
//    {
//        state = BossState.Charging;

//        // mirar hacia el jugador antes de cargar
//        if (player != null)
//        {
//            bool playerRight = player.position.x > transform.position.x;
//            Flip(playerRight);
//        }

//        // detener movimiento y hacer "wind-up"
//        rb.linearVelocity = Vector2.zero;
//        if (animator != null)
//        {
//            animator.SetBool(walkBoolName, false);
//            if (!string.IsNullOrEmpty(chargeTriggerName))
//                animator.SetTrigger(chargeTriggerName);
//        }

//        yield return new WaitForSeconds(chargeWindupTime);

//        // carga recta
//        float dir = facingRight ? 1f : -1f;
//        float timer = 0f;

//        while (timer < chargeDuration && state != BossState.Dead)
//        {
//            rb.linearVelocity = new Vector2(dir * chargeSpeed, rb.linearVelocity.y);
//            timer += Time.deltaTime;
//            yield return null;
//        }

//        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
//        state = BossState.Walking;
//    }

//    // =====================
//    //   ATAQUE: SALTO ARCO
//    // =====================

//    private IEnumerator JumpAttack()
//    {
//        state = BossState.Jumping;

//        if (player != null)
//        {
//            bool playerRight = player.position.x > transform.position.x;
//            Flip(playerRight);
//        }

//        rb.linearVelocity = Vector2.zero;

//        if (animator != null && !string.IsNullOrEmpty(jumpTriggerName))
//            animator.SetTrigger(jumpTriggerName);

//        yield return new WaitForSeconds(jumpWindupTime);

//        // salto parab�lico hacia delante
//        float dir = facingRight ? 1f : -1f;
//        rb.linearVelocity = new Vector2(dir * jumpHorizontalSpeed, 0f);
//        rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

//        // esperamos a que toque suelo de nuevo de forma muy simple
//        yield return new WaitForSeconds(0.6f);
//        state = BossState.Walking;
//    }

//    // =====================
//    //   DA�O AL JUGADOR
//    // =====================

//    private void OnCollisionEnter2D(Collision2D collision)
//    {
//        TryDamagePlayer(collision.collider);
//    }

//    private void OnCollisionStay2D(Collision2D collision)
//    {
//        TryDamagePlayer(collision.collider);
//    }

//    private void OnTriggerEnter2D(Collider2D other)
//    {
//        TryDamagePlayer(other);
//    }

//    private void TryDamagePlayer(Collider2D col)
//    {
//        if (!col.CompareTag("Player"))
//            return;

//        // usar tu sistema de armadura
//        var armor = col.GetComponentInParent<PlayerHealthArmor>();
//        if (armor != null)
//        {
//            armor.TakeDamage(contactDamage);
//        }
//        else
//        {
//            // fallback al HpPlayer directo
//            var hpPlayer = col.GetComponentInParent<HpPlayer>();
//            if (hpPlayer != null)
//                hpPlayer.RemoveHp(contactDamage);
//        }

//        // knockback sencillo
//        Rigidbody2D playerRb = col.GetComponentInParent<Rigidbody2D>();
//        if (playerRb != null)
//        {
//            Vector2 dir = (playerRb.transform.position - transform.position).normalized;
//            dir.y = Mathf.Abs(dir.y); // que siempre tire hacia arriba un poco
//            playerRb.AddForce(dir * contactKnockbackForce, ForceMode2D.Impulse);
//        }
//    }

//    // =====================
//    //   VIDA / MUERTE
//    // =====================

//    private void OnHpChanged(int currentHp)
//    {
//        // si quieres hacer algo tipo enrage cuando le quede poca vida,
//        // aqu� puedes meter l�gica m�s adelante.
//    }

//    private void HandleBossDeath()
//    {
//        if (state == BossState.Dead)
//            return;

//        state = BossState.Dead;
//        fightStarted = false;

//        rb.linearVelocity = Vector2.zero;

//        if (animator != null && !string.IsNullOrEmpty(deadTriggerName))
//            animator.SetTrigger(deadTriggerName);

//        onBossDefeated?.Invoke(); // aqu� puedes llamar GameManager.GameWon desde el inspector
//    }
//}
