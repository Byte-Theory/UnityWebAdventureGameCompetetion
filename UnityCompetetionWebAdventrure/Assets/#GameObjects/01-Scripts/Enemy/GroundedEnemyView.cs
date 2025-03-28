using UnityEngine;

public class GroundedEnemyView : MonoBehaviour
{
    [Header("Child References")] 
    [SerializeField] private GameObject spriteGo;
    
    // Look Dir
    private Vector3 lookScale; 
    
    // Grounded Enemy
    private GroundedMeleeEnemy _groundedMeleeEnemy;

    private void Update()
    {
        UpdateLookDir();
    }

    #region SetUp

    internal void SetUp(GroundedMeleeEnemy groundedMeleeEnemy)
    {
        this._groundedMeleeEnemy = groundedMeleeEnemy;
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
        EnemyStates enemyStates = _groundedMeleeEnemy.GetEnemyMovementState();

        if (enemyStates == EnemyStates.Idle)
        {
            return;
        }
        
        Vector2 moveDir = _groundedMeleeEnemy.GetLookAtDirection();

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
