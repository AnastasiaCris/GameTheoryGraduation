using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    public List<Vector2> availableDir = new List<Vector2>();
    public List<Vector2> availableDirPos = new List<Vector2>();
    public LayerMask obstacleLayer;

    void Start()
    {
        CheckForDirectionAvailability(Vector2.up);
        CheckForDirectionAvailability(Vector2.down);
        CheckForDirectionAvailability(Vector2.left);
        CheckForDirectionAvailability(Vector2.right);
    }

    private void CheckForDirectionAvailability(Vector2 dir)
    {
        Vector2 availablePos = Vector2.zero;
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.5f, 0.0f, dir, 1, obstacleLayer);

        if (hit.collider == null)
        {
            availableDir.Add(dir);
            
            availablePos = new Vector2(transform.position.x + dir.x, transform.position.y + dir.y);
            availableDirPos.Add(availablePos);
        }
    }
    
}
