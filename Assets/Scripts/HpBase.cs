using UnityEngine;
using UnityEngine.Events;

public class HpBase : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] protected int maxHp = 5;
    protected int currentHp;

    [Header("Events")]
    public UnityEvent onDeathEvent;

    protected GameManager gameManager;

    // ------------------------------
    //             INIT
    // ------------------------------
    protected virtual void Start()
    {
        currentHp = maxHp;
        gameManager = GameManager.instance;
    }

    // ------------------------------
    //            DAMAGE
    // ------------------------------
    public virtual void RemoveHp(int amount)
    {
        currentHp -= amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        if (currentHp <= 0)
        {
            HandleDeath();
        }
    }

    // ------------------------------
    //          CHECKS
    // ------------------------------
    public bool IsAlive()
    {
        return currentHp > 0;
    }

    // ------------------------------
    //            DEATH
    // ------------------------------
    protected virtual void HandleDeath()
    {
        if (onDeathEvent != null)
        {
            onDeathEvent.Invoke();
        }
    }
}
