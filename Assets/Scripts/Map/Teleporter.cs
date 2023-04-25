using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform teleportTo;
    private void OnTriggerEnter2D(Collider2D col)
    {
        Vector3 pos = col.transform.position;
        pos.x = teleportTo.position.x;
        pos.y = teleportTo.position.y;

        col.transform.position = pos;

        Movement move = col.GetComponent<Movement>();
        if (move && move.gridMovement)
        {
            move.targetPosition = new Vector2(col.transform.position.x + move.direction.x, col.transform.position.y + move.direction.y);;
        }
    }
}
