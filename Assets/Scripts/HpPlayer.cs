using UnityEngine;
using UnityEngine.Events;

public class HpPlayer : HpBase
{
    [Header("Player Events")]
    [SerializeField] private UnityEvent<int> onStartEvent;
    [SerializeField] private UnityEvent<int> onModifyHpEvent;

    [SerializeField, Space(10)]
    private UnityEvent onDieEvent;

    [Header("Feedback")]
    [SerializeField] private float hitFreezeDuration = 0.06f;
    [SerializeField] private float deathFreezeDuration = 0.25f;

    [Header("References")]
    [SerializeField] private PlayerController playerController;

    // ------------------------------
    //             INIT
    // ------------------------------
    protected override void Start()
    {
        base.Start();
        onStartEvent?.Invoke(maxHp);
    }

    // ------------------------------
    //            DAMAGE
    // ------------------------------
    public override void RemoveHp(int amount)
    {
        base.RemoveHp(amount);

        // Notificar cambio de HP
        onModifyHpEvent?.Invoke(currentHp);

        // Shake de cámara
        CameraManager.instance?.ShakeCamera();

        // Freeze frame al recibir daño
        if (TimeManager.instance != null)
        {
            float duration = currentHp <= 0 ? deathFreezeDuration : hitFreezeDuration;
            TimeManager.instance.FreezeFrame(duration, 1f);
        }

        // Si ya no queda vida, procesar muerte
        if (currentHp <= 0)
        {
            HandlePlayerDeath();
        }
    }

    // ------------------------------
    //           DEATH FLOW
    // ------------------------------
    private void HandlePlayerDeath()
    {
        playerController?.Death();
        gameManager?.GameOver();
        onDieEvent?.Invoke();
    }
}
