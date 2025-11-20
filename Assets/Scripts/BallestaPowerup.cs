using UnityEngine;

public class BallestaPowerup : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float duration = 10f;      // Cuánto dura el power up
    [SerializeField] private string playerTag = "Player";

    [Header("Feedback (Opcional)")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private ParticleSystem pickupParticles;
    [SerializeField] private bool destroyOnPickup = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("[BallestaPowerup] Trigger con: " + other.name);

        // 1) Checamos tag
        if (!other.CompareTag(playerTag))
        {
            Debug.Log("[BallestaPowerup] El objeto no tiene el tag de player (" + playerTag + ")");
            return;
        }

        // 2) Buscamos PlayerController en el objeto o en sus padres
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
        {
            player = other.GetComponentInParent<PlayerController>();
        }

        if (player == null)
        {
            Debug.LogWarning("[BallestaPowerup] No encontré PlayerController en " + other.name + " ni en sus padres.");
            return;
        }

        Debug.Log("[BallestaPowerup] PowerUp recogido por: " + player.name);

        // Activamos la ballesta por duración
        player.StartCoroutine(ActivateCrossbowTemporarily(player));

        PlayFeedback();

        if (destroyOnPickup)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator ActivateCrossbowTemporarily(PlayerController player)
    {
        player.ActivateCrossbowPowerup();
        yield return new WaitForSeconds(duration);
        player.ResetWeapon();
    }

    private void PlayFeedback()
    {
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        if (pickupParticles != null)
        {
            Instantiate(pickupParticles, transform.position, Quaternion.identity);
        }
    }
}
