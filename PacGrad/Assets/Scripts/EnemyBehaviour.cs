using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public abstract class EnemyBehaviour : MonoBehaviour
{
   public Enemy enemy { get; private set; }
    public float duration;
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        enabled = false;
    }

    public void Enable()
    {
        Enable(duration);
    }

    public virtual void Enable(float duration)
    {
        enabled = true;
        CancelInvoke();
        Invoke(nameof(Disable), duration);
    }

    public virtual void Disable()
    {
        enabled = false;
        CancelInvoke();
    }
    
    /// <summary>
    /// Calculates the shortest distance to enemy's target
    /// </summary>
    /// <param name="node"></param>
    /// <param name="target"></param>
    public void CalculateShortestDistToTarget(Node node, Vector2 target)
    {
        List<float> distances = new List<float>();
        Vector2 shortestPos = new Vector2();
        List<Vector2> posAvailable = dirAvailable(node);
        int x = 0;
        int y = 0;
        
        for (int i = 0; i < posAvailable.Count; i++)
        {
            float distance = Vector2.Distance(posAvailable[i], target);
                    
            distances.Add(distance);
            if (distance == distances.Min())
            {
                shortestPos = posAvailable[i];
            } 
        }
        x = (int)(shortestPos.x - node.transform.position.x);
        y = (int)(shortestPos.y - node.transform.position.y);
                                        
        enemy.movement.SetDirection(new Vector2(x,y));

        this.target = target;
        this.shortestPos = shortestPos;
    }
    
    /// <summary>
    /// Get the directions available of the nodes except the one that turns the enemy backwards
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    List<Vector2> dirAvailable(Node node)
    {
        List<Vector2> directionsAvailable = new List<Vector2>();
        directionsAvailable.AddRange(node.availableDir);
        List<Vector2> posAvailable = new List<Vector2>();
        posAvailable.AddRange(node.availableDirPos);

        for (int i = 0; i < directionsAvailable.Count; i++) //make sure you don't count the backwards direction
        {
            if (directionsAvailable[i] == -enemy.movement.direction)
            {
                directionsAvailable.Remove(directionsAvailable[i]);
                posAvailable.RemoveAt(i);
            }
        }
        return posAvailable;
    }

    public void GoOppositeDir()
    {
        Vector2 oppositeDir = Vector2.zero;
        oppositeDir -= enemy.movement.direction;
        enemy.movement.SetDirection(oppositeDir);
    }

    private Vector2 target = Vector2.zero;
    private Vector2 shortestPos = Vector2.zero;
    private void OnDrawGizmos()
    {
        switch (GetComponent<EnemyChase>().enemyType)
        {
            case EnemyChase.EnemyType.Blinky:
                Gizmos.color = Color.red;
                break;
            case EnemyChase.EnemyType.Pinky:
                Gizmos.color = Color.magenta;
                break;
            case EnemyChase.EnemyType.Inky:
                Gizmos.color = Color.cyan;
                break;
            case EnemyChase.EnemyType.Clyde:
                Gizmos.color = Color.yellow;
                break;
        }
            
        Gizmos.DrawSphere(target, 0.2f);
        Gizmos.DrawLine(shortestPos,target);    
    }
    
}
