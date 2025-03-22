using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    public Player player;
    public CameraMovement cameraMovement { get; private set; }
    public CameraShake cameraShake { get; private set; }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUp(player);
    }

    #region SetUp

    internal void SetUp(Player player)
    {
        cameraMovement = GetComponent<CameraMovement>();
        cameraShake = GetComponent<CameraShake>();

        cameraMovement.SetUp(player);
        
        this.player = player;
    }

    #endregion
}
