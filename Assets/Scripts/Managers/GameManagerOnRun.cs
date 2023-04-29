using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManagerOnRun : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();
    [Space]public Player player;
    [Space]public Transform points;

    public int enemyMultiplier = 1;
    public int score { get; private set; }
    public int lives { get; private set; }
    [HideInInspector]public int maxLives = 0;
    
    [Space][Header("UI Menu")][Space]
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
        GameManagerEditor.instance.onRunManager = this;
    }

    void Start()
    {
        gameOverMenu.SetActive(false);
        roundWonMenu.SetActive(false);
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
        gameOverMenu.SetActive(false);
        roundWonMenu.SetActive(false);
        if (GameManagerEditor.instance.extraEnemies.Count > 0) //if there are any extra enemies, delete them
        {
            for (int i = 0; i < GameManagerEditor.instance.extraEnemies.Count; i++)
            {
                Destroy(GameManagerEditor.instance.extraEnemies[i]);
            }
            enemies.RemoveRange(GameManagerEditor.instance.onSetUpMap.enemiesGameobjectClone.Length, enemies.Count-GameManagerEditor.instance.onSetUpMap.enemiesGameobjectClone.Length);
            GameManagerEditor.instance.extraEnemies.Clear();
        }
        
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

        if (GameManagerEditor.instance.killAllEnemies && AllPointsCollected())
        {
            StopCoroutine(EnableGameOver());
        }
    }

    /// <summary>
    /// Reset the enemies and player
    /// </summary>
    private void ResetState()
    {
        enemyMultiplier = 1;
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
        StartCoroutine(PlayerDamagedNow());
    }

    IEnumerator PlayerDamagedNow()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].movement.direction = Vector2.zero;
        }
        player.movement.direction = Vector2.zero;
        player.movement.anim.SetInteger("PlayerAnim",4);
        player.movement.playerDead = true;
        yield return new WaitForSeconds(player.animDeath.length + 0.5f);

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
        yield return new WaitUntil(() => !enemiesFreightened);//wait until enemies are not freightened
        GameOver();
    }

    #endregion

    #region Points

        public void PointCollected(Points point)
        {
            point.gameObject.SetActive(false);
            
            SetScore(score + point.points);
    
            if (!AllPointsCollected() && !GameManagerEditor.instance.killAllEnemies)
            {
                ShowScene(true);
                
            }else if (!AllPowerUpsCollected() && GameManagerEditor.instance.killAllEnemies)//if all power ups collected and your goal is to kill all enemies 
            {
                StartCoroutine(EnableGameOver());
            }
        }
        
        public void PowerUpCollected(PowerUp point)
        {
            if (GameManagerEditor.instance.normalPowerUp)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    enemies[i].freightened.Enable(point.duration);
                }
            }else if (GameManagerEditor.instance.invincible)
            {
                player.Invincibility(point.duration);
                
            }else if (GameManagerEditor.instance.speed)
            {
                player.SpeedIncrease(point.duration);
            }
            
            PointCollected(point);
        }
    
        ///Check if all points have been collected
        private bool AllPointsCollected()
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

        private bool AllPowerUpsCollected()
        {
            PowerUp[] powerUps = FindObjectsOfType<PowerUp>();
            foreach (PowerUp point in  powerUps)
            {
                if (point.gameObject.activeSelf)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    
    #region UIButtons

        public void SwitchMode()
        {
            gameOverMenu.SetActive(false);
            roundWonMenu.SetActive(false);
            GameManagerEditor.instance.ChangeSpecificMode();
        }

        public void ShowScene(bool won)
        {
            StartCoroutine(LevelOverScenes(won));
        }
        IEnumerator LevelOverScenes(bool won)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].movement.direction = Vector2.zero;
            }
                
            player.movement.direction = Vector2.zero;
            
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
                
                player.movement.anim.SetInteger("PlayerAnim", 5);
                
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

        public void ResetTime()
        {
            Time.timeScale = 1;
        }
        #endregion
    
}
