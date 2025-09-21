using System;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        
    }
    private void Start()
    {
        InvokeRepeating("PassiveIncreaseScore", 0.1f, 0.05f);

        player = FindFirstObjectByType<PlayerManager>();
        player.life = startLife;
    }
    private void Update()
    {
        playerScoreTxt.text = "score: " + score.ToString();
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
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    public void PassiveIncreaseScore()
    {
        if (this.GetComponent<EnemySpawner>().waveActive && player.isOperational)
        {
            score += 1;
        }
    }
    public void MonsterDead(int id)
    {
        // progression quadratique avec le niveau
        score += 50 * (id + 1) * (id + 1);
    }
    public void WaveTerminated(int num)
    {
        // progression quadratique de la vague
        score += 200 * (num + 1) * (num + 1);
    }

    public void Replay()
    {
        RestartGame();
    }
    public void Exit()
    {
        SceneManager.LoadSceneAsync("Menu");
    }

}
