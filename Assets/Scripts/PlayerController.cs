using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 4f;
    private float inputX;
    private bool facingRight = true;
    private Rigidbody2D rb;

    [Header("Ground Check")]
    [SerializeField] private Transform foot;
    [SerializeField] private float footRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    public bool isGrounded;
    private int jumpCounter = 0;
    private const int MAX_JUMPS = 2;

    [Header("Disparo / Pool de Balas")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform gun;
    [SerializeField] private int initialPoolSize = 5;
    private List<GameObject> bullets = new List<GameObject>();

    [Header("Arma / Powerups")]
    [SerializeField] private float normalBulletSpeed = 10f;
    [SerializeField] private float knifeBulletSpeed = 18f;
    private bool usingKnife = false;

    [Header("Munición")]
    [SerializeField] private int maxAmmo = 20;
    [SerializeField] private int startingAmmo = 10;
    [SerializeField] private bool infiniteAmmo = false;

    [Header("HP")]
    [SerializeField] private HpPlayer hpPlayer;

    [SerializeField] private Text ammoText;

    private int currentAmmo;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (hpPlayer == null)
            hpPlayer = GetComponent<HpPlayer>();

        // Crear pool de balas
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject clone = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
            clone.SetActive(false);
            bullets.Add(clone);
        }

        currentAmmo = Mathf.Clamp(startingAmmo, 0, maxAmmo);
        UpdateAmmoUI();
    }

    private void Update()
    {
        CheckGround();

        // Entrada horizontal
        inputX = Input.GetAxisRaw("Horizontal");

        // Salto
        if (Input.GetButtonDown("Jump"))
        {
            AttemptJump();
        }

        // Disparo
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        HandleFlip();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);
    }

    // ------------------------------
    //       SALTO / GROUND
    // ------------------------------
    private void AttemptJump()
    {
        if (!isGrounded && jumpCounter >= MAX_JUMPS)
            return;

        jumpCounter++;
        jumpCounter = Mathf.Clamp(jumpCounter, 0, MAX_JUMPS);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(foot.position, footRadius, groundLayer);

        if (isGrounded)
            jumpCounter = 0;
    }

    // ------------------------------
    //           DISPARO
    // ------------------------------
    private void Shoot()
    {
        if (!infiniteAmmo && currentAmmo <= 0)
            return;

        GameObject bullet = GetBulletFromPool();
        if (bullet == null)
            return;

        if (!infiniteAmmo)
        {
            currentAmmo = Mathf.Clamp(currentAmmo - 1, 0, maxAmmo);
            UpdateAmmoUI();
        }

        bullet.transform.position = gun.position;
        bullet.SetActive(true);

        int dir = facingRight ? 1 : -1;

        BulletScript b = bullet.GetComponent<BulletScript>();
        if (b != null)
        {
            float speed = usingKnife ? knifeBulletSpeed : normalBulletSpeed;
            b.SetSpeed(speed * dir);
        }
    }

    private GameObject GetBulletFromPool()
    {
        foreach (GameObject b in bullets)
        {
            if (!b.activeInHierarchy)
                return b;
        }

        return null; // No expandimos el pool
    }


    // ------------------------------
    //      POWERUP: CUCHILLO
    // ------------------------------
    public void ActivateKnifePowerup()
    {
        usingKnife = true;
        // Aquí se podría cambiar animación, color del arma, etc.
    }

    public void ResetWeapon()
    {
        usingKnife = false;
    }


    // ------------------------------
    //       MUNCIÓN PÚBLICA
    // ------------------------------
    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = currentAmmo.ToString();
    }

    // ------------------------------
    //           FLIP
    // ------------------------------
    private void HandleFlip()
    {
        if (inputX > 0 && !facingRight)
        {
            Flip();
        }
        else if (inputX < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // ------------------------------
    //   DAÑO POR COLISIONES
    // ------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Aquí solo daño real, sin deathzone
        if (collision.transform.CompareTag("Enemy"))
        {
            hpPlayer?.RemoveHp(1);
        }
    }

    public void Death()
    {
        rb.linearVelocity = Vector2.zero;
        this.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (foot == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(foot.position, footRadius);
    }
}
