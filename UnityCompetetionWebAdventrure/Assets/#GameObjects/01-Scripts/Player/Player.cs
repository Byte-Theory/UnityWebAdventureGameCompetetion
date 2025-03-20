using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMovement playermovement { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetUp();
    }

    #region SetUp

    internal void SetUp()
    {
        playermovement = GetComponent<PlayerMovement>();
        
        playermovement.SetUp();
    }

    #endregion
}
