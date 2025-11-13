using UnityEngine;

public class KnifePowerup : MonoBehaviour
{
    [SerializeField] private AudioClip pickupSfx;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ActivateKnifePowerup();

            if (pickupSfx != null)
            {
                AudioSource.PlayClipAtPoint(pickupSfx, transform.position);
            }

            Destroy(gameObject);
        }
    }
}
