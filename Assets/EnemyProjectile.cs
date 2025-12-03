using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float baseSpeed = 6f; // por si no mandas velocidad desde fuera

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogWarning("[EnemyProjectile] No hay Rigidbody2D en el proyectil.");
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// Llamado por la planta al crear la bala.
    /// </summary>
    public void Init(Vector2 direction, float speed)
    {
        if (rb == null) return;

        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = Vector2.right; // por si acaso
        }

        float finalSpeed = speed > 0 ? speed : baseSpeed;
        rb.linearVelocity = direction.normalized * finalSpeed;
        // Debug.Log("[EnemyProjectile] Velocidad: " + rb.velocity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthArmor hp = other.GetComponent<PlayerHealthArmor>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }

            gameObject.SetActive(false);
        }
        //else if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        //{
        //    Destroy(gameObject);
        //}
    }
}
