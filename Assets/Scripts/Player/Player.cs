using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Movement movement { get; private set; }

    public KeyCode[] Up;
    public KeyCode[] Down;
    public KeyCode[] Left;
    public KeyCode[] Right;
    private void Awake()
    {
        movement = GetComponent<Movement>();
    }

    void Start()
    {
        
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

    private bool nodeHit;
    IEnumerator GoInDirection(Vector2 direction)
    {
        if (direction == -movement.direction)
        {
            movement.SetDirection(direction);
            yield break;
        }
            
        nodeHit = false;
        yield return new WaitUntil((() => nodeHit));
        movement.SetDirection(direction);
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Node node = other.GetComponent<Node>();
        if (node != null)
        {
            nodeHit = true;
        }
    }
}
