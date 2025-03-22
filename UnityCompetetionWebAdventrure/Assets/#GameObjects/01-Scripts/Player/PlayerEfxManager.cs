using UnityEngine;

public class PlayerEfxManager : MonoBehaviour
{
    [Header("Attack Special")] 
    [SerializeField] private ParticleSystem attackSpecialChargingEfx;
    [SerializeField] private ParticleSystem attackSpecialChargedEfx;

    #region Attack Efx

    internal void PlayAttackSpecialChargingEfx(bool play)
    {
        if (play)
        {
            if (!attackSpecialChargingEfx.isPlaying)
            {
                attackSpecialChargingEfx.Play();
            }
        }
        else
        {
            if (attackSpecialChargingEfx.isPlaying)
            {
                attackSpecialChargingEfx.Stop();
            }
        }
    }
    
    internal void PlayAttackSpecialChargedEfx(bool play)
    {
        if (play)
        {
            if (!attackSpecialChargedEfx.isPlaying)
            {
                attackSpecialChargedEfx.Play();
            }
        }
        else
        {
            if (attackSpecialChargedEfx.isPlaying)
            {
                attackSpecialChargedEfx.Stop();
            }
        }
    }

    #endregion
}
