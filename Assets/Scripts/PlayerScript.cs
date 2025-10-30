using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 4f;

    [Header("Ground Check")]
    [SerializeField] private Transform foot;
    [SerializeField] private float footRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Disparo")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform gun;
    [SerializeField] private int initialPoolSize = 5;

    private Rigidbody2D rb;
    private List<GameObject> bullets = new List<GameObject>();
    private float inputX;
    private bool facingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // pool inicial
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject clone = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
            clone.SetActive(false);
            bullets.Add(clone);
        }
    }

    private void Update()
    {
        // entrada horizontal
        inputX = Input.GetAxisRaw("Horizontal");

        // salto
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // resetea Y para saltos m�s consistentes
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // disparo
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        // virar sprite
        HandleFlip();
    }

    private void FixedUpdate()
    {
        // movimiento horizontal estable
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);
    }

    private void Shoot()
    {
        GameObject currentBullet = GetBulletFromPool();
        if (currentBullet == null) return;

        currentBullet.transform.position = gun.position;
        currentBullet.SetActive(true);

        // direcci�n seg�n hacia d�nde mira el player
        int dir = facingRight ? 1 : -1;

        BulletScript b = currentBullet.GetComponent<BulletScript>();
        if (b != null)
        {
            b.SetSpeed(10f * dir);
        }
    }

    private GameObject GetBulletFromPool()
    {
        foreach (GameObject b in bullets)
        {
            if (!b.activeInHierarchy)
                return b;
        }

        // si no hay, creamos m�s
        GameObject extra = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
        extra.SetActive(false);
        bullets.Add(extra);
        return extra;
    }

    private bool IsGrounded()
    {
        if (foot == null) return false;
        return Physics2D.OverlapCircle(foot.position, footRadius, groundLayer);
    }

    private void HandleFlip()
    {
        if (inputX > 0 && !facingRight)
        {
            facingRight = true;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (inputX < 0 && facingRight)
        {
            facingRight = false;
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("DeathZone"))
        {
            GameManager.instance.playerLifes--;
            transform.position = Vector3.zero;
        }
    }

    // para ver el groundcheck
    private void OnDrawGizmosSelected()
    {
        if (foot == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(foot.position, footRadius);
    }
}
