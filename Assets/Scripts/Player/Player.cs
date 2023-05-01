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
    
    //Formal elements
    public bool currentlyInvincible;
    public SpriteRenderer protectionShield;
    public Color[] shieldColors;
    private void Awake()
    {
        movement = GetComponent<Movement>();
    }

    public virtual void Start()
    {
        protectionShield.enabled = false;
    }

    public virtual void Update()
    {
        //if (GameManagerEditor.instance.onRunManager.gameOverMenu.activeSelf || GameManagerEditor.instance.onRunManager.roundWonMenu.activeSelf || GameManagerEditor.instance.onRunManager.sceneEnabled)
           // return;
        
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
        
        yield return new WaitForSeconds(duration - 2.5f);
        
        protectionShield.enabled = false;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = true;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = false;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = true;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = false;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = true;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = false;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = true;
        yield return new WaitForSeconds(0.3f);
        
        StopInvincibility();
    }

    private void StopInvincibility()
    {
        protectionShield.enabled = false;
        currentlyInvincible = false;
        Physics2D.IgnoreLayerCollision(8,9, false);//enable collision between enemy and player
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
        protectionShield.enabled = true;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = false;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = true;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = false;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = true;
        yield return new WaitForSeconds(0.3f);
        protectionShield.enabled = false;
        yield return new WaitForSeconds(0.3f);
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
}
