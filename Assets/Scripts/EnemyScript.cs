using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movimiento")]
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float activationRange = 10f; // se activa si el player est� cerca
    [SerializeField] private Transform foot;
    [SerializeField] private LayerMask groundLayer;

    [Header("Drop (opcional)")]
    [SerializeField] private GameObject drop;

    [Header("Spawn")]
    [SerializeField] private float spawnDuration = 0.6f; // tiempo de "salir de la tumba"
    [SerializeField] private Animator animator;

    private Transform player;
    private bool isSpawning = true;
    private float spawnTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // buscamos player por tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;

        // empezamos en modo spawn
        isSpawning = true;
        spawnTimer = spawnDuration;

        if (animator != null)
            animator.SetTrigger("Spawn");
    }

    void Update()
    {
        // mientras est� saliendo del suelo, no hace nada
        if (isSpawning)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
                isSpawning = false;
            return;
        }

        if (player == null)
            return;

        // si el player est� muy lejos, no gastamos
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > activationRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // siempre mirar hacia el player
        FacePlayer();

        // si no hay piso, lo frenamos para que no se caiga infinitamente
        if (!IsGrounded())
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void FixedUpdate()
    {
        if (isSpawning) return;
        if (player == null) return;

        // si no hay piso, no avanzamos
        if (!IsGrounded())
            return;

        // avanzar hacia el player en X
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
    }

    private void FacePlayer()
    {
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        // si dir > 0 miramos a la derecha, si dir < 0 miramos a la izquierda
        if (dir > 0 && transform.localScale.x < 0 ||
            dir < 0 && transform.localScale.x > 0)
        {
            Vector3 s = transform.localScale;
            s.x *= -1;
            transform.localScale = s;
        }
    }

    bool IsGrounded()
    {
        if (foot == null) return true;
        return Physics2D.OverlapCircle(foot.position, 0.2f, groundLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // bala del jugador
        if (collision.CompareTag("Bullet"))
        {
            // podr�as hacer aqu� una anim de muerte
            Die();
            Destroy(collision.gameObject);
        }
        // si quieres que da�e al jugador, lo puedes checar aqu� tambi�n
    }

    private void Die()
    {
        if (drop != null)
            Instantiate(drop, transform.position, Quaternion.identity);

        gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        // para ver el rango en el editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activationRange);
    }
}
