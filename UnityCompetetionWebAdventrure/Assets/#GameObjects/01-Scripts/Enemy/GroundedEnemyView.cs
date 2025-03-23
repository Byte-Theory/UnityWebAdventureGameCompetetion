using UnityEngine;

public class GroundedEnemyView : MonoBehaviour
{
    [Header("Child References")] 
    [SerializeField] private GameObject spriteGo;
    
    // Look Dir
    private Vector3 lookScale; 
    
    // Grounded Enemy
    private GroundedEnemy groundedEnemy;

    private void Update()
    {
        UpdateLookDir();
    }

    #region SetUp

    internal void SetUp(GroundedEnemy groundedEnemy)
    {
        this.groundedEnemy = groundedEnemy;
        SetUpLookDir();
    }

    #endregion

    #region Look Dir

    private void SetUpLookDir()
    {
        lookScale = new Vector3(1, 1, 1);
        spriteGo.transform.localScale = lookScale;
    }

    private void UpdateLookDir()
    {
        EnemyMovementStates enemyMovementStates = groundedEnemy.GetEnemyMovementState();

        if (enemyMovementStates == EnemyMovementStates.Idle)
        {
            return;
        }
        
        Vector2 moveDir = groundedEnemy.GetMoveDirection();

        if (moveDir.x > 0)
        {
            lookScale.x = 1;
        }
        else if (moveDir.x < 0)
        {
            lookScale.x = -1;
        }
        
        spriteGo.transform.localScale = lookScale;
    }

    #endregion
}
