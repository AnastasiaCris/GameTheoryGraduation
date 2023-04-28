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

    void Start()
    {
        protectionShield.enabled = false;
    }

    void Update()
    {
        if (ButtonPressed(Up))
        {
            /*StopCoroutine(GoInDirection(Vector2.down));
            StopCoroutine(GoInDirection(Vector2.left));
            StopCoroutine(GoInDirection(Vector2.right));
            StartCoroutine(GoInDirection(Vector2.up));*/
            movement.SetDirection(Vector2.up);
            
        }
        if (ButtonPressed(Down))
        {
            /*StopCoroutine(GoInDirection(Vector2.up));
            StopCoroutine(GoInDirection(Vector2.left));
            StopCoroutine(GoInDirection(Vector2.right));
            StartCoroutine(GoInDirection(Vector2.down));*/
            movement.SetDirection(Vector2.down);
            
        }
        if (ButtonPressed(Left))
        {
            /*StopCoroutine(GoInDirection(Vector2.down));
            StopCoroutine(GoInDirection(Vector2.up));
            StopCoroutine(GoInDirection(Vector2.right));
            StartCoroutine(GoInDirection(Vector2.left));*/
            movement.SetDirection(Vector2.left);
            
        }
        if (ButtonPressed(Right))
        {
            /*StopCoroutine(GoInDirection(Vector2.down));
            StopCoroutine(GoInDirection(Vector2.left));
            StopCoroutine(GoInDirection(Vector2.up));
            StartCoroutine(GoInDirection(Vector2.right));*/
            movement.SetDirection(Vector2.right);
            
        }
        
        //rotate player to look in the direction you're going
        //float angleDir = Mathf.Atan2(movement.direction.y, movement.direction.x);
       // transform.rotation = Quaternion.AngleAxis(angleDir * Mathf.Rad2Deg, Vector3.forward);
    }

    private bool ButtonPressed(KeyCode[] dir)
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
        StopCoroutine(SpeedExpiring(duration));
        StartCoroutine(SpeedExpiring(duration));
    }
    /// <summary>
    /// Called when the shield is activated
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpeedExpiring(float duration)
    {
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
        
        protectionShield.enabled = false;
        protectionShield.color = shieldColors[0];
        movement.speedMultiplier = 1;
    }
    
    private void StopSpeeding()
    {
        protectionShield.enabled = false;
        protectionShield.color = shieldColors[0];
        movement.speedMultiplier = 1;
    }
}
