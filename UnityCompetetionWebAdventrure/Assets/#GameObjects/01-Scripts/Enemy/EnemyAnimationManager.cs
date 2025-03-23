using UnityEngine;

public class EnemyAnimationManager : MonoBehaviour
{
    [Header("Animator")] 
    [SerializeField] private Animator anim;

    private readonly string AnimWalkTag = "Walk";
    private readonly string AnimRunTag = "Run";
    private readonly string AnimDieTag = "Die";
    
    private readonly string AnimAttack1Tag = "Attack1";
    private readonly string AnimAttack2Tag = "Attack2";
   
    #region Animations
    
    internal void UpdateAnimation(EnemyMovementStates enemyMovementStates)
    {
        bool isWalk = enemyMovementStates == EnemyMovementStates.Patrolling;
        bool isRun = enemyMovementStates == EnemyMovementStates.ChasingPlayer;
        bool isAttack1 = enemyMovementStates == EnemyMovementStates.Attacking;
        
        anim.SetBool(AnimWalkTag, isWalk);
        anim.SetBool(AnimRunTag, isRun);
        anim.SetBool(AnimRunTag, isAttack1);
    }

    internal void PlayDeathAnimation()
    {
        anim.SetTrigger(AnimDieTag);
    }
    
    #endregion
}
