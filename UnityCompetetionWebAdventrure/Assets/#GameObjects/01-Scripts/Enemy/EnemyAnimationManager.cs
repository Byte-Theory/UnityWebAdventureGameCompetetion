using UnityEngine;

public class EnemyAnimationManager : MonoBehaviour
{
    [Header("Animator")] 
    [SerializeField] private Animator anim;

    private readonly string AnimWalkTag = "Walk";
    private readonly string AnimRunTag = "Running";
    private readonly string AnimDieTag = "Die";
    
    private readonly string AnimAttack1Tag = "Attack1";
    private readonly string AnimAttack2Tag = "Attack2";
   
    #region Animations
    
    internal void UpdateAnimation(EnemyStates enemyStates)
    {
        bool isWalk = enemyStates == EnemyStates.Patrolling;
        bool isRun = enemyStates == EnemyStates.ChasingPlayer;
        bool isAttack1 = enemyStates == EnemyStates.Attacking;
        
        anim.SetBool(AnimWalkTag, isWalk);
        anim.SetBool(AnimRunTag, isRun);
        anim.SetBool(AnimAttack1Tag, isAttack1);
    }

    internal void PlayDeathAnimation()
    {
        anim.SetTrigger(AnimDieTag);
    }
    
    #endregion
}
