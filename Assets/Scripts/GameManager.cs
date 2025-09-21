using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public float startLife = 0.5f;
    public float maxLife = 20f;
    
    public bool paused = false;
    private PlayerManager player;

    private int score = 0;
    public TMP_Text playerScoreTxt;

    public GameObject GameOverPanel;
    public TMP_Text gameScoreTxt;
    public TMP_Text bestScoreTxt;

    public Image PanelFadeIn;

    public bool isTimingFading;

    public bool isStartingFromNothing = true;
    
    private void Awake()
    {
        instance = this;
        isStartingFromNothing = true;
    }
    private void Start()
    {
        InvokeRepeating("PassiveIncreaseScore", 0.1f, 0.05f);

        player = FindFirstObjectByType<PlayerManager>();
        
        player.life = startLife;
        isStartingFromNothing = true;
        
        GetComponent<ReducePlayerLifeByTime>().enabled = false;
    }
    private void Update()
    {
        playerScoreTxt.text = "score: " + score.ToString();

        if (isTimingFading)
        {
            if (Time.timeScale > 0.1f)
            {
                Time.timeScale -= Time.fixedDeltaTime / 10f;
            }
            else
            {
                isTimingFading = false;
                GameOverPanel.SetActive(true);
            }
        }
    }
    public void OnGameOver()
    {
        paused = true;
        isTimingFading = true;

        int bestscore = PlayerPrefs.GetInt("bestscore", 0);
        if(bestscore < score)
        {
            PlayerPrefs.SetInt("bestscore", score);
            bestscore = score;
        }
        //display stat
        gameScoreTxt.text = "score: " + score.ToString();
        bestScoreTxt.text = "best score: " + bestscore.ToString();

        PanelFadeIn.GetComponent<Animation>().Play();
    }

    
    public void RestartGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        isStartingFromNothing = true;
        GetComponent<ReducePlayerLifeByTime>().enabled = false;
        
    }
    
    public void PassiveIncreaseScore()
    {
        if (isStartingFromNothing) return;
        
        if (this.GetComponent<EnemySpawner>().waveActive && player.isOperational)
        {
            score += 1;
        }
    }

    private void StartPlaying()
    {
        isStartingFromNothing = false;
        GetComponent<ReducePlayerLifeByTime>().enabled = true;
    }
    
    public void MonsterDead(int id)
    {
        if (isStartingFromNothing)
        {
            StartPlaying();
        }
        
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
        Time.timeScale = 1f;
        RestartGame();
    }
    public void Exit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync("Menu");
    }

}
