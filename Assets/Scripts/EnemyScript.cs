using UnityEngine;

public class EnemyScript : MonoBehaviour
{

    private Rigidbody2D rb;
    [SerializeField]
    private float speed;
    [SerializeField]
    private Transform foot;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private GameObject Drop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsGrounded())
        {
            speed *= -1;
           transform.localScale = new Vector3(transform.localScale.x *-1, transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnEnable()
    {
        
    }

    //private void OnDisable()
    //{
    //    Instantiate(Drop, transform.position,Quaternion.identity);
    //}

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(foot.position, .2f, groundLayer);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(speed, 0);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        { Destroy(this.gameObject); }
    }
}
