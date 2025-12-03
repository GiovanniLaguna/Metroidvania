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

    [Header("Object Pooling")]
    [SerializeField] private int initialPoolSize = 10;
    private readonly List<GameObject> bullets = new List<GameObject>();

<<<<<<< Updated upstream
=======
    [Header("PowerUps")]
    [SerializeField] private bool usingKnife = false;
    [SerializeField] private bool crossbowActive = false;

    [Header("Crossbow Settings")]
    [SerializeField] private float crossbowSpreadAngle = 15f;
    [SerializeField] private int crossbowBulletCount = 3;

    private bool canShoot = true;
    private float shootTimer = 0f;

>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
=======
        // ANIMACIÓN DE MOVIMIENTO
        if (animator != null)
            animator.SetFloat("Speed", Mathf.Abs(inputX));

        // Movimiento
        HandleMovement();

>>>>>>> Stashed changes
        // Salto
        HandleJump();

        // Flip
        HandleFlip();

        // Disparo
<<<<<<< Updated upstream
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        HandleFlip();
=======
        HandleShooting();
>>>>>>> Stashed changes
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
            return;

        if (crossbowActive)
        {
            Debug.Log("DISPARO SPREAD");
            ShootCrossbow();
        }
        else
        {
            Debug.Log("DISPARO NORMAL");
            ShootSingle();
        }
<<<<<<< Updated upstream
    }
=======
>>>>>>> Stashed changes

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

            // Dirección base: derecha o izquierda
            Vector2 dir = facingRight ? Vector2.right : Vector2.left;

            // Configuramos la dirección en el script de la bala
            BulletScript bulletScript = bullet.GetComponent<BulletScript>();
            if (bulletScript != null)
            {
                bulletScript.SetDirection(dir);
            }

            // Rotación visual opcional
            float angleToLook = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, angleToLook);

            bullet.SetActive(true);
        }
    }


    private void ShootCrossbow()
    {
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

            // Rotamos baseDir por "angle" grados
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);
            Vector2 dir = rot * baseDir;

            GameObject bullet = GetPooledBullet();
            if (bullet != null)
            {
                bullet.transform.position = firePoint.position;

                BulletScript bulletScript = bullet.GetComponent<BulletScript>();
                if (bulletScript != null)
                {
                    bulletScript.SetDirection(dir);
                }

                // Rotación visual opcional
                float angleToLook = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                bullet.transform.rotation = Quaternion.Euler(0f, 0f, angleToLook);

                bullet.SetActive(true);
            }
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
        GameManager.hasKnifePowerup = true;   // ⬅ se guarda entre escenas
    }

    public void ActivateCrossbowPowerup()
    {
        crossbowActive = true;
        GameManager.hasCrossbowPowerup = true; // ⬅ se guarda entre escenas
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
<<<<<<< Updated upstream
=======

    public void EnableControl()
    {
        this.enabled = true;
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
    }
>>>>>>> Stashed changes

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

<<<<<<< Updated upstream
=======
    private void PlayMusic()
    {
        SoundList.instance.PlaySound("Theme");
    }
>>>>>>> Stashed changes
}
