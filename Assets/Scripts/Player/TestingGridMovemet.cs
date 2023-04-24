using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingGridMovemet : MonoBehaviour
{
    public float speed = 4f;
    private float speedMultiplier = 1;
    public Vector2 direction;
    Vector3 targetPosition;
    private Rigidbody2D rb;

    private bool wantsToSwitchDir;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPosition = transform.position;
    }

    void Update()
    {
        Move();
    }

    private void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition,
            speed * speedMultiplier * Time.fixedDeltaTime);
        
        Collider2D overlap = Physics2D.OverlapPoint(new Vector2(transform.position.x + direction.x, transform.position.y + direction.y));

        if (transform.position == targetPosition && overlap == null)
        {
            targetPosition = new Vector2(transform.position.x + direction.x, transform.position.y + direction.y);
        }
    }

    void Move()
    {
        float Xinput = Input.GetAxisRaw("Horizontal");
        float Yinput = Input.GetAxisRaw("Vertical");

        if (Xinput != 0 || Yinput != 0)
        {
            Collider2D xOverlap =
                Physics2D.OverlapPoint(new Vector2(transform.position.x + Xinput, transform.position.y));
            Collider2D yOverlap =
                Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y + Yinput));
            

            // Horizontal Movement
            if (Xinput != 0 && xOverlap == null)
            {
                direction = Vector2.right * Xinput;
            }

            // Vertical movement
            if (Yinput != 0 && yOverlap == null)
            {
                direction = Vector2.up * Yinput;
            }
        }

       
    }
    
    // Display x & y OverlapPoints
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(new Vector3(transform.position.x + Input.GetAxisRaw("Horizontal"), transform.position.y), 0.25f);
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y + Input.GetAxisRaw("Vertical")), 0.25f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(targetPosition.x, targetPosition.y, 0), 0.25f);

    }
}
