using UnityEngine;

public class EnemyAttackManager : MonoBehaviour
{
    [Header("Attack Pattern")]
    [SerializeField] private int[] attackPattern;
    
    [Header("Attack Durations")]
    [SerializeField] private Vector2 attackIdleDuration;
    [SerializeField] private float attackAnimDuration;
    [SerializeField] private float attackDelay;

    [Header("Damage Intensity")] 
    [SerializeField] private Vector2 enemyDamageRange;
    
    // Current Attack Data
    private int attackPatternIdx;
    private int continuousAttackCt;
    private int attackCt;
    private bool isAttackCompleted;

    private GroundedMeleeEnemy groundedMeleeEnemy;

    #region SetUp

    internal void SetUp(GroundedMeleeEnemy groundedMeleeEnemy)
    {
        this.groundedMeleeEnemy = groundedMeleeEnemy;
    }
    
    #endregion
    
    #region Attack Pattern

    internal void SetUpAttackPattern()
    {
        attackCt = 0;
        isAttackCompleted = false;
        
        continuousAttackCt = attackPattern[attackPatternIdx];
        
        attackPatternIdx++;

        if (attackPatternIdx >= attackPattern.Length)
        {
            attackPatternIdx = 0;
        }
    }

    internal void ResetAttackPattern()
    {
        attackPatternIdx = 0;
    }
    
    internal void SetUpAttack()
    {
        attackCt++;
        isAttackCompleted = false;
    }

    internal void TryDamageThePlayer(float attackTimeElapsed, Player player)
    {
        if (attackTimeElapsed > attackDelay && !isAttackCompleted)
        {
            isAttackCompleted = true;
            
            float distToPlayer = Vector3.Distance(player.transform.position, transform.position);
            float maxDistForValidDmg = groundedMeleeEnemy.groundedMeleeEnemySensor.GetMaxDistForValidDmg();
            if (distToPlayer <= maxDistForValidDmg)
            {
                float damageToGive = Random.Range(enemyDamageRange.x, enemyDamageRange.y);
                // TODO: do damage to player
            }
        }
    }
    
    #endregion

    #region Getter

    internal float GetAttackDuration()
    {
        return attackAnimDuration;
    }
    
    internal float GetAttackIdleDuration()
    {
        float dur = Random.Range(attackIdleDuration.x, attackIdleDuration.y);
        
        return dur;
    }

    internal bool CheckIfAttackCompleted()
    {
        return attackCt >= continuousAttackCt;
    }

    #endregion
}
