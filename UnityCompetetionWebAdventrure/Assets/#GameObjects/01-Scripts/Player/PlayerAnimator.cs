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
    
    private readonly string AnimWeaponTypeTag = "WeaponType";
    private readonly string AnimAttack1Tag = "Attack1";
    private readonly string AnimAttack2Tag = "Attack2";
    private readonly string AnimAttackSpecialTag = "AttackSpecial";
    
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

    #region Movement Animations
    
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

    #region Animation Type Setting Based On Weapon

    internal void SetAnimatorWeaponType(int weaponType)
    {
        anim.SetInteger(AnimWeaponTypeTag, weaponType);
    }

    #endregion

    #region Attack Animations

    internal void SetAttack1AnimState(bool play)
    {
        anim.SetBool(AnimAttack1Tag, play);    
    }
    
    internal void SetAttack2AnimState(bool play)
    {
        anim.SetBool(AnimAttack2Tag, play);    
    }
    
    internal void SetAttackSpecialAnimState(bool play)
    {
        anim.SetBool(AnimAttackSpecialTag, play);    
    }
    
    #endregion
}
