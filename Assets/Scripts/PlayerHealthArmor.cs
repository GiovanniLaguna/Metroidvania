using UnityEngine;

public class PlayerHealthArmor : MonoBehaviour
{
    [Header("Armadura")]
    [SerializeField] private bool hasArmor = true;
    [SerializeField] private Sprite armoredSprite;
    [SerializeField] private Sprite underwearSprite;
    [SerializeField] private SpriteRenderer sr;

    [Header("Anim")]
    [SerializeField] private Animator animator; // usa "IsHurt" trigger, "IsDead" trigger (opcional)

    public System.Action OnPlayerDamaged;

    public bool HasArmor => hasArmor;

    void Reset()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    public void GainArmor()
    {
        hasArmor = true;
        if (sr && armoredSprite) sr.sprite = armoredSprite;
        // podrías poner una pequeña anim o efecto aquí
    }

    public void TakeDamage(int dmg = 1)
    {
        // Shake por daño
        CameraShakeCinemachine.Instance?.Shake(2.5f, 0.2f);
        OnPlayerDamaged?.Invoke();

        if (hasArmor)
        {
            hasArmor = false;
            if (sr && underwearSprite) sr.sprite = underwearSprite;
            animator?.SetTrigger("IsHurt");
        }
        else
        {
            Die();
        }
    }

    public void Die()
    {
        animator?.SetTrigger("IsDead"); // si aún no tienes anim, no pasa nada
        // TODO: bloquear input / recargar escena, etc.
        // UnityEngine.SceneManagement.SceneManager.LoadScene(
        //     UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
