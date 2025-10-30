using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
   private Rigidbody2D rb;
    private float speed;
    private bool dir = true;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private Transform foot;

    [SerializeField]
    private GameObject bullet;

    [SerializeField]
    private List <GameObject> Bullets = new List<GameObject>();

    [SerializeField]
    private Transform gun;
    private float direcction;

    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = 5;
        jumpForce = 4;
        for (int i = 0; i < 5; i++) 
        {
         GameObject clone = Instantiate(bullet,Vector3.zero, Quaternion.identity);
            clone.SetActive(false);
            Bullets.Add(clone);
        }
    }

    // Update is called once per frame
    void Update()
    {
        direcction = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump")&& isGrounded() == true)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
           if (rb.linearVelocity.x >= 5)
            {
                rb.linearVelocityX = 5;
            }

           else if (rb.linearVelocity.x <= -5)
            {
                rb.linearVelocityX = 5;
            }

        }
        if(Input.GetButtonDown("Fire1"))
        {
            GameObject currentbullet = Getbullet();
            if (currentbullet != null)
            {
                if (dir == true)

                {
                    currentbullet.GetComponent<BulletScript>().SetSpeed(10);
                }
                else if (dir == false) 
                {
                currentbullet.GetComponent <BulletScript>().SetSpeed(-10);
                }

                currentbullet.transform.position = gun.position;
                currentbullet.SetActive(true);
            }
     
        
        }

        if (direcction < 0)
        {
            transform.localScale = new Vector3(-1, 1, 0);
            dir = false;
        }

        else if (direcction > 0)
        {
            transform.localScale = new Vector3(1, 1, 0);
            dir = true;
        }
    }

    GameObject Getbullet() 
    {
    foreach (GameObject b in Bullets) 
        {
        if (!b.activeInHierarchy)
            {
                return b;
            }    
        }
    GameObject clone = Instantiate(bullet, Vector3.zero,Quaternion.identity);
        clone.SetActive(false);
        Bullets.Add (clone);
    return clone;
    }

    bool isGrounded()
    {
        return Physics2D.OverlapCircle(foot.position, .1f);

    }
    
   
    private void FixedUpdate()
    {
      float xdir = Input.GetAxis("Horizontal");
      rb.AddForce(new Vector2(xdir * speed,0));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("DeathZone"))
            {
            GameManager.instance.playerLifes--;
            transform.position = Vector3.zero;
        }
    }
}
