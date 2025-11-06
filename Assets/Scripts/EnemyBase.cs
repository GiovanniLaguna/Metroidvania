using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [SerializeField] protected int maxHealth = 1;
    [SerializeField] protected Animator animator;

    protected int health;

    public static System.Action<EnemyBase> OnAnyEnemyKilled;

    protected virtual void Awake()
    {
        health = maxHealth;
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    public virtual void TakeDamage(int dmg = 1)
    {
        health -= dmg;
        if (health <= 0) Die();
        else animator?.SetTrigger("Hurt");
    }

    protected virtual void Die()
    {
        animator?.SetTrigger("Die"); // si la tienes
        OnAnyEnemyKilled?.Invoke(this);
        // pequeña sacudida global
        CameraShakeCinemachine.Instance?.Shake(1.2f, 0.15f);
        Destroy(gameObject, 0.05f); // destrúyelo rápido
    }
}
