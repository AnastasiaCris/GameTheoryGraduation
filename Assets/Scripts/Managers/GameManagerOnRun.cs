using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManagerOnRun : MonoBehaviour
{
    public List<Enemy> enemies = new List<Enemy>();
    public Player player;
    public Transform points;
    
    public int score { get; private set; }
    [HideInInspector]public int enemyPointMultiplier = 1;

    public int lives { get; private set; }
    [HideInInspector]public int maxLives = 0;
    
    [Space][Header("UI Menu")][Space]
    public GameObject gameOverMenu;
    public GameObject roundWonMenu;
    public GameObject gameOverText;
    public GameObject roundWonText;
    [Space]
    public GameObject P1WonMenu;
    public GameObject P2WonMenu;
    public GameObject P1WonText;
    public GameObject P2WonText;

    //extra
    [HideInInspector]public bool enemiesFreightened;
    [HideInInspector]public bool newRound;
    [HideInInspector]public bool sceneEnabled;

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

    public void StopCoroutines()
    {
        StopAllCoroutines();
        gameOverMenu.SetActive(false);
        roundWonMenu.SetActive(false);
        gameOverText.SetActive(false);
        roundWonText.SetActive(false);
        sceneEnabled = false;
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
        gameOverText.SetActive(false);
        roundWonText.SetActive(false);
        P1WonMenu.SetActive(false);
        P1WonText.SetActive(false);
        P2WonMenu.SetActive(false);
        P2WonText.SetActive(false);
        sceneEnabled = false;
        
        if (GameManagerEditor.instance.extraEnemies.Count > 0) //if there are any extra enemies, delete them
        {
            for (int i = 0; i < GameManagerEditor.instance.extraEnemies.Count; i++)
            {
                Destroy(GameManagerEditor.instance.extraEnemies[i]);
            }

            int range = GameManagerEditor.instance.multiplayer
                ? GameManagerEditor.instance.onSetUpMap.enemiesGameobjectClone.Length + 1
                : GameManagerEditor.instance.onSetUpMap.enemiesGameobjectClone.Length;
            enemies.RemoveRange(range, enemies.Count-range);
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
        
        if(GameManagerEditor.instance.multiplayer)
        {
            SetLives(GameManagerEditor.instance.maxLives, true);
        }
    }

    /// <summary>
    /// Reset the enemies and player
    /// </summary>
    private void ResetState()
    {
        enemyPointMultiplier = 1;
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
        SetScore(score + enemy.points * enemyPointMultiplier);
        enemyPointMultiplier++; //everytime you kill an enemy the score multiplies
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
        if (player.movement.playerDead)
            yield break;
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].canMove = false;
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

            sceneEnabled = true;    
            player.movement.direction = Vector2.zero;
            
            if (!won)
            {
                if (GameManagerEditor.instance.multiplayer)
                {
                    P2WonText.SetActive(true);
                    yield return new WaitForSeconds(0.3f);
                    P2WonText.SetActive(false);
                    yield return new WaitForSeconds(0.3f);
                    P2WonText.SetActive(true);
                    yield return new WaitForSeconds(0.3f);
                    P2WonText.SetActive(false);
                    yield return new WaitForSeconds(0.3f);
                    P2WonMenu.SetActive(true);
                    Time.timeScale = 0;
                }
                else
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

            }
            else
            {
                player.movement.anim.SetInteger("PlayerAnim", 5);

                if (GameManagerEditor.instance.multiplayer)
                {
                    P1WonText.SetActive(true);
                    yield return new WaitForSeconds(0.3f);
                    P1WonText.SetActive(false);
                    yield return new WaitForSeconds(0.3f);
                    P1WonText.SetActive(true);
                    yield return new WaitForSeconds(0.3f);
                    P1WonText.SetActive(false);
                    yield return new WaitForSeconds(0.3f);
                    P1WonMenu.SetActive(true);
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
            }

            sceneEnabled = false;
            yield return null;
        }

        public void ResetTime()
        {
            Time.timeScale = 1;
        }
        #endregion
    
}
