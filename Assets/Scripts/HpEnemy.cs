using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class HpEnemy : HpBase
{
    [Header("Visual")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    public const string DEATH = "Death";
    public const int TRANSPARENT_LAYER = 1;
    public const float VANISH_TIME = 1f;

    [Header("Death Settings")]
    [SerializeField] private float destroyDelay = 1.2f;

    // Para evitar que se ejecute la muerte varias veces
    private bool isDead = false;

    // Si quieres algo al ser golpeado pero no morir (flash, sonido, etc.)
    public UnityEvent onHitEvent;

    public override void RemoveHp(int amount)
    {
        // Si ya está muerto, ignoramos más daño
        if (isDead) return;

        base.RemoveHp(amount);

        // Feedback al recibir daño pero sigue vivo
        if (currentHp > 0)
        {
            onHitEvent?.Invoke();
            return;
        }

        // Si llega aquí, ya no tiene vida
        HandleDeath();
    }

    private void HandleDeath()
    {
        if (isDead) return;
        isDead = true;

        // Evento de muerte (lo puedes conectar al GameManager para sumar monedas, score, etc.)
        onDeathEvent?.Invoke();

        RemoveEnemy();
    }

    private void RemoveEnemy()
    {
        // Cambiamos de layer para que no vuelva a colisionar con el jugador / balas
        gameObject.layer = TRANSPARENT_LAYER;

        // Opcional: desactivar colliders para que no estorben
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
        {
            col.enabled = false;
        }

        // Animación de muerte
        animator?.SetTrigger(DEATH);

        // Fade out con DOTween y luego destruir el objeto
        if (spriteRenderer != null)
        {
            spriteRenderer
                .DOFade(0, VANISH_TIME)
                .SetDelay(VANISH_TIME)
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });
        }
        else
        {
            // Si no hay spriteRenderer, al menos destruimos después de un tiempo
            Destroy(gameObject, destroyDelay);
        }
    }
}
