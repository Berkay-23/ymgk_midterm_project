using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rgb;
    Vector3 velocity;

    float speedAmount = 5.0f;
    float jumpAmount = 10f;

    public bool isGrounded;
    public Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            UnityEngine.Debug.Log("true");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rgb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        velocity = new Vector3(Input.GetAxis("Horizontal"), 0f);
        transform.position += velocity * speedAmount * Time.deltaTime;
        
        animator.SetFloat("Speed", Mathf.Abs(velocity.x));


        if (Input.GetButtonDown("Jump") && Mathf.Approximately(rgb.velocity.y, 0.0f))
        {
            rgb.AddForce(Vector3.up * jumpAmount, ForceMode2D.Impulse);
            animator.SetBool("Jumping", true);
        }

        if(animator.GetBool("Jumping") == true && Mathf.Approximately(rgb.velocity.y, 0.0f))
        {
            animator.SetBool("Jumping", false);
        }

        if (Input.GetAxisRaw("Horizontal") == -1)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (Input.GetAxisRaw("Horizontal") == 1)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

    }
}