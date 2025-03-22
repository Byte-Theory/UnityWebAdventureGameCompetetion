using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class UserInput : Singleton<UserInput>
{
    [Header("Player Input")]
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private bool isWalking;
    [SerializeField] private bool isDashing;
    [SerializeField] private bool isAttacking;
    [SerializeField] private bool isAttackSpecialCharging;
    [SerializeField] private bool isAttackingSpecial;
    
    // Input Actions
    InputAction moveAction;
    InputAction walkAction;
    InputAction dashAction;
    InputAction attackAction;
    InputAction attackSpecialAction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        walkAction = InputSystem.actions.FindAction("Walk");
        dashAction = InputSystem.actions.FindAction("Dash");
        attackAction = InputSystem.actions.FindAction("Attack");
        attackSpecialAction = InputSystem.actions.FindAction("AttackSpecial");
    }

    // Update is called once per frame
    void Update()
    {
        ListenForMoveInput();
        ListenForWalkInput();
        ListenForDashInput();
        ListenForAttackInput();
        ListenForAttackSpecialInput();
    }

    #region Move Input

    private void ListenForMoveInput()
    {
        moveInput = moveAction.ReadValue<Vector2>();
    }

    internal Vector3 GetMoveInput()
    {
        return moveInput;
    }

    #endregion

    #region Walk Input

    private void ListenForWalkInput()
    {
        isWalking = walkAction.IsPressed();
    }

    internal bool GetWalkInput()
    {
        return isWalking;
    }
    
    #endregion

    #region Dash Input

    private void ListenForDashInput()
    {
        isDashing = dashAction.IsPressed();
    }

    internal bool GetDashInput()
    {
        return isDashing;
    }
    
    #endregion

    #region Attack

    private void ListenForAttackInput()
    {
        isAttacking = attackAction.IsPressed();
    }

    internal bool GetAttackInput()
    {
        return isAttacking;
    }

    private void ListenForAttackSpecialInput()
    {
        isAttackSpecialCharging = attackSpecialAction.IsPressed();
        isAttackingSpecial = attackSpecialAction.WasReleasedThisFrame();
    }

    internal bool GetAttackSpecialChargingInput()
    {
        return isAttackSpecialCharging;
    }

    internal bool GetAttackSpecialInput()
    {
        return isAttackingSpecial;
    }

    #endregion
}
