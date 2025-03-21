using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionDetector : MonoBehaviour
{
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private List<Vector3> groundCheckPoints;
    private bool isGrounded;
    public bool IsGrounded => isGrounded;
    
    [Header("Grounded Coyote Time")]
    [SerializeField] private float coyoteDuration;
    private float coyoteTimeElapsed;
    private bool isCoyoteGrounded;
    
    // Update is called once per frame
    void Update()
    {
        CheckGround();
        UpdateCoyoteTimer();
    }

    #region SetUp

    internal void SetUp()
    {
        isGrounded = false;
        isCoyoteGrounded = false;
        coyoteTimeElapsed = 0.0f;
    }

    #endregion

    #region Ground Detection

    private void CheckGround()
    {
        bool isTempGrounded = false;
        
        for (int i = 0; i < groundCheckPoints.Count; i++)
        {
            Vector3 point = transform.position + groundCheckPoints[i];
            
            RaycastHit2D[] hits = Physics2D.CircleCastAll(point, groundCheckRadius, 
                                                    Vector2.one, 0, groundLayer);

            if (hits.Length > 0)
            {
                isTempGrounded = true;
            }
        }
        
        isCoyoteGrounded = isTempGrounded;

        if (isCoyoteGrounded)
        {
            isGrounded = true;
            coyoteTimeElapsed = 0;
        }
    }

    private void UpdateCoyoteTimer()
    {
        if (!isCoyoteGrounded)
        {
            coyoteTimeElapsed += Time.deltaTime;

            if (coyoteTimeElapsed >= coyoteDuration)
            {
                isGrounded = false;
            }
        }
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        
        for (int i = 0; i < groundCheckPoints.Count; i++)
        {
            Vector3 point = transform.position + groundCheckPoints[i];
            
            Gizmos.DrawSphere(point, groundCheckRadius);
        }
    }

    #endregion
}
