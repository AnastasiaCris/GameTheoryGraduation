using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerOnRun : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();
    public Player player;
    public Transform points;

    public int enemyMultiplier { get; private set; } = 1;
    public int score { get; private set; }
    public int lives { get; private set; }

    [Header("UI Menu")] 
    public GameObject gameOverMenu;
    public GameObject pauseMenu;

    public static GameManagerOnRun instance;
    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    void Start()
    {
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);
     StartNewGame();   
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GameModeManager.gameMode == GameModeManager.GameMode.Classic)
        {
            Time.timeScale = 0;
            if(!pauseMenu.activeSelf)
                pauseMenu.SetActive(true);
            else
            {
                Resume();
            }
        }
    }

    public void StartNewGame()
    {
        SetScore(0);
        SetLives(3);
        StartNewRound();
    }

    /// <summary>
    /// Called when starting a new round (eating all points/losing all lives/starting new game)
    /// all points are added together with the player an enemeies
    /// </summary>
    private void StartNewRound()
    {
        //Precaution
        if(GameModeManager.gameMode == GameModeManager.GameMode.Classic)
            Time.timeScale = 1;
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        
        //Reset everything
        foreach (Transform points in this.points)
        {
            points.gameObject.SetActive(true);
        }

        ResetState();

        if (GameManagerEditor.instance.timerOn)
        {
            GameManagerEditor.instance.remainingTimer = GameManagerEditor.instance.timer;
            StopCoroutine(GameManagerEditor.instance.UpdateTimer());
            GameManagerEditor.instance.StartTimer();
        }

        if (GameManagerEditor.instance.spawnMoreEnemiesOnKill)
        {
            //delete all extra enemies and t
        }
    }

    /// <summary>
    /// Reset the enemies and player
    /// </summary>
    private void ResetState()
    {
        ResetEnemyMultiplier();
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].ResetState();
        }
        player.ResetState();
    }

    private void SetScore(int score)
    {
        this.score = score;
        UIManager.instance.score.text = "Score: " + score;
    }
    
    private void SetLives(int lives)
    {
        this.lives = lives;
        if(lives > 0 )
            UIManager.instance.life.text = "Lives: " + lives;
    }

    /// <summary>
    /// called when player loses all lives
    /// </summary>
     public void GameOver()
    {
        Time.timeScale = 0;
        gameOverMenu.SetActive(true);
        
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].gameObject.SetActive(false);
        }
        player.gameObject.SetActive(false);
    }
    

    public void EnemyDamaged(Enemy enemy)
    {
        SetScore(score + enemy.points * enemyMultiplier);
        enemyMultiplier++; //everytime you kill an enemy the score multiplies
    }
    
    /// <summary>
    /// Function called when the player is damaged by the enemy
    /// </summary>
    public void PlayerDamaged()
    {
        player.gameObject.SetActive(false);
        
        SetLives(lives-1);
        
        if (lives >= 0)
        {
            Invoke(nameof(ResetState), 1.0f);
        }
        else
        {
           GameOver();
        }
    }

    public void PointCollected(Points point)
    {
        point.gameObject.SetActive(false);
        
        SetScore(score + point.points);

        if (!PointsRemained())
        {
            player.gameObject.SetActive(false);
            Invoke(nameof(StartNewRound), 3);
        }
    }
    
    public void PowerUpCollected(PowerUp point)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].freightened.Enable(point.duration);
        }
        
        PointCollected(point);
        CancelInvoke();
        Invoke(nameof(ResetEnemyMultiplier), point.duration);
    }

    //Check if all points have been collected
    private bool PointsRemained()
    {
        foreach (Transform point in points)
        {
            if (point.gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    private void ResetEnemyMultiplier()
    {
        enemyMultiplier = 1;
    }

    #region UIButtons

    public void SwitchScene()
        {
            SceneManager.LoadScene(0);
        }
    
        public void SwitchMode()
        {
            pauseMenu.SetActive(false);
            gameOverMenu.SetActive(false);
            GameManagerEditor.instance.ChangeModes();
        }
    
        public void Resume()
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }

    #endregion
    
}
