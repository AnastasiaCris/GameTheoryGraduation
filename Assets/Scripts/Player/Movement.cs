using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public bool gridMovement;
    public bool playerDead;
    public float speed = 8;
    public float speedMultiplier = 1;

    public Vector3 targetPosition;
    public Vector2 startDir;
    public LayerMask obstacleLayer;//check for raycasts
    public Rigidbody2D rb { get; private set; }
    public Vector2 direction;
    public  Vector2 nextDirection { get; private set; }
    public Vector3 startPos;

    public Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }


    void Update()
    {
        if (nextDirection != Vector2.zero)
        {
            SetDirection(nextDirection);
        }
    }

    public void ResetState()
    {
        speedMultiplier = GameManagerEditor.instance.changedSpeedMultiplier;
        direction = startDir;
        nextDirection = Vector2.zero;
        transform.position = startPos;
        if(gridMovement)
            targetPosition = transform.position;
        playerDead = false;
        rb.isKinematic = false;
        enabled = true;
    }

    private void FixedUpdate()
    {
        if (gridMovement)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition,
                speed * speedMultiplier * Time.fixedDeltaTime);

            if (transform.position == targetPosition)
            {
                if (!Ocuppied(direction))
                {
                    targetPosition = new Vector2(transform.position.x + direction.x, transform.position.y + direction.y);
                }
            }
        }
        else
        {
            Vector2 pos = rb.position;
            Vector2 translation = direction * speed * speedMultiplier * Time.fixedDeltaTime;
            rb.MovePosition(pos + translation);
        }

    }


    /// <summary>
    /// Set in which direction the object should move
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="forced"> Times for when the enemies should go through an obstacle</param>
    public void SetDirection(Vector2 dir, bool forced = false)
    {
        if (playerDead) return;
        
        //only set direction if it's possible to move there
        if (forced || !Ocuppied(dir))
        {
            direction = dir;
            nextDirection = Vector2.zero;

            if (dir == Vector2.right)
            {
                anim.SetInteger("PlayerAnim", 0);
            }else if (dir == Vector2.left)
            {
                anim.SetInteger("PlayerAnim", 1);
            }else if (dir == Vector2.up)
            {
                anim.SetInteger("PlayerAnim", 2);
            }else if (dir == Vector2.down)
            {
                anim.SetInteger("PlayerAnim", 3);
            }
        }
        else
        {
            nextDirection = dir;
        }
    }
    
    /// <summary>
    /// Check if the direction you want to go toward is ocuppied by a wall
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool Ocuppied(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.5f, 0.0f, dir, 1f, obstacleLayer);

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(transform.position + new Vector3(direction.x, direction.y, 0) * 1f, Vector2.one * 0.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position + new Vector3(nextDirection.x, nextDirection.y, 0) * 1f, Vector2.one * 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(targetPosition.x, targetPosition.y, 0), 0.25f);
    }

}
