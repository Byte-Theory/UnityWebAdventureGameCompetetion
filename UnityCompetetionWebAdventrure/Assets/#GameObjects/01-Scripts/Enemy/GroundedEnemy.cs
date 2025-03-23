using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GroundedEnemy : MonoBehaviour
{
    [Header("Enemy Type")] 
    [SerializeField] private EnemyType enemyType;
    
    [Header("States")] 
    [SerializeField] private EnemyAgroState enemyAgroState = EnemyAgroState.Unknown;
    [SerializeField] private EnemyMovementStates enemyMovementStates = EnemyMovementStates.Unknown;
    private float moveStateTimeElapsed;
    private float moveStateDuration;

    private Vector3 stateStartPos;
    private Vector3 stateEndPos;

    [Header("Movement Data")] 
    [SerializeField] private Vector2 idleDuration;
    [SerializeField] private float patrollingSpeed;
    [SerializeField] private Vector2 looAtPlayerDuration;
    [SerializeField] private float chasingSpeed;
    
    [Header("Patrolling Pts")]
    [SerializeField] private List<Transform> patrolPointTransforms;
    private List<Vector3> patrolPoints;
    
    //Ref
    private GroundedEnemyView groundedEnemyView;
    private EnemyAnimationManager enemyAnimationManager;

    private void Start()
    {
        SetUp();
    }

    private void Update()
    {
        UpdateMoveStateTimer();
    }

    #region SetUp

    internal void SetUp()
    {
        groundedEnemyView = GetComponent<GroundedEnemyView>();
        enemyAnimationManager = GetComponent<EnemyAnimationManager>();
        
        groundedEnemyView.SetUp(this);
        
        patrolPoints = new List<Vector3>();
        for (int i = 0; i < patrolPointTransforms.Count; i++)
        {
            patrolPoints.Add(patrolPointTransforms[i].position);
        }
        
        SetEnemyAgroState(EnemyAgroState.Calm);
        SetEnemyMovementState(EnemyMovementStates.Idle);
    }

    #endregion
    
    #region Agro State

    internal EnemyAgroState GetEnemyAgroState()
    {
        return enemyAgroState;
    }
    
    private void SetEnemyAgroState(EnemyAgroState newState)
    {
        if (enemyAgroState == newState)
        {
            return;
        }
        
        enemyAgroState = newState;
    }

    #endregion

    #region Movement State

    internal EnemyMovementStates GetEnemyMovementState()
    {
        return enemyMovementStates;
    }

    private void SetEnemyMovementState(EnemyMovementStates newState)
    {
        if (enemyMovementStates == newState)
        {
            return;
        }

        SetStateData(newState);
        
        enemyAnimationManager.UpdateAnimation(newState);
        
        enemyMovementStates = newState;
        moveStateTimeElapsed = 0.0f;
    }

    private void SetStateData(EnemyMovementStates newState)
    {
        if (newState == EnemyMovementStates.Idle)
        {
            moveStateDuration = Random.Range(idleDuration.x, idleDuration.y);
        }
        else if (newState == EnemyMovementStates.Patrolling)
        {
            stateStartPos = transform.position;
            stateEndPos = GetNextPatrolPoint();
            
            float dist = Vector3.Distance(stateStartPos, stateEndPos);
            moveStateDuration = dist / patrollingSpeed;
        }
    }

    private void UpdateMoveStateTimer()
    {
        if (enemyMovementStates == EnemyMovementStates.Idle)
        {
            moveStateTimeElapsed += Time.deltaTime;

            if (moveStateTimeElapsed > moveStateDuration)
            {
                if (enemyAgroState == EnemyAgroState.Calm)
                {
                    SetEnemyMovementState(EnemyMovementStates.Patrolling);
                }
            }
        }
        else if (enemyMovementStates == EnemyMovementStates.Patrolling)
        {
            moveStateTimeElapsed += Time.deltaTime;

            if (moveStateTimeElapsed < moveStateDuration)
            {
                float fac = moveStateTimeElapsed / moveStateDuration;
                transform.position = Vector3.Lerp(stateStartPos, stateEndPos, fac);
            }
            else
            {
                if (enemyAgroState == EnemyAgroState.Calm)
                {
                    SetEnemyMovementState(EnemyMovementStates.Idle);
                }
            }
        }
    }

    private Vector3 GetNextPatrolPoint()
    {
        List<Vector3> availablePatrolPoints = new List<Vector3>();
        for (int i = 0; i < patrolPoints.Count; i++)
        {
            Vector3 point = patrolPoints[i];

            if (Vector3.Distance(point, transform.position) < 0.1f)
            {
                continue;
            }
            
            availablePatrolPoints.Add(point);
        }
        
        int ranAvailablePatrolPointIndex = Random.Range(0, availablePatrolPoints.Count);
        return availablePatrolPoints[ranAvailablePatrolPointIndex];
    }

    #endregion

    #region Getters

    internal Vector3 GetMoveDirection()
    {
        Vector3 moveDirection = stateEndPos - stateStartPos;
        moveDirection.Normalize();
        return moveDirection;
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            return;
        }
        
        Gizmos.color = new Color(1f, 0.25f, 0.25f, 1.0f);
        for (int i = 0; i < patrolPointTransforms.Count; i++)
        {
            Gizmos.DrawWireSphere(patrolPointTransforms[i].position, 0.1f);
        }
    }

    #endregion
}
