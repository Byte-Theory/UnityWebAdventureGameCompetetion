using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : Singleton<UserInput>
{
    [Header("Player Input")]
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private bool isWalking;
    [SerializeField] private bool isDashing;
    
    // Input Actions
    InputAction moveAction;
    InputAction walkAction;
    InputAction dashAction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        walkAction = InputSystem.actions.FindAction("Walk");
        dashAction = InputSystem.actions.FindAction("Dash");
    }

    // Update is called once per frame
    void Update()
    {
        ListenForMoveInput();
        ListenForWalkInput();
        ListenForDashInput();
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
}
