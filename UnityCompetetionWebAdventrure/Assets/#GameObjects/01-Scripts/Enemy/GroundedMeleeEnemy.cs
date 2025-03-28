using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GroundedMeleeEnemy : MonoBehaviour
{
    [Header("Enemy Type")] 
    [SerializeField] private EnemyType enemyType;
    
    [Header("States")] 
    [SerializeField] private EnemyAgroState enemyAgroState = EnemyAgroState.Unknown;
    [SerializeField] private EnemyStates enemyStates = EnemyStates.Unknown;
    private float moveStateTimeElapsed;
    private float moveStateDuration;

    private Vector3 stateStartPos;
    private Vector3 stateEndPos;

    [Header("Movement Data")] 
    [SerializeField] private Vector2 idleDuration;
    [SerializeField] private float patrollingSpeed;
    [SerializeField] private Vector2 looAtPlayerDuration;
    [SerializeField] private float chasingSpeed;
    private float stoppingDistance;
    
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
    public GroundedEnemyView groundedEnemyView {get; private set; }
    public EnemyAnimationManager enemyAnimationManager {get; private set; }
    public GroundedMeleeEnemySensor groundedMeleeEnemySensor {get; private set; }
    public EnemyAttackManager enemyAttackManager {get; private set; }

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
        groundedMeleeEnemySensor = GetComponent<GroundedMeleeEnemySensor>();
        enemyAttackManager = GetComponent<EnemyAttackManager>();
        
        groundedEnemyView.SetUp(this);
        groundedMeleeEnemySensor.SetUp(this);
        enemyAttackManager.SetUp(this);

        stoppingDistance = groundedMeleeEnemySensor.GetStoppingDistance();
        
        SetUpPatrolPoints();
        
        SetEnemyAgroState(EnemyAgroState.Calm);
        SetEnemyMovementState(EnemyStates.Idle);
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

    internal EnemyStates GetEnemyMovementState()
    {
        return enemyStates;
    }

    private void SetEnemyMovementState(EnemyStates newState, bool overrideState = false)
    {
        if (enemyStates == newState && !overrideState)
        {
            return;
        }

        SetStateData(newState);
        
        enemyAnimationManager.UpdateAnimation(newState);
        
        enemyStates = newState;
        moveStateTimeElapsed = 0.0f;
    }

    private void SetStateData(EnemyStates newState)
    {
        if (newState == EnemyStates.Idle)
        {
            enemyAttackManager.ResetAttackPattern();
            moveStateDuration = Random.Range(idleDuration.x, idleDuration.y);
        }
        else if (newState == EnemyStates.Patrolling)
        {
            stateStartPos = transform.position;
            stateEndPos = GetNextPatrolPoint();
            
            UpdateLookAtDirections(stateStartPos, stateEndPos);
            
            float dist = Vector3.Distance(stateStartPos, stateEndPos);
            moveStateDuration = dist / patrollingSpeed;
        }
        else if (newState == EnemyStates.LookingAtPlayer)
        {
            stateStartPos = transform.position;
            stateEndPos = detectedPlayer.transform.position;
            
            UpdateLookAtDirections(stateStartPos, stateEndPos);
            
            moveStateDuration = Random.Range(looAtPlayerDuration.x, looAtPlayerDuration.y);
        }
        else if (newState == EnemyStates.ChasingPlayer)
        {
            stateStartPos = transform.position;
            stateEndPos = detectedPlayer.transform.position;
            
            UpdateLookAtDirections(stateStartPos, stateEndPos);
        }
        else if (newState == EnemyStates.ChasingPlayerIdle)
        {
            stateStartPos = transform.position;
            stateEndPos = detectedPlayer.transform.position;
            
            UpdateLookAtDirections(stateStartPos, stateEndPos);
        }
        else if (newState == EnemyStates.StartAttacking)
        {
            enemyAttackManager.SetUpAttackPattern();
        }
        else if (newState == EnemyStates.Attacking)
        {
            enemyAttackManager.SetUpAttack();
            
            stateStartPos = transform.position;
            stateEndPos = detectedPlayer.transform.position;

            moveStateDuration = enemyAttackManager.GetAttackDuration();
            
            UpdateLookAtDirections(stateStartPos, stateEndPos);
        }
        else if (newState == EnemyStates.AttackingIdle)
        {
            stateStartPos = transform.position;
            stateEndPos = detectedPlayer.transform.position;

            moveStateDuration = enemyAttackManager.GetAttackIdleDuration();
            
            UpdateLookAtDirections(stateStartPos, stateEndPos);
        }
    }

    private void UpdateMoveStateTimer()
    {
        if (enemyStates == EnemyStates.StartAttacking)
        {
            SetEnemyMovementState(EnemyStates.Attacking);
        }
        
        if (enemyStates == EnemyStates.Idle)
        {
            moveStateTimeElapsed += Time.deltaTime;

            if (moveStateTimeElapsed > moveStateDuration)
            {
                SetEnemyMovementState(EnemyStates.Patrolling);
            }
        }
        else if (enemyStates == EnemyStates.Patrolling)
        {
            moveStateTimeElapsed += Time.deltaTime;

            if (moveStateTimeElapsed < moveStateDuration)
            {
                float fac = moveStateTimeElapsed / moveStateDuration;
                transform.position = Vector3.Lerp(stateStartPos, stateEndPos, fac);
            }
            else
            {
                SetEnemyMovementState(EnemyStates.Idle);
            }
        }
        else if (enemyStates == EnemyStates.ChasingPlayerIdle)
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
                    SetEnemyMovementState(EnemyStates.Patrolling);
                }
                else if (enemyAgroState == EnemyAgroState.Agro)
                {
                    SetEnemyMovementState(EnemyStates.ChasingPlayer);
                }
            }
            
            UpdateLookAtDirections(transform.position, detectedPlayer.transform.position);
        }
        else if (enemyStates == EnemyStates.LookingAtPlayer)
        {
            moveStateTimeElapsed += Time.deltaTime;

            if (moveStateTimeElapsed > moveStateDuration)
            {
                if (enemyAgroState == EnemyAgroState.Calm)
                {
                    SetEnemyMovementState(EnemyStates.Patrolling);
                }
                else if (enemyAgroState == EnemyAgroState.Agro)
                {
                    SetEnemyMovementState(EnemyStates.ChasingPlayer);
                }
            }
            
            UpdateLookAtDirections(transform.position, detectedPlayer.transform.position);
        }
        else if (enemyStates == EnemyStates.ChasingPlayer)
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

            float distToDestination = Vector3.Distance(transform.position, destination);
            float distToPlayer = Vector3.Distance(transform.position, detectedPlayer.transform.position);
            
            if ((Mathf.Approximately(transform.position.x, patrolPointMinX) || 
                 Mathf.Approximately(transform.position.x, patrolPointMaxX)))
            {
                if (enemyAgroState == EnemyAgroState.Calm)
                {
                    SetEnemyMovementState(EnemyStates.Patrolling);
                }
                else if (enemyAgroState == EnemyAgroState.Agro)
                {
                    SetEnemyMovementState(EnemyStates.ChasingPlayerIdle);
                }
            }
            else if (distToPlayer < stoppingDistance)
            {
                if (enemyAgroState == EnemyAgroState.Calm)
                {
                    SetEnemyMovementState(EnemyStates.Patrolling);
                }
                else if (enemyAgroState == EnemyAgroState.Agro)
                {
                    SetEnemyMovementState(EnemyStates.StartAttacking);
                }
            }
            
            UpdateLookAtDirections(transform.position, detectedPlayer.transform.position);
        }
        else if (enemyStates == EnemyStates.Attacking)
        {
            moveStateTimeElapsed += Time.deltaTime;

            if (moveStateTimeElapsed < moveStateDuration)
            {
                enemyAttackManager.TryDamageThePlayer(moveStateTimeElapsed, detectedPlayer);
            }
            else
            {
                bool isAttackCompleted = enemyAttackManager.CheckIfAttackCompleted();
                if (isAttackCompleted)
                {
                    SetEnemyMovementState(EnemyStates.AttackingIdle);
                }
                else
                {   
                    SetEnemyMovementState(EnemyStates.Attacking, true);
                }
            }
            
            
            UpdateLookAtDirections(transform.position, detectedPlayer.transform.position);
        }
        else if (enemyStates == EnemyStates.AttackingIdle)
        {
            moveStateTimeElapsed += Time.deltaTime;
            
            if (moveStateTimeElapsed > moveStateDuration)
            {
                float distToPlayer = Vector3.Distance(transform.position, detectedPlayer.transform.position);
                if (distToPlayer > stoppingDistance)
                {
                    if (enemyAgroState == EnemyAgroState.Calm)
                    {
                        SetEnemyMovementState(EnemyStates.Patrolling);
                        return;
                    }
                    else if (enemyAgroState == EnemyAgroState.Agro)
                    {
                        SetEnemyMovementState(EnemyStates.ChasingPlayer);
                        return;
                    }
                }
                else
                {
                    SetEnemyMovementState(EnemyStates.StartAttacking);
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
            SetEnemyMovementState(EnemyStates.LookingAtPlayer);
        }
        else if (player == null && enemyAgroState != EnemyAgroState.Calm)
        {
            detectedPlayer = null;
            SetEnemyAgroState(EnemyAgroState.Calm);
            SetEnemyMovementState(EnemyStates.Idle);
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
