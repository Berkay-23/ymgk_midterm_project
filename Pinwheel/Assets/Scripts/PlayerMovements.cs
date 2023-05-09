using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovements : MonoBehaviour
{
    Rigidbody2D rigidBody;
    Vector3 velocity;
    public Collider2D playerCollider;
    float speedAmount = 5.0f;
    float jumpAmount = 6f;

    public int score;
    public Vector3 respawnPoint;

    public Animator animator;
    public bool isMoveEnabled;
    void Start()
    {
        score = 0;
        respawnPoint = new Vector3(-28f, -7f, 0f); // Oyuncunun pozisyonunu (-28,-7,0) olarak ayarla
        rigidBody = GetComponent<Rigidbody2D>();
        isMoveEnabled = true;
    }
    void Update()
    {
        if (isMoveEnabled)
        {
            PlayerMovementController();
            RotatePlayerAxis();
            ChangeColliders();
            PlayerDeathCheck();
        }
    }
    private void PlayerMovementController()
    {
        float moveInput = Input.GetAxis("Horizontal");

        velocity = new Vector3(moveInput, 0f);
        transform.position += velocity * speedAmount * Time.deltaTime;
        animator.SetFloat("Speed", Mathf.Abs(velocity.x));

        if (Input.GetButtonDown("Jump") && !animator.GetBool("IsJumping"))
        {
            rigidBody.AddForce(Vector3.up * jumpAmount, ForceMode2D.Impulse);
            animator.SetBool("IsJumping", true);
        }
    }

    private void RotatePlayerAxis()
    {
        if (Input.GetAxisRaw("Horizontal") == -1)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        else if (Input.GetAxisRaw("Horizontal") == 1)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
            animator.SetBool("IsJumping", false);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "tilemap")
            animator.SetBool("IsJumping", true);
    }

    private void ChangeColliders()
    {
        float jumpInput = rigidBody.velocity.y;

        if (jumpInput > 0)
            playerCollider.enabled = false;
        else
            playerCollider.enabled = true;
    }

    private void PlayerDeathCheck()
    {
        if (transform.position.y < -25)
        {
            transform.position = respawnPoint;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero; // Oyuncunun hýzýný sýfýrla
        }
    }
}