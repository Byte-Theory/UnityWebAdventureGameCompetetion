using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GroundedMeleeEnemySensor : MonoBehaviour
{
    [Header("Player Detection")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float playerDetectionRadius;
    [SerializeField] private Vector3 playerDetectionOffset;
    
    [Header("Stopping Distance")]
    [SerializeField] private float stoppingDistance;
    [SerializeField] private float maxDistForPlayerDmg;
    
    // Enemy
    private GroundedMeleeEnemy groundedMeleeEnemy;

    private Player detectedPlayer;

    private void Update()
    {
        CheckForPlayerInRange();
        CheckInPlayerIsOutOfRange();
    }

    #region SetUp

    internal void SetUp(GroundedMeleeEnemy groundedMeleeEnemy)
    {
        this.groundedMeleeEnemy = groundedMeleeEnemy;
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
        groundedMeleeEnemy.PlayerDetected(player);
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
            groundedMeleeEnemy.PlayerDetected(null);
        }
    }

    #endregion

    #region Getter

    internal float GetStoppingDistance()
    {
        return stoppingDistance;
    }
    
    internal float GetMaxDistForValidDmg()
    {
        return maxDistForPlayerDmg;
    }

    #endregion
    
    #region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.25f, 0.25f, 0.75f);
        Gizmos.DrawWireSphere(transform.position + playerDetectionOffset, playerDetectionRadius);
        
        Gizmos.color = new Color(1.0f, 0.2f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
        
        Gizmos.color = new Color(0.5f, 0.75f, 0.2f);
        Gizmos.DrawWireSphere(transform.position + playerDetectionOffset, maxDistForPlayerDmg);
    }

    #endregion
}
