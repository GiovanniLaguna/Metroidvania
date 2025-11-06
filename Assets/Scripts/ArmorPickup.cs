using UnityEngine;

public class ArmorPickup : MonoBehaviour
{
    [SerializeField] private AudioClip pickupSfx;
    [SerializeField] private ParticleSystem pickupFx;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var armor = other.GetComponentInParent<PlayerHealthArmor>();
        if (armor == null) return;

        armor.GainArmor();
        CameraShakeCinemachine.Instance?.Shake(0.8f, 0.12f); // feedback leve

        if (pickupSfx) AudioSource.PlayClipAtPoint(pickupSfx, transform.position);
        if (pickupFx) Instantiate(pickupFx, transform.position, Quaternion.identity);

        gameObject.SetActive(false);
    }
}
