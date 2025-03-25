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
    private float patrolPointMinX;
    private float patrolPointMaxX;
    
    // Player detection
    private Player detectedPlayer;
    
    // Look At
    private Vector3 lookAtStartPos;
    private Vector3 lookAtEndPos;
    
    //Ref
    private GroundedEnemyView groundedEnemyView;
    private EnemyAnimationManager enemyAnimationManager;
    private EnemySensor enemySensor;

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
        enemySensor = GetComponent<EnemySensor>();
        
        groundedEnemyView.SetUp(this);
        enemySensor.SetUp(this);
        
        SetUpPatrolPoints();
        
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
            
            UpdateLookAtDirections(stateStartPos, stateEndPos);
            
            float dist = Vector3.Distance(stateStartPos, stateEndPos);
            moveStateDuration = dist / patrollingSpeed;
        }
        else if (newState == EnemyMovementStates.LookingAtPlayer)
        {
            stateStartPos = transform.position;
            stateEndPos = detectedPlayer.transform.position;
            
            UpdateLookAtDirections(stateStartPos, stateEndPos);
            
            moveStateDuration = Random.Range(looAtPlayerDuration.x, looAtPlayerDuration.y);
        }
        else if (newState == EnemyMovementStates.ChasingPlayer)
        {
            stateStartPos = transform.position;
            stateEndPos = detectedPlayer.transform.position;
            
            UpdateLookAtDirections(stateStartPos, stateEndPos);
        }
        else if (newState == EnemyMovementStates.ChasingPlayerIdle)
        {
            stateStartPos = transform.position;
            stateEndPos = detectedPlayer.transform.position;
            
            UpdateLookAtDirections(stateStartPos, stateEndPos);
        }
    }

    private void UpdateMoveStateTimer()
    {
        if (enemyMovementStates == EnemyMovementStates.Idle)
        {
            moveStateTimeElapsed += Time.deltaTime;

            if (moveStateTimeElapsed > moveStateDuration)
            {
                SetEnemyMovementState(EnemyMovementStates.Patrolling);
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
                SetEnemyMovementState(EnemyMovementStates.Idle);
            }
        }
        else if (enemyMovementStates == EnemyMovementStates.ChasingPlayerIdle)
        {
            Vector3 destination = detectedPlayer.transform.position;
            destination.y = patrolPoints[0].y;
            
            if (destination.x < patrolPointMinX)
            {
                destination.x = patrolPointMinX;
            }
            else if (destination.x > patrolPointMaxX)
            {
                destination.x = patrolPointMaxX;
            }
            
            if (!Mathf.Approximately(destination.x, patrolPointMinX) && 
                !Mathf.Approximately(destination.x, patrolPointMaxX))
            {
                if (enemyAgroState == EnemyAgroState.Calm)
                {
                    SetEnemyMovementState(EnemyMovementStates.Patrolling);
                }
                else if (enemyAgroState == EnemyAgroState.Agro)
                {
                    SetEnemyMovementState(EnemyMovementStates.ChasingPlayer);
                }
            }
            
            UpdateLookAtDirections(transform.position, detectedPlayer.transform.position);
        }
        else if (enemyMovementStates == EnemyMovementStates.LookingAtPlayer)
        {
            moveStateTimeElapsed += Time.deltaTime;

            if (moveStateTimeElapsed > moveStateDuration)
            {
                if (enemyAgroState == EnemyAgroState.Calm)
                {
                    SetEnemyMovementState(EnemyMovementStates.Patrolling);
                }
                else if (enemyAgroState == EnemyAgroState.Agro)
                {
                    SetEnemyMovementState(EnemyMovementStates.ChasingPlayer);
                }
            }
            
            UpdateLookAtDirections(transform.position, detectedPlayer.transform.position);
        }
        else if (enemyMovementStates == EnemyMovementStates.ChasingPlayer)
        {
            Vector3 destination = detectedPlayer.transform.position;
            destination.y = patrolPoints[0].y;
            
            if (destination.x < patrolPointMinX)
            {
                destination.x = patrolPointMinX;
            }
            else if (destination.x > patrolPointMaxX)
            {
                destination.x = patrolPointMaxX;
            }
            
            transform.position = Vector3.MoveTowards(transform.position, 
                                                destination, Time.deltaTime * chasingSpeed);

            if ((Mathf.Approximately(transform.position.x, patrolPointMinX) || 
                 Mathf.Approximately(transform.position.x, patrolPointMaxX)))
            {
                if (enemyAgroState == EnemyAgroState.Calm)
                {
                    SetEnemyMovementState(EnemyMovementStates.Patrolling);
                }
                else if (enemyAgroState == EnemyAgroState.Agro)
                {
                    SetEnemyMovementState(EnemyMovementStates.ChasingPlayerIdle);
                }
            }
            
            UpdateLookAtDirections(transform.position, detectedPlayer.transform.position);
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

    #region Patrol points

    private void SetUpPatrolPoints()
    {
        patrolPointMinX = float.MaxValue;
        patrolPointMaxX = float.MinValue;
        patrolPoints = new List<Vector3>();
        
        for (int i = 0; i < patrolPointTransforms.Count; i++)
        {
            Transform point = patrolPointTransforms[i];
            if (point.position.x < patrolPointMinX)
            {
                patrolPointMinX = point.position.x;
            }

            if (point.position.x > patrolPointMaxX)
            {
                patrolPointMaxX = point.position.x;
            }
            
            patrolPoints.Add(point.position);
        }
    }

    #endregion
    
    #region Player Detection

    internal void PlayerDetected(Player player)
    {
        if (player != null && enemyAgroState != EnemyAgroState.Agro)
        {
            detectedPlayer = player;
            SetEnemyAgroState(EnemyAgroState.Agro);
            SetEnemyMovementState(EnemyMovementStates.LookingAtPlayer);
        }
    }

    #endregion
    
    #region Look At

    private void UpdateLookAtDirections(Vector3 startPos, Vector3 endPos)
    {
        lookAtStartPos = startPos;
        lookAtEndPos = endPos;
    }
    
    internal Vector3 GetLookAtDirection()
    {
        Vector3 moveDirection = lookAtEndPos - lookAtStartPos;
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
