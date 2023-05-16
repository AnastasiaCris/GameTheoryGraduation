using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour
{
    public int points = 10;
    protected GameManagerOnRun _managerOnRun;

    private void Awake()
    {
        _managerOnRun = FindObjectOfType<GameManagerOnRun>();
    }

    protected virtual void Collect()
    {
        _managerOnRun.PlayAudioClip(_managerOnRun.audio_coinCollected, 0);
        _managerOnRun.PointCollected(this);
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Collect();
        }
    }
}
