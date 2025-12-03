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
    [SerializeField] private Animator animator;

    [Header("Ground Check")]
    [SerializeField] private Transform foot;
    [SerializeField] private float footRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded;
    private int jumpCounter = 0;

    [Header("Salto")]
    [SerializeField] private int maxJumps = 1;

    [Header("Disparo")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float shootCooldown = 0.25f;
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private int startingAmmo = 10;
    [SerializeField] private bool infiniteAmmo = false;

    [Header("HP / Armadura")]
    [SerializeField] private PlayerHealthArmor healthArmor;

    [Header("UI")]
    [SerializeField] private Text ammoText;

    private int currentAmmo;

    [Header("Audio Disparo")]
    [SerializeField] private AudioClip shootSfx;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip theme;

    [Header("Object Pooling")]
    [SerializeField] private int initialPoolSize = 10;
    private readonly List<GameObject> bullets = new List<GameObject>();

    [Header("PowerUps")]
    [SerializeField] private bool usingKnife = false;
    [SerializeField] private bool crossbowActive = false;

    [Header("Crossbow Settings")]
    [SerializeField] private float crossbowSpreadAngle = 15f;
    [SerializeField] private int crossbowBulletCount = 3;

    private bool canShoot = true;
    private float shootTimer = 0f;

    private void Start()
    {
        PlayMusic();
        rb = GetComponent<Rigidbody2D>();

        if (healthArmor == null)
            healthArmor = GetComponent<PlayerHealthArmor>();

        // Crear pool de balas
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject clone = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
            clone.SetActive(false);
            bullets.Add(clone);
        }

        currentAmmo = Mathf.Clamp(startingAmmo, 0, maxAmmo);
        UpdateAmmoUI();

        // ======= CARGAR POWERUPS PERSISTENTES DESDE GAMEMANAGER =======
        if (GameManager.hasKnifePowerup)
            usingKnife = true;

        if (GameManager.hasCrossbowPowerup)
            crossbowActive = true;
    }

    private void Update()
    {
        CheckGround();

        // Entrada horizontal
        inputX = Input.GetAxisRaw("Horizontal");

        // ANIMACIÓN DE MOVIMIENTO
        if (animator != null)
            animator.SetFloat("Speed", Mathf.Abs(inputX));

        // Movimiento
        HandleMovement();

        // Salto
        HandleJump();

        // Flip
        HandleFlip();

        // Disparo
        HandleShooting();
    }

    private void HandleMovement()
    {
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded || jumpCounter < maxJumps)
            {
                Jump();
                jumpCounter++;
            }
        }
    }

    public void EnableDoubleJump()
    {
        maxJumps = 2;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
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
    private void HandleShooting()
    {
        if (!canShoot)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootCooldown)
            {
                canShoot = true;
                shootTimer = 0f;
            }
        }

        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (!infiniteAmmo && currentAmmo <= 0)
        {
            return;
        }

        if (crossbowActive)
        {
            ShootCrossbow();
        }
        else
        {
            ShootSingle();
        }

        if (!infiniteAmmo)
        {
            currentAmmo = Mathf.Max(0, currentAmmo - 1);
            UpdateAmmoUI();
        }

        canShoot = false;
        PlayShootSound();
    }

    private void ShootSingle()
    {
        GameObject bullet = GetPooledBullet();

        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = Quaternion.identity; // rotación simple
            bullet.SetActive(true);

            BulletScript b = bullet.GetComponent<BulletScript>();
            if (b != null)
            {
                // Velocidad de la bala
                b.SetSpeed(bulletSpeed);

                // Dirección horizontal según hacia dónde mira el player
                Vector2 dir = facingRight ? Vector2.right : Vector2.left;
                b.SetDirection(dir);
            }
        }
    }

    private void ShootCrossbow()
    {
        // Por seguridad
        if (crossbowBulletCount <= 1)
        {
            ShootSingle();
            return;
        }

        float startAngle = -crossbowSpreadAngle;
        float endAngle = crossbowSpreadAngle;

        // Dirección base (derecha o izquierda)
        Vector2 baseDir = facingRight ? Vector2.right : Vector2.left;

        for (int i = 0; i < crossbowBulletCount; i++)
        {
            float t = (float)i / (crossbowBulletCount - 1);
            float angle = Mathf.Lerp(startAngle, endAngle, t);

            // Convertimos el ángulo a radianes y rotamos baseDir
            float rad = angle * Mathf.Deg2Rad;

            // Rotación 2D de un vector
            Vector2 dir = new Vector2(
                baseDir.x * Mathf.Cos(rad) - baseDir.y * Mathf.Sin(rad),
                baseDir.x * Mathf.Sin(rad) + baseDir.y * Mathf.Cos(rad)
            );

            FireBulletInDirection(dir);
        }
    }

    private void FireBulletInDirection(Vector2 dir)
    {
        GameObject bullet = GetPooledBullet();
        if (bullet == null)
            return;

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = Quaternion.identity;
        bullet.SetActive(true);

        BulletScript b = bullet.GetComponent<BulletScript>();
        if (b != null)
        {
            b.SetSpeed(bulletSpeed);
            b.SetDirection(dir);
        }
    }

    private GameObject GetPooledBullet()
    {
        foreach (GameObject bullet in bullets)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }

        GameObject newBullet = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
        newBullet.SetActive(false);
        bullets.Add(newBullet);
        return newBullet;
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            if (infiniteAmmo)
            {
                ammoText.text = "∞";
            }
            else
            {
                ammoText.text = currentAmmo.ToString();
            }
        }
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        UpdateAmmoUI();
    }

    public void SetInfiniteAmmo(bool value)
    {
        infiniteAmmo = value;
        UpdateAmmoUI();
    }

    // ------------------------------
    //      POWERUPS PERSISTENTES
    // ------------------------------
    public void ActivateKnifePowerup()
    {
        usingKnife = true;
        GameManager.hasKnifePowerup = true;   // se guarda entre escenas
    }

    public void ActivateCrossbowPowerup()
    {
        crossbowActive = true;
        GameManager.hasCrossbowPowerup = true; // se guarda entre escenas
    }

    public void ResetWeapon()
    {
        usingKnife = false;
        crossbowActive = false;

        GameManager.hasKnifePowerup = false;
        GameManager.hasCrossbowPowerup = false;
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
        if (collision.transform.CompareTag("Enemy"))
        {
            healthArmor?.TakeDamage(1);
        }
    }

    public void EnableControl()
    {
        this.enabled = true;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
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

    private void PlayShootSound()
    {
        if (shootSfx == null) return;

        if (audioSource != null)
            audioSource.PlayOneShot(shootSfx);
        else
            AudioSource.PlayClipAtPoint(shootSfx, transform.position);
    }

    private void PlayMusic()
    {
        // Usa tu sistema de sonido actual
        SoundList.instance.PlaySound("Theme");
    }
}
