using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int points = 200;
    public Movement movement{ get; private set; }
    public EnemyHome home{ get; private set; }
    public EnemyScatter scatter{ get; private set; }
    public EnemyChase chase{ get; private set; }
    public EnemyFreightened freightened{ get; private set; }
    public EnemyBehaviour behaviour;
    public Transform target;
    public GameManager manager{ get; private set; }

    private void Awake()
    {
        movement = GetComponent<Movement>();
        home = GetComponent<EnemyHome>();
        scatter = GetComponent<EnemyScatter>();
        chase = GetComponent<EnemyChase>();
        freightened = GetComponent<EnemyFreightened>();
        home = GetComponent<EnemyHome>();
        manager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        gameObject.SetActive(true);
        movement.ResetState();
        
        freightened.Disable();
        chase.Disable();
        scatter.Enable();
        if (home != behaviour)
        {
            home.Disable();
        }

        if (behaviour != null)
        {
            behaviour.Enable();
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (freightened.enabled)
            {
                manager.EnemyDamaged(this);
            }
            else
            {
                manager.PlayerDamaged();
            }
        }
    }
}
