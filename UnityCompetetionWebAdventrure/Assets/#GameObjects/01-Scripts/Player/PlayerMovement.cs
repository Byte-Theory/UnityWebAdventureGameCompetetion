using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private bool isWalking;
    
    [Header("Movement Data")]
    [SerializeField] private float runSpeed;
    [SerializeField] private float walkSpeed;
    private float moveSpeed;
    public float moveSpeedChangeSpeed;
    private Vector2 rbLinearVelocity;

    [Header("Dash Data")] 
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldownDuration;
    private float dashTimeElapsed;
    private bool isDashTriggered;
    private bool isDashActive;
    private bool isDashOnCooldown;
    
    [Header("Jump")] 
    [SerializeField] private float jumpForce;
    [SerializeField] private float minJumpDuration;
    [SerializeField] private float jumpBufferDuration;
    private float jumpTimeElapsed;
    private float jumpBufferTimeElapsed;
    private bool jump;
    private bool isJumpInBuffer;
    private bool isJumping;
    private bool canResetJump;

    [Header("Falling")] 
    [SerializeField] private float fallSpeedIncrementFac;

    // Ground Check 
    private bool isGrounded;
    
    // Rigidbody
    private Rigidbody2D rb;
    
    // Player
    private Player player;
    
    //Ref
    private UserInput userInput;
    
    // Update is called once per frame
    void Update()
    {
        // Input
        SetUserInput();
        
        // Move Speed
        UpdateMoveSpeed();
        
        // Dash
        CheckForDash();
        UpdateDashTimeElapsed();
        UpdateDashCooldown();
        
        // Jump
        TriggerJump();
        UpdateJumpBufferDuration();
        UpdateJumpDuration();
        
        // Ground
        SetGrounded();
    }

    private void FixedUpdate()
    {
        // Move
        Move();
        
        // Jump
        Jump();
    }

    #region SetUp

    internal void SetUp(Player player)
    {
        rb = GetComponent<Rigidbody2D>();
        
        userInput = UserInput.Instance;

        this.player = player;
    }

    #endregion

    #region Input

    private void SetUserInput()
    {
        Vector2 input = userInput.GetMoveInput();

        if (input.x > 0)
        {
            moveInput.x = 1.0f;
        }
        else if (input.x < 0)
        {
            moveInput.x = -1.0f;
        }
        else
        {
            moveInput.x = 0.0f;
        }
        
        if (input.y > 0)
        {
            moveInput.y = 1.0f;
        }
        else if (input.y < 0)
        {
            moveInput.y = -1.0f;
        }
        else
        {
            moveInput.y = 0.0f;
        }
        
        jump = moveInput.y > 0;

        isWalking = userInput.GetWalkInput();
        isDashTriggered = userInput.GetDashInput();

        bool isAttacking = player.playerWeaponManager.GetIsAttacking();

        if (isAttacking && isGrounded)
        {
            moveInput.x = 0;
        }
    }

    #endregion

    #region Move

    private void UpdateMoveSpeed()
    {
        if (isDashActive)
        {
            moveSpeed = moveInput.x * dashSpeed;
        }
        else 
        {
            if (isWalking)
            {
                moveSpeed = moveInput.x * walkSpeed;
            }
            else
            {
                moveSpeed = moveInput.x * runSpeed;   
            }
        }
    }

    private void Move()
    {
        rbLinearVelocity.x = moveSpeed;
        rbLinearVelocity.y = rb.linearVelocityY;

        if (rbLinearVelocity.y < 0)
        {
            rbLinearVelocity.y -= Time.deltaTime * fallSpeedIncrementFac;
        }
        
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, rbLinearVelocity, Time.fixedDeltaTime * moveSpeedChangeSpeed);
    }

    #endregion

    #region Dash

    private void CheckForDash()
    {
        if (isDashTriggered && !isDashActive && !isDashOnCooldown)
        {
            isDashActive = true;
            dashTimeElapsed = 0.0f;
        }
    }

    private void UpdateDashTimeElapsed()
    {
        if (isDashActive)
        {
            dashTimeElapsed += Time.deltaTime;

            if (dashTimeElapsed >= dashDuration)
            {
                dashTimeElapsed = 0.0f;
                isDashActive = false;
                isDashOnCooldown = true;
            }
        }
    }
    
    private void UpdateDashCooldown()
    {
        if (isDashOnCooldown)
        {
            dashTimeElapsed += Time.deltaTime;

            if (dashTimeElapsed >= dashCooldownDuration)
            {
                dashTimeElapsed = 0.0f;
                isDashOnCooldown = false;
            }
        }
    }

    internal bool GetDashingActive()
    {
        return isDashActive;
    }
    
    #endregion
    
    #region Jump

    private void TriggerJump()
    {
        if (jump)
        {
            isJumpInBuffer = true;
            jumpBufferTimeElapsed = 0.0f;
        }
    }
    
    private void Jump()
    {
        if (isJumpInBuffer && isGrounded && !isJumping)
        {
            isJumping = true;
            jumpTimeElapsed = 0.0f;
            canResetJump = false;
            
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Force);
        }
    }

    // CanReset Jump makes sure that jump is reset only after a certain time is passed.
    // This is added because after adding coyote time the ground check remains true for some time even after player has
    // left the ground. And this check being true is resetting the jump and which in turn apply the jump force again.
    private void ResetJumping()
    {
        if (canResetJump)
        {
            canResetJump = false;
            isJumping = false;
        }
    }

    private void UpdateJumpBufferDuration()
    {
        if (isJumpInBuffer)
        {
            jumpBufferTimeElapsed += Time.deltaTime;

            if (jumpBufferTimeElapsed >= jumpBufferDuration)
            {
                isJumpInBuffer = false;
            }
        }
    }
    
    private void UpdateJumpDuration()
    {
        if (isJumping && !canResetJump)
        {
            jumpTimeElapsed += Time.deltaTime;

            if (jumpTimeElapsed >= minJumpDuration)
            {
                canResetJump = true;
            }
        }
    }
    
    #endregion

    #region Ground Detection

    private void SetGrounded()
    {
        isGrounded = player.playerCollisionDetector.IsGrounded;

        if (isGrounded)
        {
            ResetJumping();
        }
    }

    #endregion

    #region Physics

    internal Vector2 GetRbVelocity()
    {
        return rb.linearVelocity;
    }

    #endregion
}
