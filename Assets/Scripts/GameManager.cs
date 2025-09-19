using System;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public float startLife = 5.0f;
    
    public bool paused = false;
    private PlayerManager player;

    private void Awake()
    {
        instance = this;
        player = FindFirstObjectByType<PlayerManager>();
        player.life = startLife;
    }
    
    public void OnGameOver()
    {
        paused = true;
        RestartGame();
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
