using System;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [Header("Child References")] 
    [SerializeField] private GameObject containerGo;
    [SerializeField] private GameObject spriteGo;
    
    // Look Dir
    private Vector3 lookScale; 
    
    // Player
    private Player player;

    private void Update()
    {
        UpdateLookDir();
    }

    #region SetUp

    internal void SetUp(Player player)
    {
        this.player = player;
        SetUpLookDir();
    }

    #endregion

    #region Look Dir

    private void SetUpLookDir()
    {
        lookScale = new Vector3(1, 1, 1);
        spriteGo.transform.localScale = lookScale;
    }

    private void UpdateLookDir()
    {
        Vector2 playerVelocity = player.playermovement.GetRbVelocity();

        if (playerVelocity.x > 0)
        {
            lookScale.x = 1;
        }
        else if (playerVelocity.x < 0)
        {
            lookScale.x = -1;
        }
        
        spriteGo.transform.localScale = lookScale;
    }

    #endregion
}
