using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : Singleton<UserInput>
{
    [Header("Player Input")]
    [SerializeField] private Vector2 moveInput;
    
    // Input Actions
    InputAction moveAction;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }

    // Update is called once per frame
    void Update()
    {
        ListenForMoveInput();
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
}
