using UnityEngine;

public class CrossbowPowerup : MonoBehaviour
{
    [Header("Feedback")]
    [SerializeField] private AudioClip pickupSfx;

    [Header("Duración del power up")]
    [SerializeField] private float duration = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 🔎 Intentamos obtener el player directamente
        PlayerController player = other.GetComponent<PlayerController>();

        // Por si el collider está en un hijo del player
        if (player == null)
        {
            player = other.GetComponentInParent<PlayerController>();
        }

        if (player != null)
        {
            // Activa la ballesta temporalmente
            player.StartCoroutine(ActivateCrossbowTemporarily(player));

            if (pickupSfx != null)
            {
                AudioSource.PlayClipAtPoint(pickupSfx, transform.position);
            }

            Destroy(gameObject); // Destruimos el power up tras recogerlo
        }
    }

    private System.Collections.IEnumerator ActivateCrossbowTemporarily(PlayerController player)
    {
        player.ActivateCrossbowPowerup();   // Activa ballesta
        yield return new WaitForSeconds(duration);
        player.ResetWeapon();               // Regresa al arma normal
    }
}
