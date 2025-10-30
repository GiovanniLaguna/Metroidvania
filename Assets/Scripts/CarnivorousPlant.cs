using UnityEngine;

public class CarnivorousPlant : MonoBehaviour
{
    [Header("Detecci�n")]
    [SerializeField] private Transform player;
    [SerializeField] private float activationRange = 6f;   // distancia a la que empieza a hacer su ciclo
    [SerializeField] private float attackRange = 4f;       // distancia para disparar/morder

    [Header("Ciclo de ataque")]
    [SerializeField] private float idleTime = 2f;          // tiempo cerrada
    [SerializeField] private float attackTime = 1f;        // tiempo abierta/atacando
    [SerializeField] private bool loop = true;

    [Header("Disparo (opcional)")]
    [SerializeField] private bool shoots = true;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float projectileSpeed = 6f;

    [Header("Vida")]
    [SerializeField] private int maxHealth = 1;
    private int currentHealth;

    [Header("Animaci�n (opcional)")]
    [SerializeField] private Animator animator; // open / close

    private bool isAttacking = false;
    private float timer;

    private void Awake()
    {
        currentHealth = maxHealth;

        // si no asignaste player en el inspector, lo buscamos por tag
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        timer = idleTime;
    }

    private void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // si el jugador est� muy lejos, la planta se queda dormida
        if (dist > activationRange)
        {
            SetAnimAttack(false);
            return;
        }

        // control del ciclo
        timer -= Time.deltaTime;

        if (!isAttacking)
        {
            // estamos en estado idle, esperando a atacar
            if (timer <= 0f)
            {
                StartAttack(dist);
            }
        }
        else
        {
            // estamos atacando
            if (timer <= 0f)
            {
                EndAttack();
            }
        }
    }

    private void StartAttack(float currentDistToPlayer)
    {
        isAttacking = true;
        timer = attackTime;
        SetAnimAttack(true);

        // si el jugador est� en rango de ataque en este momento, atacamos
        if (currentDistToPlayer <= attackRange)
        {
            if (shoots)
                ShootAtPlayer();
            else
                BitePlayerIfClose();
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        timer = loop ? idleTime : Mathf.Infinity;
        SetAnimAttack(false);
    }

    private void ShootAtPlayer()
    {
        if (projectilePrefab == null || shootPoint == null || player == null)
            return;

        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Vector2 dir = (player.position - shootPoint.position).normalized;

        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = dir * projectileSpeed;
        }

        // si tu proyectil tiene script propio, aqu� le puedes pasar el da�o
        // proj.GetComponent<EnemyProjectile>()?.Init(damage, dir);
    }

    private void BitePlayerIfClose()
    {
        // aqu� puedes hacer un overlap para ver si lo agarra
        // Collider2D hit = Physics2D.OverlapCircle(mouthPoint.position, 0.5f, playerLayer);
        // if (hit != null) hit.GetComponent<PlayerHealth>()?.TakeDamage(1);
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        // anim de morir
        // Drop
        gameObject.SetActive(false);
    }

    private void SetAnimAttack(bool value)
    {
        if (animator != null)
        {
            animator.SetBool("IsOpen", value);
        }
    }

    // opcional: ver �rea en editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
