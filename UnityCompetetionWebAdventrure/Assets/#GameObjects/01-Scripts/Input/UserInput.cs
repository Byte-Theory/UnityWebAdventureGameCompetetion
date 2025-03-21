using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : Singleton<UserInput>
{
    [Header("Player Input")]
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private bool isWalking;
    
    // Input Actions
    InputAction moveAction;
    InputAction walkAction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        walkAction = InputSystem.actions.FindAction("Walk");
    }

    // Update is called once per frame
    void Update()
    {
        ListenForMoveInput();
        ListenForWalkInput();
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
}
