using UnityEngine;

public class EnemyAttackManager : MonoBehaviour
{
    [Header("Attack Pattern")]
    [SerializeField] private int[] attackPattern;
    
    [Header("Attack Durations")]
    [SerializeField] private Vector2 attackIdleDuration;
    [SerializeField] private float attackAnimDuration;
    [SerializeField] private float attackDelay;
    
    // Current Attack Data
    private int attackPatternIdx;
    private int continuousAttackCt;
    private int attackCt;
    private bool isAttackCompleted;

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

    internal void TryDamageThePlayer(float attackTimeElapsed)
    {
        if (attackTimeElapsed > attackDelay && !isAttackCompleted)
        {
            isAttackCompleted = true;
            // TODO: do damage to player
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
