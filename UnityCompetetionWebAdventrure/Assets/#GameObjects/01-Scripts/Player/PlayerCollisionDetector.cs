using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionDetector : MonoBehaviour
{
    [Header("Ground Detection")]
    [SerializeField] private bool isGround;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private List<Vector3> groundCheckPoints;
    public bool IsGrounded => isGround;
    
    // Update is called once per frame
    void Update()
    {
        CheckGround();
    }

    #region Ground Detection

    private void CheckGround()
    {
        bool isGrounded = false;
        
        for (int i = 0; i < groundCheckPoints.Count; i++)
        {
            Vector3 point = transform.position + groundCheckPoints[i];
            
            RaycastHit2D[] hits = Physics2D.CircleCastAll(point, groundCheckRadius, 
                                                    Vector2.one, 0, groundLayer);

            if (hits.Length > 0)
            {
                isGrounded = true;
            }
        }
        
        this.isGround = isGrounded;
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
