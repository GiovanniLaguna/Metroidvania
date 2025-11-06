using UnityEngine;

public class GhostEnemy : EnemyBase
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 1.4f;
    [SerializeField] private float hoverAmplitude = 0.2f;
    [SerializeField] private float hoverFrequency = 2f;
    [SerializeField] private float chaseRange = 12f;

    private Transform player;
    private Vector3 origin;

    protected override void Awake()
    {
        base.Awake();
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
        origin = transform.position;
    }

    void Update()
    {
        Vector3 pos = transform.position;
        pos.y = origin.y + Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        transform.position = pos;

        if (!player) return;

        float dist = Vector2.Distance(player.position, transform.position);
        if (dist > chaseRange) { animator?.SetFloat("Speed", 0f); return; }

        // moverse hacia el jugador atravesando todo (no usamos física)
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)(dir * speed * Time.deltaTime);

        // flip visual
        if (dir.x > 0 && transform.localScale.x < 0 || dir.x < 0 && transform.localScale.x > 0)
        {
            var s = transform.localScale; s.x *= -1; transform.localScale = s;
        }

        animator?.SetFloat("Speed", speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            other.GetComponentInParent<PlayerHealthArmor>()?.TakeDamage(1);
    }
}
