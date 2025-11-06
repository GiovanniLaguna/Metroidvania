using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed;
    private bool dir;
    [SerializeField]
    private float MaxTime;

    private float currentTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

   public void SetSpeed(float s)
    { this.speed = s; }

 public   float GetSpeed () { return this.speed; }

    void Start()

    {
        rb = GetComponent<Rigidbody2D>();
        speed = 15;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > MaxTime) 
        {
        currentTime = 0;
            gameObject.SetActive(false);
        }
        rb.AddForce(new Vector2(speed * 1, 0));
    }
    private void FixedUpdate()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //var enemy = collision.GetComponent<EnemyBase>();
        /*if (enemy)
        {
            enemy.TakeDamage(1);
            gameObject.SetActive(false); // devuelve bala al pool
        }*/

        gameObject.SetActive(false);
   }
}
