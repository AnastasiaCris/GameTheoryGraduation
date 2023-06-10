using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyFreightened : EnemyBehaviour
{
    private float durationLeft;
    [SerializeField] private int index;
    public SpriteRenderer body;
    public SpriteRenderer deadBody;
    
    public Color originalBodyCol;
    public Color freightenedBodyCol;
    public Color flashFreightenedBodyCol;
    [HideInInspector]public bool dead;
    [HideInInspector]public bool canExit;
    private bool reachedHouse;
    public bool player;
    public GameObject arrow;
    
    public override void Enable(float duration)
    {
        originalDuration = duration;
        duration = originalDuration * GameManagerEditor.instance.changedSpeedMultiplierForTimers;
        this.duration = duration;
        arrow.SetActive(true);
        EnemyScared(duration);
        base.Enable(duration);
        StartCoroutine(CountdownDuration(duration));
    }

    public override void Disable()
    {
        base.Disable();
        arrow.SetActive(false);
        StopAllCoroutines();
        body.color = originalBodyCol;
        enemy.managerOnRun.enemiesFreightened = false;
        enemy.managerOnRun.enemyPointMultiplier = 1;
    }

    private IEnumerator CountdownDuration(float duration)
    {
        durationLeft = duration;
        while (durationLeft > 0)
        {
            durationLeft--;
            yield return new WaitForSeconds(1);
        }
        
        yield return new WaitUntil((() => durationLeft<=0));
        durationLeft = 0;
    }

    public void EnemyScared(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(BodyFlash(duration));
    }
    /// <summary>
    /// Called when the shield is activated
    /// </summary>
    /// <returns></returns>
    public IEnumerator BodyFlash(float duration)
    {
        body.color = freightenedBodyCol;
        enemy.managerOnRun.enemiesFreightened = true;

        yield return new WaitForSeconds(duration - 2.5f);
        
        body.color = freightenedBodyCol;
        deadBody.color = freightenedBodyCol;
        arrow.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        GameManagerOnRun.instance.PlayAudioClip(GameManagerOnRun.instance.audio_powerupExpiring, 2);
        body.color = flashFreightenedBodyCol;
        deadBody.color = flashFreightenedBodyCol;
        arrow.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        body.color = freightenedBodyCol;
        deadBody.color = freightenedBodyCol;
        arrow.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        GameManagerOnRun.instance.PlayAudioClip(GameManagerOnRun.instance.audio_powerupExpiring, 2);
        body.color = flashFreightenedBodyCol;
        deadBody.color = flashFreightenedBodyCol;
        arrow.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        body.color = freightenedBodyCol;
        deadBody.color = freightenedBodyCol;
        arrow.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        GameManagerOnRun.instance.PlayAudioClip(GameManagerOnRun.instance.audio_powerupExpiring, 2);
        body.color = flashFreightenedBodyCol;
        deadBody.color = flashFreightenedBodyCol;
        arrow.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        body.color = freightenedBodyCol;
        deadBody.color = freightenedBodyCol;
        arrow.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        GameManagerOnRun.instance.PlayAudioClip(GameManagerOnRun.instance.audio_powerupExpiring, 2);
        body.color = flashFreightenedBodyCol;
        deadBody.color = flashFreightenedBodyCol;
        arrow.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        body.color = originalBodyCol;
        deadBody.color = originalBodyCol;
        enemy.managerOnRun.enemiesFreightened = false;
        arrow.SetActive(false);
    }

    public void Damaged()
    {
        dead = true;
        canExit = true;
        
        Vector3 pos = enemy.home.home.transform.position;
        pos.z = enemy.transform.position.z;

        enemy.transform.position = pos;

        body.enabled = false;
        deadBody.enabled = true;

        enemy.home.Enable(durationLeft);

        #region Elements

        //Rule Element

        if (GameManagerEditor.instance.spawnMoreEnemiesOnKill)
        {
            int Id =  enemy.gameObject.layer == 9? enemy.Id : Random.Range(0, 4);//checks if it's an ordinary enemy, otherwise it's player 2
            GameObject newEnemy = Instantiate(enemy.setUpMapManagerSetUp.enemiesPrefab[Id],
                enemy.home.home.position, Quaternion.identity, transform.parent.transform);
            Enemy newEnem_ = newEnemy.GetComponent<Enemy>();
            
            //Set up enemy
            newEnem_.target = enemy.setUpMapManagerSetUp.playerClone.transform;
            newEnem_.home.home = enemy.setUpMapManagerSetUp.enemiesHomePoints[0];
            newEnem_.home.exit = enemy.setUpMapManagerSetUp.enemiesHomePoints[1];
            newEnem_.scatter.scatterNode = enemy.setUpMapManagerSetUp.enemiesScatterPoints[enemy.Id];
            newEnem_.canMove = true;
            newEnem_.movement.direction = Vector2.right;
            if (newEnem_.chase.enemyType == EnemyChase.EnemyType.Inky)
            {
                newEnem_.chase.inkyDependant =
                    enemy.gameObject.layer == 9 ? enemy.chase.inkyDependant : enemy.gameObject;
            }
            
            //The enemy will start with being dead first
            newEnem_.freightened.dead = true;
            newEnem_.freightened.canExit = true;
            newEnem_.freightened.body.enabled = false;
            newEnem_.freightened.deadBody.enabled = true;
            newEnem_.home.Enable(duration);
            
            //Add the extra enemy to a list
            enemy.managerOnRun.enemies.Add(newEnemy.GetComponent<Enemy>());
            GameManagerEditor.instance.extraEnemies.Add(newEnemy);
        }

        //Goal Element
        if (GameManagerEditor.instance.killAllEnemies && AreAllEnemiesDead() && GameModeManager.gameMode == GameModeManager.GameMode.Classic)
        {
            enemy.managerOnRun.ShowScene(true);
        }
        
        //check if all enemies are eaten 
        bool AreAllEnemiesDead(){
            foreach(Enemy ball in enemy.managerOnRun.enemies) {
                if(!ball.GetComponent<EnemyFreightened>().dead) {
                    return false;
                }
            }
            return true;
        }
        
        #endregion

    }

    private void OnEnable()
    {
        if(!player)
            enemy.movement.speedMultiplier *= 0.5f;
        dead = false;
        body.enabled = true;
        deadBody.enabled = false;
        
        if (enemy.managerOnRun.player.alwaysInvincible)
        {
            Physics2D.IgnoreLayerCollision(8,9, false);//enable collision between enemy and player

        }
    }

    private void OnDisable()
    {
        enemy.movement.speedMultiplier = GameManagerEditor.instance.changedSpeedMultiplier;
        dead = false;
        
        body.enabled = true;
        deadBody.enabled = false;
        
        if (enemy.managerOnRun.player.alwaysInvincible)
        {
            Physics2D.IgnoreLayerCollision(8,9, true);//disable collision between enemy and player
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        
        Node node = col.GetComponent<Node>();
        if (node != null && enabled && !player)//if you hit a node calculate the longest pos from the player
        {
            CalculateDistToTarget(node, enemy.target.position, false, true);
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Spawn") && dead)
        {
            body.enabled = true;
            deadBody.enabled = false;
            Disable();
        }
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (enabled)
            {
                Damaged();
            }
        }
    }

}
