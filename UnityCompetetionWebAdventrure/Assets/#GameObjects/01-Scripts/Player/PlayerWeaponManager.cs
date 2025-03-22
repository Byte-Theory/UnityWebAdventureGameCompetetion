using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerWeaponManager : MonoBehaviour
{
    [Header("Weapon Data")]
    [SerializeField] private WeaponData weaponData;
    
    [Header("Equipped Weapon")]
    [SerializeField] private WeaponType equippedWeaponType;
    private WeaponData.Data equippedWeaponData;
    
    // Attack Common
    private bool isSpecialAttack;
    private bool isAttacking;
    private bool doAttackDmg;
    
    // Attack Normal
    private bool attackInput;
    private bool isDoingAttack1;
    private float attackAnimDuration;
    private float attackDelay;
    private float attackTimeElapsed;
    
    // Attack Special
    private bool attackSpecialInput;
    private bool isAttackSpecialOnCooldown;
    private float attackSpecialCooldownTimeElpased;
    
    // Carrying Weapon
    private List<WeaponType> carryingWeaponTypes;
    
    // Player
    private Player player;
    
    //Ref
    private UserInput userInput;

    private void Update()
    {
        GetInput();
        
        CheckForAttack();
        CheckForAttackSpecial();
        
        UpdateAttackTimer();
        AttackSpecialCooldownTimer();
    }

    #region SetUp

    internal void SetUp(Player player)
    {
        userInput = UserInput.Instance;
        
        this.player = player;

        carryingWeaponTypes = new List<WeaponType>();

        AddWeaponTypeCarryingWeapon(WeaponType.None);
        EquipNoWeapon();
    }

    #endregion

    #region Weapon PickedUp

    internal void OnWeaponPickup(WeaponPickUp weaponPickUp)
    {
        if (weaponPickUp == null)
        {
            return;
        }

        WeaponType pickUpWeaponType = weaponPickUp.GetWeaponType();
        
        AddWeaponTypeCarryingWeapon(pickUpWeaponType);
        SetEquippedWeapon(pickUpWeaponType);

        weaponPickUp.OnPickUpCompleted();
    }

    #endregion

    #region Carrying Weapon

    private void AddWeaponTypeCarryingWeapon(WeaponType weaponType)
    {
        if (!carryingWeaponTypes.Contains(weaponType))
        {
            carryingWeaponTypes.Add(weaponType);
            SortCarryingWeapons();
        }
    }

    private void SortCarryingWeapons()
    {
        
    }

    #endregion

    #region Equiped Weapon

    private void EquipNoWeapon()
    {
        SetEquippedWeapon(WeaponType.None);
    }
    
    private void SetEquippedWeapon(WeaponType weaponType)
    {
        equippedWeaponType = weaponType;

        equippedWeaponData = weaponData.GetWeaponData(equippedWeaponType);
        
        player.playerAnimator.SetAnimatorWeaponType((int)equippedWeaponType);
    }

    #endregion

    #region Input

    private void GetInput()
    {
        attackInput = userInput.GetAttackInput();
        attackSpecialInput = userInput.GetAttackSpecialInput();
    }

    #endregion
    
    #region Attack
    
    #region Attack Normal

    private void CheckForAttack()
    {
        bool isDashing = player.playermovement.GetDashingActive();
        
        if (attackInput && !isAttacking && !isDashing && equippedWeaponType != WeaponType.None)
        {
            isAttacking = true;
            doAttackDmg = false;
            attackTimeElapsed = 0.0f;
            isSpecialAttack = false;
            
            CalcAttackData();
            SetAttackAnimation(isAttacking);
        }
    }

    private void CalcAttackData()
    {
        float chance = Random.Range(0.0f, 1.0f);

        isDoingAttack1 = chance <= equippedWeaponData.attack1Chance;

        if (isDoingAttack1)
        {
            attackAnimDuration = equippedWeaponData.attack1AnimDur;
            attackDelay = equippedWeaponData.attack1Delay;
        }
        else
        {
            attackAnimDuration = equippedWeaponData.attack2AnimDur;
            attackDelay = equippedWeaponData.attack2Delay;
        }
    }

    #endregion

    #region Attack Special

    private void CheckForAttackSpecial()
    {
        bool isDashing = player.playermovement.GetDashingActive();
        
        if (attackSpecialInput && !isAttacking && !isAttackSpecialOnCooldown && !isDashing  && equippedWeaponType != WeaponType.None)
        {
            isAttacking = true;
            doAttackDmg = false;
            attackTimeElapsed = 0.0f;
            isSpecialAttack = true;

            SetAttackSpecialData();
            SetAttackAnimation(isAttacking);
        }
    }
    
    private void SetAttackSpecialData()
    {
        attackAnimDuration = equippedWeaponData.attackSpAnimDur;
        attackDelay = equippedWeaponData.attackSpDelay;
    }

    private void SetAttackSpecialOnCooldown()
    {
        isAttackSpecialOnCooldown = true;
        attackSpecialCooldownTimeElpased = 0.0f;
    }

    private void AttackSpecialCooldownTimer()
    {
        if (isAttackSpecialOnCooldown)
        {
            attackSpecialCooldownTimeElpased += Time.deltaTime;

            if (attackSpecialCooldownTimeElpased >= equippedWeaponData.attackSpCooldownDuration)
            {
                isAttackSpecialOnCooldown = false;
                attackSpecialCooldownTimeElpased = 0.0f;
            }
        }
    }

    private bool CheckAndThrowSpear()
    {
        if (equippedWeaponType == WeaponType.Spear)
        {
            carryingWeaponTypes.Remove(equippedWeaponType);
            EquipNoWeapon();

            return true;
        }

        return false;
    }
    
    #endregion
    
    private void UpdateAttackTimer()
    {
        if (isAttacking)
        {
            attackTimeElapsed += Time.deltaTime;

            if (attackDelay >= 0 && attackTimeElapsed >= attackDelay && !doAttackDmg)
            {
                doAttackDmg = true;
                
                //TODO: Damage Enemy
            }

            if (attackTimeElapsed > attackAnimDuration)
            {
                isAttacking = false;
                doAttackDmg = false;

                
                if (isSpecialAttack && !isAttackSpecialOnCooldown)
                {
                    bool isSpearThrown = CheckAndThrowSpear();
                    SetAttackSpecialOnCooldown();
                }
                
                SetAttackAnimation(isAttacking);
            }
        }
    }

    internal bool GetIsAttacking()
    {
        return isAttacking;
    }
    
    private void SetAttackAnimation(bool play)
    {
        if (isSpecialAttack)
        {
            player.playerAnimator.SetAttackSpecialAnimState(play);
        }
        else
        {
            if (isDoingAttack1)
            {
                player.playerAnimator.SetAttack1AnimState(play);
            }
            else
            {
                player.playerAnimator.SetAttack2AnimState(play);
            }
        }
    }
    
    #endregion
}
