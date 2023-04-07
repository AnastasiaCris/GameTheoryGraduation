using System;
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
        
        //rotate player to look in the direction you're going
        float angleDir = Mathf.Atan2(movement.direction.y, movement.direction.x);
        transform.rotation = Quaternion.AngleAxis(angleDir * Mathf.Rad2Deg, Vector3.forward);
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
}
