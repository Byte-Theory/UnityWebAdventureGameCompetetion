using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionDetector : MonoBehaviour
{
    [Header("PickUp")]
    [SerializeField] private LayerMask pickUpLayer;
    [SerializeField] private float pickUpRadius;
    [SerializeField] private Vector3 pickUpOffset;
    
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
    
    // Player
    private Player player;
    
    // Update is called once per frame
    void Update()
    {
        // Ground Detection
        CheckGround();
        UpdateCoyoteTimer();
        
        // Pick Up Detection
        CheckForPickUps();
    }

    #region SetUp

    internal void SetUp(Player player)
    {
        this.player = player;
        
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

    #region PickUp

    private void CheckForPickUps()
    {
        Vector3 center = transform.position + pickUpOffset;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(center, pickUpRadius, 
                                                    Vector2.one, 0, pickUpLayer);

        if (hits.Length == 0)
        {
            return;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            GameObject hitObject = hits[i].transform.gameObject;
            WeaponPickUp weaponPickUp = hitObject.GetComponent<WeaponPickUp>();

            if (weaponPickUp != null)
            {
                player.playerWeaponManager.OnWeaponPickup(weaponPickUp);
            }
        }
    }

    #endregion
    
    #region Gizmos

    private void OnDrawGizmos()
    {
        
        Gizmos.color = new Color(0, 0, 0, 0.5f);
        for (int i = 0; i < groundCheckPoints.Count; i++)
        {
            Vector3 point = transform.position + groundCheckPoints[i];
            
            Gizmos.DrawSphere(point, groundCheckRadius);
        }

        Gizmos.color = new Color(0.0f, 1.0f, 0.5f, 0.25f);
        Gizmos.DrawSphere(transform.position + pickUpOffset, pickUpRadius);
    }

    #endregion
}
