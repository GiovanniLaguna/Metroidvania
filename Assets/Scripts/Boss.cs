using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : MonoBehaviour
{
    private Animator animator;
    public Rigidbody2D rb;
    [SerializeField] public Transform player;
    private bool isFacingRight = true;
    [Header("Boss Stats")]
    [SerializeField] private float life;
    //[SerializeField] private LifeBar lifeBar;
    [Header("Attack")]
    [SerializeField] private Transform attackController;
    [SerializeField] private float attackRadius;
    [SerializeField] private int Damage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    public void TakeDamage(float damage)
    {
        life -= damage;
        //lifeBar.SetHealth(life);
        if (life <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        animator.SetTrigger("Death");
        // Desactivar el objeto después de la animación de muerte
        Destroy(gameObject, 1f); // Ajusta el tiempo según la duración de la animación
        GameManager.instance.GameWon();
    }

    public void LookAtPlayer()
    {
        if (player == null) return;

        float dir = player.position.x - transform.position.x;

        // Si el jugador está a la derecha → mira a la derecha
        if (dir > 0 && transform.localScale.x < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        // Si el jugador está a la izquierda → mira a la izquierda
        else if (dir < 0 && transform.localScale.x > 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
    // Update is called once per frame
    void Update()
    {
        float PlayerDistance = Vector2.Distance(transform.position, player.position);
        animator.SetFloat("PlayerDistance", PlayerDistance);
    }

    public void Attack()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(attackController.position, attackRadius);
        foreach (Collider2D obj in objects)
        {
            if (obj.CompareTag("Player"))
            {
                obj.GetComponent<PlayerHealthArmor>().TakeDamage(Damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackController.position, attackRadius);
    }
}
