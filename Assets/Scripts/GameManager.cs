using System;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public float startLife = 5.0f;
    public float maxLife = 20f;
    
    public bool paused = false;
    private PlayerManager player;

    private int score = 0;
    public TMP_Text playerScoreTxt;

    public GameObject GameOverPanel;
    public TMP_Text gameScoreTxt;
    public TMP_Text bestScoreTxt;

    private void Awake()
    {
        instance = this;
        player = FindFirstObjectByType<PlayerManager>();
        player.life = startLife;
    }
    private void Start()
    {
        InvokeRepeating("PassiveIncreaseScore", 0.1f, 0.05f);  
    }

    public void OnGameOver()
    {
        paused = true;
        GameOverPanel.SetActive(true);

        int bestscore = PlayerPrefs.GetInt("bestscore", 0);
        if(bestscore < score)
        {
            PlayerPrefs.SetInt("bestscore", score);
            bestscore = score;
        }
        //display stat
        gameScoreTxt.text = "score: " + score.ToString();
        bestScoreTxt.text = "best score: " + bestscore.ToString();
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void PassiveIncreaseScore()
    {
        score += 1;
        playerScoreTxt.text = "score: " + score.ToString();
    }

    public void Replay()
    {
        RestartGame();
    }
    public void Exit()
    {
        SceneManager.LoadScene("Menu");
    }

}
