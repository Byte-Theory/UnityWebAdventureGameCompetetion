using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    public Vector2 moveInput;
    
    [Header("Movement Data")]
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed;
    private float moveSpeed;
    public float moveSpeedChangeSpeed;
    private Vector2 rbLinearVelocity;
    
    [Header("Jump")] 
    [SerializeField] private float jumpForce;
    private bool jump;
    private bool isJumping;

    [Header("Ground Check")] 
    private bool isGrounded;
    
    // Rigidbody
    private Rigidbody2D rb;
    
    //Ref
    private UserInput userInput;
    
    // Update is called once per frame
    void Update()
    {
        // Input
        SetUserInput();
        
        // Move
        UpdateMoveSpeed();
        
        // Jump
        Jump();
    }

    private void FixedUpdate()
    {
        Move();
    }

    #region SetUp

    internal void SetUp()
    {
        rb = GetComponent<Rigidbody2D>();
        
        userInput = UserInput.Instance;
    }

    #endregion

    #region Input

    private void SetUserInput()
    {
        moveInput = userInput.GetMoveInput();
        
        jump = userInput.GetJumpInput();
    }

    #endregion

    #region Move

    private void UpdateMoveSpeed()
    {
        moveSpeed = moveInput.x * runSpeed;
    }

    private void Move()
    {
        rbLinearVelocity.x = moveSpeed;
        rbLinearVelocity.y = rb.linearVelocityY;
        
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, rbLinearVelocity, Time.fixedDeltaTime * moveSpeedChangeSpeed);
    }

    #endregion
    
    #region Jump

    private void Jump()
    {
        if (jump && isGrounded && !isJumping)
        {
            isJumping = true;
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }

    #endregion
}
