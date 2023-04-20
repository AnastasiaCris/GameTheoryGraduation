using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public int maxLives = 0;
    
    [Header("UI Menu")] 
    public GameObject gameOverMenu;
    public GameObject roundWonMenu;
    public GameObject gameOverText;
    public GameObject roundWonText;

    //extra
    [HideInInspector]public bool enemiesFreightened;
    [HideInInspector]public bool newRound;

    
    public static GameManagerOnRun instance;
    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    void Start()
    {
        gameOverMenu.SetActive(false);
        roundWonMenu.SetActive(false);
        StartNewGame();   
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchMode();
        }
    }

    public void StartNewGame()
    {
        SetScore(0);
        SetLives(GameManagerEditor.instance.maxLives, true);
        StartNewRound();
    }

    /// <summary>
    /// Called when starting a new round (eating all points/losing all lives/starting new game)
    /// all points are added together with the player an enemies
    /// </summary>
    public void StartNewRound()
    {
        //Precaution
        if(GameModeManager.gameMode == GameModeManager.GameMode.Classic)
            Time.timeScale = 1;
        gameOverMenu.SetActive(false);
        roundWonMenu.SetActive(false);
        
        //Reset everything
        foreach (Transform points in this.points)
        {
            points.gameObject.SetActive(true);
        }

        ResetState();
        
        if (GameManagerEditor.instance.timerOn)
        {
            GameManagerEditor.instance.remainingTimer = GameManagerEditor.instance.timer;
            GameManagerEditor.instance.StartTimer();
        }

        if (GameManagerEditor.instance.spawnMoreEnemiesOnKill && enemies.Count > 4)
        {
            
            List<Enemy> extraEnemies = new List<Enemy>(enemies.Skip(4)) ;
            for (int i = 0; i < extraEnemies.Count; i++)
            {
                Destroy(extraEnemies[i].gameObject);
            }
            enemies.RemoveRange(4, enemies.Count-4);
        }

        if (GameManagerEditor.instance.killAllEnemies && PointsRemained())
        {
            StopCoroutine(EnableGameOver());
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
        UIManager.instance.highScore.text = "High Score \n" + PlayerPrefs.GetInt("HighScore", 0);
        if (this.score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", this.score);
            UIManager.instance.highScore.text = "High Score \n" + PlayerPrefs.GetInt("HighScore", 0);
        }
            
    }

    public void SetLives(int currentLife, bool startOfGame = false)
    {
        lives = currentLife;
        
        if (startOfGame)
        {
            maxLives = currentLife;
            if (maxLives < 6)
            {
                UIManager.instance.lifeHearts.SetActive(true);
                UIManager.instance.lifeBar.SetActive(false);
                for (int i = 0; i < UIManager.instance.hearts.Length; i++)
                {
                    if (i < lives)
                    {
                        UIManager.instance.hearts[i].enabled = true;
                    }
                    else
                    {
                        UIManager.instance.hearts[i].enabled = false;
                    }
                }
            }
            else
            {
                UIManager.instance.lifeHearts.SetActive(false);
                UIManager.instance.lifeBar.SetActive(true);
                UIManager.instance.healthSlider.maxValue = maxLives;
            }
        }

        if (maxLives < 6) //if it's hearts
        {
            for (int i = 0; i < UIManager.instance.hearts.Length; i++)
            {
                if (i < lives)
                {
                    UIManager.instance.hearts[i].sprite = UIManager.instance.fullHeart;
                }
                else
                {
                    UIManager.instance.hearts[i].sprite = UIManager.instance.emptyHeart;
                }
            }
        }
        else //if it's a health bar
        {
            UIManager.instance.healthSlider.value = lives;
        }
    }


    /// <summary>
    /// called when player loses all lives
    /// </summary>
     public void GameOver()
    {
        ShowScene(false);

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
        
        if (lives > 0)
        {
            Invoke(nameof(ResetState), 1.0f);
        }
        else
        {
           GameOver();
        }
    }

    #region GoalElement

    IEnumerator EnableGameOver()
    {
        yield return new WaitUntil((() => !enemiesFreightened));//wait until enemies are not freightened
        GameOver();
    }

    #endregion

    public void PointCollected(Points point)
    {
        point.gameObject.SetActive(false);
        
        SetScore(score + point.points);

        if (!PointsRemained() && !GameManagerEditor.instance.killAllEnemies)
        {
            player.gameObject.SetActive(false);
            ShowScene(true);
            
        }else if (!PointsRemained() && GameManagerEditor.instance.killAllEnemies )//if all points collected and you have to kill all enemies
        {
            StartCoroutine(EnableGameOver());
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

    ///Check if all points have been collected
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
            gameOverMenu.SetActive(false);
            roundWonMenu.SetActive(false);
            StartNewGame(); 
            GameManagerEditor.instance.ChangeModes();
        }

        public void ShowScene(bool won)
        {
            StartCoroutine(LevelOverScenes(won));
        }
        IEnumerator LevelOverScenes(bool won)
        {
            if (!won)
            {
                gameOverText.SetActive(true);
                yield return new WaitForSeconds(0.3f);
                gameOverText.SetActive(false);
                yield return new WaitForSeconds(0.3f);
                gameOverText.SetActive(true);
                yield return new WaitForSeconds(0.3f);
                gameOverText.SetActive(false);
                yield return new WaitForSeconds(0.3f);
                gameOverMenu.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                roundWonText.SetActive(true);
                yield return new WaitForSeconds(0.3f);
                roundWonText.SetActive(false);
                yield return new WaitForSeconds(0.3f);
                roundWonText.SetActive(true);
                yield return new WaitForSeconds(0.3f);
                roundWonText.SetActive(false);
                yield return new WaitForSeconds(0.3f);
                roundWonMenu.SetActive(true);
                Time.timeScale = 0;
            }

            yield return null;
        }
        #endregion
    
}
