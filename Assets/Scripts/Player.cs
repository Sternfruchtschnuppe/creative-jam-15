using System;
using UnityEditor.UI;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float life = 0f;
    
    private FlashLightController flashLightController;

    private void Start()
    {
        flashLightController = GetComponentInChildren<FlashLightController>();
    }

    public void UpdateLife(float life)
    {
        this.life = life;
        flashLightController.UpdateLife(life);
        
        if (life <= 0)
        {
            GameManager.instance.OnGameOver();
        }
    }
}
