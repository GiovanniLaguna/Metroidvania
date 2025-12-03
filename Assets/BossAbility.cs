using UnityEngine;
using System.Collections;
using System.Collections.Concurrent;
using JetBrains.Annotations;

public class BossAbility : MonoBehaviour
{
    [SerializeField] private int Damage;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private Transform boxPosition;
    [SerializeField] private float Lifetime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, Lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Hit()
    {
        Collider2D[] objetos = Physics2D.OverlapBoxAll(boxPosition.position, boxSize, 0f);
        foreach (Collider2D obj in objetos)
        {
            if (obj.CompareTag("Player"))
            {
                obj.GetComponent<PlayerHealthArmor>().TakeDamage(Damage);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boxPosition.position, boxSize);
    }
}
