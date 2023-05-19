using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerMovements : MonoBehaviour
{
    Rigidbody2D rigidBody;
    Vector3 velocity;
    public Collider2D playerCollider;
    float moveInput;
    float speedAmount = 5.0f;
    float jumpAmount = 6.0f;

    public int score;
    public Vector3 respawnPoint;

    public Animator animator;
    public bool isMoveEnabled;


    PlayerControls controls;
    private void Awake()
    {
        controls = new PlayerControls();
        OnEnable();

        controls.Land.Move.performed += ctx => moveInput = ctx.ReadValue<float>();
        controls.Land.Jump.performed += ctx =>
        {
            if (!animator.GetBool("IsJumping"))
            {
                rigidBody.AddForce(Vector3.up * jumpAmount, ForceMode2D.Impulse);
                animator.SetBool("IsJumping", true);
            }
        };
        OnDisable();
    }
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
        //moveInput = Input.GetAxis("Horizontal");
        
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
        if (moveInput == -1)
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        else if (moveInput == 1)
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

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
}