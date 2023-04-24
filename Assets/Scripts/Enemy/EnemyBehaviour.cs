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
        duration *= GameManagerEditor.instance.changedSpeedMultiplier;
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
    /// Calculates the shortest/longest distance to enemy's target and goes in that direction
    /// </summary>
    /// <param name="node"></param>
    /// <param name="target"></param>
    public void CalculateDistToTarget(Node node, Vector2 target, bool shortest = true, bool canTurnBack = false)
    {
        List<float> distances = new List<float>();
        Vector2 desiredPos = new Vector2();

        List<Vector2> posAvailable = dirAvailable(node,canTurnBack);
        int x = 0;
        int y = 0;

        for (int i = 0; i < posAvailable.Count; i++)
        {
            float distance = Vector2.Distance(posAvailable[i], target);

            distances.Add(distance);
            if (shortest && distance == distances.Min())
            {
                desiredPos = posAvailable[i];
            }
            else if (!shortest && distance == distances.Max())
            {
                desiredPos = posAvailable[i];
            }
        }

        x = (int)(desiredPos.x - node.transform.position.x);
        y = (int)(desiredPos.y - node.transform.position.y);
        
        enemy.movement.SetDirection(new Vector2(x, y));

        this.target = target;
        goingToPos = desiredPos;
    }

    /// <summary>
    /// Get the directions available of the nodes except the one that turns the enemy backwards
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    List<Vector2> dirAvailable(Node node, bool canTurnBack)
    {
        List<Vector2> directionsAvailable = new List<Vector2>();
        directionsAvailable.AddRange(node.availableDir);
        List<Vector2> posAvailable = new List<Vector2>();
        posAvailable.AddRange(node.availableDirPos);

        if (!node.turnBack || canTurnBack)// if you can't turn back remove the backwards direction
        {
            for (int i = 0; i < directionsAvailable.Count; i++)
            {
                if (directionsAvailable[i] == -enemy.movement.direction)
                {
                    directionsAvailable.Remove(directionsAvailable[i]);
                    posAvailable.RemoveAt(i);
                }
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
    private Vector2 goingToPos = Vector2.zero;
    /*private void OnDrawGizmos()
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
        Gizmos.DrawLine(goingToPos,target);    
    }*/
    
}
