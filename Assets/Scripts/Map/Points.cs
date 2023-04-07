using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour
{
    public int points = 10;

    protected virtual void Collect()
    {
        FindObjectOfType<GameManagerOnRun>().PointCollected(this);
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Collect();
        }
    }
}
