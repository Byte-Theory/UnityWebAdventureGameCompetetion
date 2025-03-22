using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Rig Follow Data")] 
    [SerializeField] private float followSpeedX;
    [SerializeField] private float followSpeedY;
    [SerializeField] private Vector3 rigFollowOffset;
    private float refRigVelY;
    private Vector3 rigPos;
    
    [Header("Pivot")]
    [SerializeField] private Transform pivotT;
    [SerializeField] private float pivotChaneSpeed;
    [SerializeField] private Vector3 pivotOffsetCenter;
    [SerializeField] private Vector3 pivotOffsetRight;
    [SerializeField] private Vector3 pivotOffsetLeft;
    private Vector3 pivotOffset;
    
    // Player
    private Player player;
    private Transform playerT;

    private void Update()
    {
        UpdatePivotOffset();
        UpdatePivotPos();
    }

    void LateUpdate()
    {
        RigFollow();
    }

    #region Set Up

    internal void SetUp(Player player)
    {
        this.player = player;
        playerT = player.transform;

        SetUpRigFollow();
    }

    #endregion

    #region Rig

    private void SetUpRigFollow()
    {
        Vector3 rigDestination = playerT.position + rigFollowOffset;
        
        rigPos = rigDestination;
        transform.position = rigPos;
    }

    private void RigFollow()
    {
        Vector3 rigDestination = playerT.position + rigFollowOffset;
        
        rigPos.x = Mathf.Lerp(rigPos.x, rigDestination.x, Time.deltaTime * followSpeedX);
        rigPos.y = Mathf.SmoothDamp(rigPos.y, rigDestination.y, 
            ref refRigVelY, Time.deltaTime * followSpeedY);
        rigPos.z = rigDestination.z;
        
        transform.position = rigPos;
    }

    #endregion

    #region Pivot

    private void UpdatePivotOffset()
    {
        Vector2 playerSpeed = player.playermovement.GetRbVelocity();

        if (playerSpeed.x > 0)
        {
            pivotOffset = pivotOffsetRight;
        }
        else if (playerSpeed.x < 0)
        {
            pivotOffset = pivotOffsetLeft;
        }
        else
        {
            pivotOffset = pivotOffsetCenter;
        }
    }

    private void UpdatePivotPos()
    {
        pivotT.transform.localPosition = Vector3.Lerp(pivotT.transform.localPosition, 
                                                        pivotOffset, Time.deltaTime * pivotChaneSpeed);
    }
    
    #endregion
}
