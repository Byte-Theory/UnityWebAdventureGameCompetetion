using System;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemySensor : MonoBehaviour
{
    [Header("Player Detection")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float playerDetectionRadius;
    [SerializeField] private Vector3 playerDetectionOffset;
    
    // Enemy
    private GroundedEnemy groundedEnemy;

    private Player detectedPlayer;

    private void Update()
    {
        CheckForPlayerInRange();
        CheckInPlayerIsOutOfRange();
    }

    #region SetUp

    internal void SetUp(GroundedEnemy groundedEnemy)
    {
        this.groundedEnemy = groundedEnemy;
    }

    #endregion
    
    #region Player

    private void CheckForPlayerInRange()
    {
        Vector3 center = transform.position + playerDetectionOffset;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(center, playerDetectionRadius, 
            Vector2.one, 0, playerLayer);

        if (hits.Length == 0)
        {
            return;
        }

        Player player = hits[0].transform.GetComponent<Player>();

        detectedPlayer = player;
        groundedEnemy.PlayerDetected(player);
    }

    private void CheckInPlayerIsOutOfRange()
    {
        if (detectedPlayer == null)
        {
            return;
        }
        
        float dist = Vector3.Distance(transform.position, detectedPlayer.transform.position);

        if (dist > playerDetectionRadius)
        {
            detectedPlayer = null;
            groundedEnemy.PlayerDetected(null);
        }
    }

    #endregion
    
    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.2f, 0.2f);
        Gizmos.DrawWireSphere(transform.position + playerDetectionOffset, playerDetectionRadius);
    }

    #endregion
}
