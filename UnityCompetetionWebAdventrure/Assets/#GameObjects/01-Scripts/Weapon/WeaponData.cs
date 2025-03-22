using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/WeaponData")]
public class WeaponData : ScriptableObject
{
    [System.Serializable]
    public class Data
    {
        public WeaponType WeaponType;

        [Header("Attack 1")] 
        public float attack1Chance;
        public float attack1AnimDur;
        public float attack1Delay;
        public float attack1StaminaCost;
        
        [Header("Attack 2")]
        public float attack2Chance;
        public float attack2AnimDur;
        public float attack2Delay;
        public float attack2StaminaCost;

        [Header("Attack Special")] 
        public float attackSpAnimDur;
        public float attackSpDelay;
        public float attackSpStaminaCost;
        public float attackSpCooldownDuration;
    }

    [SerializeField] private List<Data> allWeaponsData;

    internal Data GetWeaponData(WeaponType weaponType)
    {
        for (int i = 0; i < allWeaponsData.Count; i++)
        {
            Data data = allWeaponsData[i];

            if (data.WeaponType == weaponType)
            {
                return data;
            }
        }

        return null;
    }
}
