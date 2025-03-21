using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMovement playermovement { get; private set; }
    public PlayerCollisionDetector playerCollisionDetector { get; private set; }
    public PlayerView playerView { get; private set; }
    public PlayerAnimator playerAnimator { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUp();
    }

    #region SetUp

    internal void SetUp()
    {
        playermovement = GetComponent<PlayerMovement>();
        playerCollisionDetector = GetComponent<PlayerCollisionDetector>();
        playerView = GetComponent<PlayerView>();
        playerAnimator = GetComponent<PlayerAnimator>();
        
        playermovement.SetUp(this);
        playerCollisionDetector.SetUp();
        playerView.SetUp(this);
        playerAnimator.SetUp(this);
    }

    #endregion
}
