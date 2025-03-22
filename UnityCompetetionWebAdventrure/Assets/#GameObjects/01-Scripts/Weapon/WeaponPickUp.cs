using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    [Header("Weapon Type")]
    [SerializeField] private WeaponType weaponType;

    #region Weapon Pick Up Functions

    internal void OnPickUpCompleted()
    {
        gameObject.SetActive(false);
    }

    #endregion
    
    #region Getter

    internal WeaponType GetWeaponType()
    {
        return weaponType;
    }

    #endregion
}
