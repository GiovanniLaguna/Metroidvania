using DG.Tweening.Core.Easing;
using UnityEngine;

public class PlayerHealthArmor : MonoBehaviour
{
    [Header("Armadura")]
    [SerializeField] private bool hasArmor = true;
    [SerializeField] private Sprite armoredSprite;
    [SerializeField] private Sprite underwearSprite;
    [SerializeField] private SpriteRenderer sr;

    [Header("Vida")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private int currentHealth = 5;

    // Evento opcional para UI: (vidaActual, vidaMax)
    public System.Action<int, int> OnHealthChanged;

    [Header("Anim")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController playerController;

    [Header("Feedback")]
    [SerializeField] private float hitFreezeDuration = 0.06f;
    [SerializeField] private float deathFreezeDuration = 0.25f;

    public System.Action OnPlayerDamaged;
    private bool IsDead = false;

    public bool HasArmor => hasArmor;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    void Reset()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
    }

    private void Awake()
    {
        // Primera vez que aparece un player en este "run":
        if (!GameManager.armorInitialized)
        {
            GameManager.hasArmor = hasArmor; // toma el valor del prefab
            GameManager.armorInitialized = true;
        }

        // Si por alguna razón currentHealth está en 0 en el inspector, lo igualamos
        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
        }
    }

    private void Start()
    {
        // Aplica el estado persistente de armadura
        hasArmor = GameManager.hasArmor;

        if (hasArmor)
        {
            if (sr && armoredSprite) sr.sprite = armoredSprite;
        }
        else
        {
            if (sr && underwearSprite) sr.sprite = underwearSprite;
        }

        // Notificar vida inicial a quien escuche (UI, etc.)
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void GainArmor()
    {
        hasArmor = true;
        GameManager.hasArmor = true; // ⬅ persistente

        if (sr && armoredSprite)
            sr.sprite = armoredSprite;
    }

    /// <summary>
    /// Recuperar vida (sin tocar armadura).
    /// </summary>
    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int dmg = 1)
    {
        if (IsDead) return;

        // Shake por daño
        CameraShakeCinemachine.Instance?.Shake(2.5f, 0.2f);

        OnPlayerDamaged?.Invoke();

        // 1️⃣ Primero se chequea la armadura
        if (hasArmor)
        {
            // Pierde armadura, pero no baja vida aún
            hasArmor = false;
            GameManager.hasArmor = false; // ⬅ persistente

            if (sr && underwearSprite)
                sr.sprite = underwearSprite;

            animator?.SetTrigger("IsHurt");
            return;
        }

        // 2️⃣ Ya no hay armadura → ahora sí baja la vida
        currentHealth = Mathf.Max(currentHealth - dmg, 0);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth > 0)
        {
            // Sigue vivo pero herido
            animator?.SetTrigger("IsHurt");
            
            return;
        }

        // 3️⃣ Vida llegó a 0 → muerte
        Die();
    }

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;

        animator?.SetTrigger("IsDead");

        // Avisar al Player y al GameManager
        playerController?.Death();
        GameManager.instance?.OnPlayerDeath();
    }
}
