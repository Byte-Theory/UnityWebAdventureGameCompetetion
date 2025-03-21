using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Animator")] 
    [SerializeField] private Animator anim;

    private readonly string AnimRunTag = "Running";
    private readonly string AnimJumpTag = "Jump";
    private readonly string AnimFallTag = "Fall";
    private readonly string AnimDashTag = "Dash";
    private readonly string AnimDieTag = "Die";
    
    // Player
    private Player player;
    
    // Update is called once per frame
    void Update()
    {
        UpdateRunAnimation();
        UpdateJumpAnimation();
        UpdateFallAnimation();
        UpdateDashAnimation();
    }

    #region SetUp

    internal void SetUp(Player player)
    {
        this.player = player;
    }

    #endregion

    #region Animations Triggering
    
    private void UpdateRunAnimation()
    {
        Vector2 playerVel = player.playermovement.GetRbVelocity();
        bool isRunning = playerVel.x != 0;
        
        anim.SetBool(AnimRunTag, isRunning);
    }

    private void UpdateJumpAnimation()
    {
        Vector2 playerVel = player.playermovement.GetRbVelocity();
        bool isJumping = playerVel.y > 0;
        
        anim.SetBool(AnimJumpTag, isJumping);
    }

    private void UpdateFallAnimation()
    {
        Vector2 playerVel = player.playermovement.GetRbVelocity();
        bool isFalling = playerVel.y < 0;
        
        anim.SetBool(AnimFallTag, isFalling);
    }

    private void UpdateDashAnimation()
    {
        bool isDashing = player.playermovement.GetDashingActive();
        
        anim.SetBool(AnimDashTag, isDashing);
    }

    internal void PlayDeathAnimation()
    {
        anim.SetTrigger(AnimDieTag);
    }
    
    #endregion
}
