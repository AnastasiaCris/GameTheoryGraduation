using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Movement movement { get; private set; }

    public KeyCode[] Up;
    public KeyCode[] Down;
    public KeyCode[] Left;
    public KeyCode[] Right;

    public AnimationClip animDeath;
    public GameObject indicationKeys;
    public GameObject body;
    public float bigSize = 1.7f;

    public GameObject potionIndicator;
    
    //Formal elements
    public bool currentlyInvincible;
    public bool alwaysInvincible;
    public SpriteRenderer protectionShield;
    public Color[] shieldColors;
    private Enemy enemy;
    private void Awake()
    {
        movement = GetComponent<Movement>();
    }

    public virtual void Start()
    {
        protectionShield.enabled = false;
        enemy = GetComponent<Enemy>();
    }

    public virtual void Update()
    {
        
        if (GameManagerEditor.instance.onRunManager.gameOverMenu.activeSelf || GameManagerEditor.instance.onRunManager.roundWonMenu.activeSelf || GameManagerEditor.instance.onRunManager.sceneEnabled || enemy != null && !enemy.canMove || ConsoleDebug.instance.showDebug)
            return;
        
        if (ButtonPressed(Up))
        {
            movement.SetDirection(Vector2.up);
        }
        if (ButtonPressed(Down))
        {
            movement.SetDirection(Vector2.down);
            
        }
        if (ButtonPressed(Left))
        {
            movement.SetDirection(Vector2.left);
            
        }
        if (ButtonPressed(Right))
        {
            movement.SetDirection(Vector2.right);
            
        }
        
        if (indicationKeys.activeSelf && (ButtonPressed(Up) || ButtonPressed(Down) || ButtonPressed(Left) || ButtonPressed(Right)))
            indicationKeys.SetActive(false);
    }

    public bool ButtonPressed(KeyCode[] dir)
    {
        for (int i = 0; i < dir.Length; i++)
        {
            if (Input.GetKeyDown(dir[i]))
            {
                return true;
            }
        }
        return false;
    }
    
    public void ResetState()
    {
        movement.ResetState();
        gameObject.SetActive(true);
        movement.anim.SetInteger("PlayerAnim", 0);
        StopAllCoroutines();
        if (GameManagerEditor.instance.invincible)
        {
            StopInvincibility();
        }else if (GameManagerEditor.instance.speed)
        {
            StopSpeeding();
        }
    }

    /// <summary>
    /// Changes the size of the player during the effect
    /// </summary>
    /// <param name="duration">How long the effect lasts</param>
    public void CanKillEnemies(float duration)
    {
        duration *= GameManagerEditor.instance.changedSpeedMultiplierForTimers;
        StopCoroutine(CanKillEnemiesExpiring(duration));
        StartCoroutine(CanKillEnemiesExpiring(duration));
    }

    /// <summary>
    /// Changes the size of the player to show that the effect is running out
    /// </summary>
    /// <param name="duration">How long the effect lasts</param>
    /// <returns></returns>
    public IEnumerator CanKillEnemiesExpiring(float duration)
    {
        SpriteRenderer spriteRend = body.GetComponent<SpriteRenderer>();
        float normalSize = 1.4f;

        if(GameManagerEditor.instance.firstTime)
            potionIndicator.SetActive(true);
        
        body.transform.localScale = new Vector3(bigSize,bigSize,bigSize);
        spriteRend.color = shieldColors[2];
        
        yield return new WaitForSeconds(duration - 2.5f);
        
        body.transform.localScale = new Vector3(normalSize,normalSize,normalSize);
        spriteRend.color = Color.white;
        
        yield return new WaitForSeconds(0.3f);
        
        body.transform.localScale = new Vector3(bigSize,bigSize,bigSize);
        spriteRend.color = shieldColors[2];
        
        yield return new WaitForSeconds(0.3f);
        
        body.transform.localScale = new Vector3(normalSize,normalSize,normalSize);
        spriteRend.color = Color.white;
        
        yield return new WaitForSeconds(0.3f);
        
        body.transform.localScale = new Vector3(bigSize,bigSize,bigSize);
        spriteRend.color = shieldColors[2];
        
        yield return new WaitForSeconds(0.3f);
        
        body.transform.localScale = new Vector3(normalSize,normalSize,normalSize);
        spriteRend.color = Color.white;
        
        yield return new WaitForSeconds(0.3f);
        
        body.transform.localScale = new Vector3(bigSize,bigSize,bigSize);
        spriteRend.color = shieldColors[2];
        
        yield return new WaitForSeconds(0.3f);
        
        body.transform.localScale = new Vector3(normalSize,normalSize,normalSize);
        spriteRend.color = Color.white;
        
        yield return new WaitForSeconds(0.3f);
        
        body.transform.localScale = new Vector3(bigSize,bigSize,bigSize);
        spriteRend.color = shieldColors[2];
        
        yield return new WaitForSeconds(0.3f);
        
        body.transform.localScale = new Vector3(normalSize,normalSize,normalSize);
        spriteRend.color = Color.white;
        if(GameManagerEditor.instance.firstTime)
            potionIndicator.SetActive(false);
    }

    public void InvincibilityDebug(bool inv)
    {
        alwaysInvincible = inv;
        
        if (alwaysInvincible)
        {
            Physics2D.IgnoreLayerCollision(8,9, true);//disable collision between enemy and player
        }
        else
        {
            Physics2D.IgnoreLayerCollision(8,9, false);//enable collision between enemy and player
        }
        
    }

    public void Invincibility(float duration)
    {
        duration *= GameManagerEditor.instance.changedSpeedMultiplierForTimers;
        StopCoroutine(InvincibilityExpiring(duration));
        StartCoroutine(InvincibilityExpiring(duration));
    }
    /// <summary>
    /// Called when the shield is activated
    /// </summary>
    /// <returns></returns>
    public IEnumerator InvincibilityExpiring(float duration)
    {
        currentlyInvincible = true;
        protectionShield.enabled = true;
        Physics2D.IgnoreLayerCollision(8,9, true);//disable collision between enemy and player
        if(GameManagerEditor.instance.multiplayer)
            Physics2D.IgnoreLayerCollision(8,11, true);//disable collision between player 1 and player 2

        yield return new WaitForSeconds(duration - 2.5f);
        
        protectionShield.enabled = false;
        yield return new WaitForSeconds(0.3f);
        GameManagerOnRun.instance.PlayAudioClip(GameManagerOnRun.instance.audio_powerupExpiring, 2);
        protectionShield.enabled = true;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = false;
        yield return new WaitForSeconds(0.3f);
        GameManagerOnRun.instance.PlayAudioClip(GameManagerOnRun.instance.audio_powerupExpiring, 2);
        protectionShield.enabled = true;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = false;
        yield return new WaitForSeconds(0.3f);
        GameManagerOnRun.instance.PlayAudioClip(GameManagerOnRun.instance.audio_powerupExpiring, 2);
        protectionShield.enabled = true;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = false;
        yield return new WaitForSeconds(0.3f);
        GameManagerOnRun.instance.PlayAudioClip(GameManagerOnRun.instance.audio_powerupExpiring, 2);
        protectionShield.enabled = true;
        yield return new WaitForSeconds(0.3f);
        
        StopInvincibility();
    }

    private void StopInvincibility()
    {
        protectionShield.enabled = false;
        currentlyInvincible = false;
        Physics2D.IgnoreLayerCollision(8,9, false);//enable collision between enemy and player
        if(GameManagerEditor.instance.multiplayer)
            Physics2D.IgnoreLayerCollision(8,11, false);//enable collision between player 1 and player 2
    }

    public void SpeedIncrease(float duration)
    {
        duration *= GameManagerEditor.instance.changedSpeedMultiplierForTimers;
        StopAllCoroutines();
        StartCoroutine(SpeedExpiring(duration));
    }
    /// <summary>
    /// Called when the shield is activated
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpeedExpiring(float duration)
    {
        movement.speedMultiplier = GameManagerEditor.instance.changedSpeedMultiplier;
        movement.speedMultiplier *= 1.5f;
        protectionShield.enabled = true;
        protectionShield.color = shieldColors[1];
        yield return new WaitForSeconds(duration - 2.5f);
        
        protectionShield.enabled = false;
        yield return new WaitForSeconds(0.3f);
        GameManagerOnRun.instance.PlayAudioClip(GameManagerOnRun.instance.audio_powerupExpiring, 2);
        protectionShield.enabled = true;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = false;
        yield return new WaitForSeconds(0.3f);
        GameManagerOnRun.instance.PlayAudioClip(GameManagerOnRun.instance.audio_powerupExpiring, 2);
        protectionShield.enabled = true;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = false;
        yield return new WaitForSeconds(0.3f);
        GameManagerOnRun.instance.PlayAudioClip(GameManagerOnRun.instance.audio_powerupExpiring, 2);
        protectionShield.enabled = true;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = false;
        yield return new WaitForSeconds(0.3f);
        GameManagerOnRun.instance.PlayAudioClip(GameManagerOnRun.instance.audio_powerupExpiring, 2);
        protectionShield.enabled = true;
        yield return new WaitForSeconds(0.3f);
        
        StopSpeeding();
    }
    
    private void StopSpeeding()
    {
        protectionShield.enabled = false;
        protectionShield.color = shieldColors[0];
        movement.speedMultiplier = GameManagerEditor.instance.changedSpeedMultiplier;;
    }

    /*public int typeEnemyGizmos = 0;
    public Vector3 targetGizmos = Vector3.zero;
    public GameObject inkyDependant;
    public GameObject OrangeEnemy;
    private void OnDrawGizmos()
    {

        switch (typeEnemyGizmos)
        {
            case 0:
                Gizmos.color = Color.red;

                targetGizmos = transform.position;
                break;
            case 1:
                Gizmos.color = Color.gray;

                int n = 3;
                if (movement.direction == Vector2.up)
                {
                    targetGizmos = new Vector2(transform.position.x - n, transform.position.y + n);
                    
                    Gizmos.DrawLine(transform.position, new Vector3(transform.position.x,transform.position.y + n));
                    Gizmos.DrawLine(new Vector3(transform.position.x,transform.position.y + n), targetGizmos);
                }
                else if (movement.direction == Vector2.down)
                {
                    targetGizmos = new Vector2(transform.position.x, transform.position.y - n);
                    Gizmos.DrawLine(transform.position, targetGizmos);
                }
                else if (movement.direction == Vector2.left)
                {
                    targetGizmos = new Vector2(transform.position.x - n, transform.position.y);
                    Gizmos.DrawLine(transform.position, targetGizmos);
                }
                else if (movement.direction == Vector2.right)
                {
                    targetGizmos = new Vector2(transform.position.x + n, transform.position.y);
                    Gizmos.DrawLine(transform.position, targetGizmos);
                }
                Gizmos.color = Color.magenta;

                break;
            case 2:
                Gizmos.color = Color.gray;
                Vector2 playerDirByN = Vector2.zero;
                n = 1;

                //Calculating target
                    
                if (GameManagerOnRun.instance.player.movement.direction == Vector2.up)
                {
                    playerDirByN = new Vector2(transform.position.x - n, transform.position.y + n);
                    Gizmos.DrawLine(transform.position, new Vector3(transform.position.x,transform.position.y + n));
                    Gizmos.DrawLine( new Vector3(transform.position.x,transform.position.y + n), playerDirByN);
                }
                else if (GameManagerOnRun.instance.player.movement.direction == Vector2.down)
                {
                    playerDirByN = new Vector2(transform.position.x, transform.position.y - n);
                    Gizmos.DrawLine(transform.position, playerDirByN);

                }
                else if (GameManagerOnRun.instance.player.movement.direction == Vector2.left)
                {
                    playerDirByN = new Vector2(transform.position.x - n, transform.position.y);
                    Gizmos.DrawLine(transform.position, playerDirByN);

                }
                else if (GameManagerOnRun.instance.player.movement.direction == Vector2.right)
                {
                    playerDirByN = new Vector2(transform.position.x + n, transform.position.y);
                    Gizmos.DrawLine(transform.position, playerDirByN);
                }

                float xDist = playerDirByN.x - inkyDependant.transform.position.x;
                float yDist = playerDirByN.y - inkyDependant.transform.position.y;
                
                targetGizmos = new Vector2(playerDirByN.x + xDist, playerDirByN.y + yDist);
                
                Gizmos.DrawLine(inkyDependant.transform.position, targetGizmos);
                
                Gizmos.color = Color.cyan;

                break;
            case 3:
                Gizmos.color = Color.gray;
                if (Vector3.Distance(transform.position, OrangeEnemy.transform.position) > 8)
                {
                    targetGizmos = transform.position;
                }
                else
                {
                    targetGizmos = OrangeEnemy.GetComponent<EnemyScatter>().scatterNode.position;
                }
                
                Gizmos.DrawWireSphere(transform.position, 8);
                Gizmos.color = Color.yellow;
                break;
        }
        
        Gizmos.DrawSphere(targetGizmos, 0.2f);
    }*/
}
