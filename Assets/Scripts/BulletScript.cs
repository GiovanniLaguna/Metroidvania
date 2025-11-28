using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float maxTime = 2f;
    [SerializeField] private float defaultSpeed = 15f;

    [Header("Direction")]
    [SerializeField] private Vector2 defaultDirection = Vector2.right; // derecha por defecto

    private Rigidbody2D rb;
    private float speed;
    private float currentTime;

    // Dirección actual de la bala (se puede setear desde el Player)
    private Vector2 direction = Vector2.zero;

    // ------------------------------
    //             INIT
    // ------------------------------
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        currentTime = 0f;

        // Si nadie llamó SetSpeed, usa la velocidad por defecto
        if (Mathf.Approximately(speed, 0f))
        {
            speed = defaultSpeed;
        }

        // Si nadie llamó SetDirection, usa la dirección por defecto (derecha)
        if (direction == Vector2.zero)
        {
            direction = defaultDirection;
        }
    }

    // ------------------------------
    //      SET / GET SPEED
    // ------------------------------
    public void SetSpeed(float value)
    {
        speed = value;
    }

    public float GetSpeed()
    {
        return speed;
    }

    // ------------------------------
    //     SET / GET DIRECTION
    // ------------------------------
    public void SetDirection(Vector2 dir)
    {
        // Normalizamos para que solo importe la dirección,
        // la magnitud la controla "speed".
        direction = dir.normalized;
    }

    public Vector2 GetDirection()
    {
        return direction;
    }

    // ------------------------------
    //            UPDATE
    // ------------------------------
    private void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime >= maxTime)
        {
            gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        // Velocidad constante usando la dirección (soporta diagonales)
        rb.linearVelocity = direction * speed;
    }

    // ------------------------------
    //          COLLISIONS
    // ------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si quieres hacer daño a algo que tenga HpBase:
        
        HpBase hp = collision.collider.GetComponent<HpBase>();
        if (hp != null)
        {
            hp.RemoveHp(1);
        }
       

        // Devuelve la bala al pool
        gameObject.SetActive(false);
    }
}
