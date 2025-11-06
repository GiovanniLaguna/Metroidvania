using UnityEngine;

public class CrowEnemy : EnemyBase
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float waveAmplitude = 0.5f;
    [SerializeField] private float waveFrequency = 4f;
    [SerializeField] private float aggroRange = 10f;

    private Transform player;
    private float baseY;

    protected override void Awake()
    {
        base.Awake();
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;
        baseY = transform.position.y;
    }

    void Update()
    {
        if (!player) return;

        float dist = Mathf.Abs(player.position.x - transform.position.x);
        if (dist > aggroRange) { animator?.SetFloat("Speed", 0f); return; }

        // Mirar hacia el jugador
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        if (dir > 0 && transform.localScale.x < 0 || dir < 0 && transform.localScale.x > 0)
        {
            var s = transform.localScale; s.x *= -1; transform.localScale = s;
        }

        // Movimiento: avance + onda senoidal en Y
        Vector3 pos = transform.position;
        pos.x += dir * speed * Time.deltaTime;
        pos.y = baseY + Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;
        transform.position = pos;

        animator?.SetFloat("Speed", speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Daño al player si tienes collider trigger
        if (other.CompareTag("Player"))
            other.GetComponentInParent<PlayerHealthArmor>()?.TakeDamage(1);
    }
}
