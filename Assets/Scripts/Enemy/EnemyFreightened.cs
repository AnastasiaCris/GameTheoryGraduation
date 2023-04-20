using UnityEngine;

public class EnemyFreightened : EnemyBehaviour
{
    [SerializeField] private int index;
    public SpriteRenderer body;
    public SpriteRenderer deadBody;
    
    public Color originalBodyCol;
    public Color freightenedBodyCol;
    public Color flashFreightenedBodyCol;
    public bool dead { get; private set; }
    public bool canExit;
    private bool reachedHouse;
    
    public override void Enable(float duration)
    {
        base.Enable(duration);
        body.color = freightenedBodyCol;
        enemy.managerOnRun.enemiesFreightened = true;
        
        Invoke(nameof(Flash), duration/2);
    }

    public override void Disable()
    {
        base.Disable();
        body.color = originalBodyCol;
        enemy.managerOnRun.enemiesFreightened = false;
    }
    void Flash()
    {
        if (!dead)
        {
            body.color = flashFreightenedBodyCol;
        }
    }

    private void Damaged()
    {
        dead = true;
        canExit = true;
        
        Vector3 pos = enemy.home.home.transform.position;
        pos.z = enemy.transform.position.z;

        enemy.transform.position = pos;

        body.enabled = false;
        deadBody.enabled = true;

        enemy.home.Enable(duration);

        #region Elements

        //Rule Element

        if (GameManagerEditor.instance.spawnMoreEnemiesOnKill)
        {
            GameObject newEnemy = Instantiate(enemy.enemyPrefab,
                enemy.mapSwitchManager.enemiesStartingPos[index].position, Quaternion.identity,
                transform.parent.transform);

            newEnemy.GetComponent<EnemyHome>().Disable();

            enemy.managerOnRun.enemies.Add(newEnemy.GetComponent<Enemy>());
        }

        //Goal Element
        if (GameManagerEditor.instance.killAllEnemies && AreAllEnemiesDead())
        {
            Time.timeScale = 0;
            enemy.managerOnRun.roundWonMenu.SetActive(true);
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
        enemy.movement.speedMultiplier = 0.5f;
        dead = false;
        body.enabled = true;
        deadBody.enabled = false;
    }

    private void OnDisable()
    {
        enemy.movement.speedMultiplier = 1f;
        dead = false;
        
        body.enabled = true;
        deadBody.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Node node = col.GetComponent<Node>();
        if (node != null && enabled)//if you hit a node calculate the longest pos from the player
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
