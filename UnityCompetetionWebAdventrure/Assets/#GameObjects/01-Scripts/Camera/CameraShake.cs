using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class CameraShake : MonoBehaviour
{
    [System.Serializable]
    public class CameraShakeData
    {
        public float shakeDuration;
        public float shakeStrength;
        public int vibrato;
        public float randomness;
        public bool snapping;
        public bool fadeOut;
        public ShakeRandomnessMode shakeRandomnessMode;
    }
    
    [Header("Cam")]
    [SerializeField] private Transform camT;
    
    [Header("Shake Data")] 
    [SerializeField] private Vector3 camOriginalPos;
    [SerializeField] private CameraShakeData attackSpShakeData;

    #region SetUp

    internal void SetUp()
    {
        camOriginalPos = camT.transform.localPosition;
    }

    #endregion

    #region Attach Special Charging

    internal void ShakeCameraOnAttackSpecial()
    {
        camT.DOShakePosition(attackSpShakeData.shakeDuration, attackSpShakeData.shakeStrength, 
                attackSpShakeData.vibrato, attackSpShakeData.randomness, attackSpShakeData.snapping, 
                attackSpShakeData.fadeOut, attackSpShakeData.shakeRandomnessMode)
            .SetEase(Ease.InOutQuad)
            .OnComplete(OnShakeCompleted);
    }

    #endregion

    #region Shape Complete

    private void OnShakeCompleted()
    {
        camT.transform.localPosition = camOriginalPos;
    }

    #endregion
}
