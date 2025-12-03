using UnityEngine;
using DG.Tweening;

public class CarnivorousPlant : MonoBehaviour
{
    [Header("Detección")]
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

    [Header("Animación (opcional)")]
    [SerializeField] private Animator animator; // open / close

    private bool isAttacking = false;
    private float timer;

    private void Start()
    {
        currentHealth = maxHealth;

        // si no asignaste player en el inspector, lo buscamos por tag
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
                Debug.Log("[Planta] Player encontrado por tag.");
            }
            else
            {
                Debug.LogWarning("[Planta] NO encontré ningún objeto con tag 'Player'.");
            }
        }

        timer = idleTime;
    }

    private void Update()
    {
        if (player == null)
        {
            // si ves este log en consola, el problema es el tag o la referencia del player
            Debug.LogWarning("[Planta] Player es null, no puedo detectar distancia.");
            return;
        }
        LookAtPlayer();

        float dist = Vector2.Distance(transform.position, player.position);
        // Debug de distancia
        // Debug.Log("[Planta] Distancia al player: " + dist);

        // si el jugador está muy lejos, la planta se queda dormida
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
                Debug.Log("[Planta] Comienza ataque. Distancia actual: " + dist);
                StartAttack(dist);
            }
        }
        else
        {
            // estamos atacando
            if (timer <= 0f)
            {
                Debug.Log("[Planta] Termina ataque.");
                EndAttack();
            }
        }
    }

    private void StartAttack(float currentDistToPlayer)
    {
        isAttacking = true;
        timer = attackTime;
        SetAnimAttack(true);

        // si el jugador está en rango de ataque en este momento, atacamos
        if (currentDistToPlayer <= attackRange)
        {
            if (shoots)
            {
                Debug.Log("[Planta] Disparo al jugador.");
                ShootAtPlayer();
            }
            else
            {
                Debug.Log("[Planta] Muerdo al jugador.");
                BitePlayerIfClose();
            }
        }
        else
        {
            Debug.Log("[Planta] Player dentro de activationRange pero fuera de attackRange.");
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
        {
            Debug.LogWarning("[Planta] Falta projectilePrefab o shootPoint o player.");
            return;
        }

        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Vector2 dir = (player.position - shootPoint.position).normalized;

        // Primero probamos con el script EnemyProjectile
        EnemyProjectile enemyProj = proj.GetComponent<EnemyProjectile>();
        if (enemyProj != null)
        {
            enemyProj.Init(dir, projectileSpeed);
        }
        else
        {
            // Plan B por si no lo tiene: asignar velocidad directa
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * projectileSpeed;
            }
            else
            {
                Debug.LogWarning("[Planta] El proyectil no tiene EnemyProjectile ni Rigidbody2D.");
            }
        }
    }



    private void BitePlayerIfClose()
    {
        // TODO: aquí puedes hacer un OverlapCircle para daño cuerpo a cuerpo
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        // Evita que vuelva a tomar daño o ataque
        this.enabled = false;

        // Tween de escala (se hace chiquita)
        transform.DOScale(Vector3.zero, 0.35f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                gameObject.SetActive(false); // o Destroy(gameObject)
            });
    }

    private void SetAnimAttack(bool value)
    {
        if (animator != null)
        {
            animator.SetBool("IsOpen", value);
        }
    }

    // opcional: ver área en editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si quieres que la planta reciba daño al colisionar con algo, hazlo aquí
        if (collision.CompareTag("Bullet"))
        {
            Die();

        collision.gameObject.SetActive(false); // desactivar bala
        }
    }
    private void LookAtPlayer()
    {
        if (player == null) return;

        float dir = player.position.x - transform.position.x;

        // Si el jugador está a la derecha → mira a la derecha
        if (dir > 0 && transform.localScale.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        // Si el jugador está a la izquierda → mira a la izquierda
        else if (dir < 0 && transform.localScale.x > 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

}
