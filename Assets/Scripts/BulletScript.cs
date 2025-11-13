using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float maxTime = 2f;
    [SerializeField] private float defaultSpeed = 15f;

    private Rigidbody2D rb;
    private float speed;
    private float currentTime;

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
        // Velocidad constante en X, sin aceleración acumulada
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
    }

    // ------------------------------
    //          COLLISIONS
    // ------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si quieres hacer daño a algo que tenga HpBase:
        /*
        HpBase hp = collision.collider.GetComponent<HpBase>();
        if (hp != null)
        {
            hp.RemoveHp(1);
        }
        */

        // Devuelve la bala al pool
        gameObject.SetActive(false);
    }
}
