
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerOnRun : MonoBehaviour
{
    public Enemy[] enemies;
    public Player player;
    public Transform points;

    public int enemyMultiplier { get; private set; } = 1;
    public int score { get; private set; }
    public int lives { get; private set; }

    public static GameManagerOnRun instance;
    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    void Start()
    {
     StartNewGame();   
    }

    void StartNewGame()
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
        foreach (Transform points in this.points)
        {
            points.gameObject.SetActive(true);
        }

        ResetState();
    }

    /// <summary>
    /// Reset the enemies and player
    /// </summary>
    private void ResetState()
    {
        ResetEnemyMultiplier();
        for (int i = 0; i < enemies.Length; i++)
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
    /// 
    /// </summary>
     IEnumerator GameOver()
    {
        //TO DO: display UI
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].gameObject.SetActive(false);
        }
        player.gameObject.SetActive(false);

        //yield return new WaitForSeconds(1f);
        //TO DO: display text saying to press any button to start new game
        
        yield return new WaitUntil(() => Input.anyKey);
        StartNewGame();
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
            Invoke(nameof(ResetState), 3.0f);
        }
        else
        {
            StartCoroutine(GameOver());
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
        for (int i = 0; i < enemies.Length; i++)
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
}
