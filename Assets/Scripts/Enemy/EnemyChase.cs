using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : EnemyBehaviour
{
    [SerializeField]private GameObject blinky;
 public enum EnemyType
    {
        Blinky,
        Inky,
        Pinky,
        Clyde
    }

    public EnemyType enemyType;

    private void OnDisable()
    {
        //switch direction
        GoOppositeDir();
        enemy.scatter.Enable();
    }

    private Vector2 from;
    private Vector2 target = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D col)
    {
        Node node = col.GetComponent<Node>();
        if (node != null && enabled && !enemy.freightened.enabled)//if you hit a node and you're not in frightened mode
        {
            switch (enemyType)
            {
                case EnemyType.Blinky:
                    
                    target = enemy.target.position;
                    
                    break;
                
                ////////////////////
                
                case EnemyType.Pinky:
                    
                    target = Vector2.zero;
                    int n = 3;
                    var position = enemy.target.position;

                    //Calculating target
                    
                    if (GameManagerOnRun.instance.player.movement.direction == Vector2.up)
                    {
                        target = new Vector2(position.x - n, position.y + n);
                    }
                    else if (GameManagerOnRun.instance.player.movement.direction == Vector2.down)
                    {
                        target = new Vector2(position.x, position.y - n);
                    }
                    else if (GameManagerOnRun.instance.player.movement.direction == Vector2.left)
                    {
                        target = new Vector2(position.x - n, position.y);
                    }
                    else if (GameManagerOnRun.instance.player.movement.direction == Vector2.right)
                    {
                        target = new Vector2(position.x + n, position.y);
                    }

                    break;
                
                ///////////////////
                
                case EnemyType.Inky:

                    target = Vector2.zero;
                    position = enemy.target.position;
                    Vector2 playerDirByN = Vector2.zero;
                    n = 1;

                    //Calculating target
                    
                    if (GameManagerOnRun.instance.player.movement.direction == Vector2.up)
                    {
                        playerDirByN = new Vector2(position.x - n, position.y + n);

                    }
                    else if (GameManagerOnRun.instance.player.movement.direction == Vector2.down)
                    {
                        playerDirByN = new Vector2(position.x, position.y - n);
                    }
                    else if (GameManagerOnRun.instance.player.movement.direction == Vector2.left)
                    {
                        playerDirByN = new Vector2(position.x - n, position.y);
                    }
                    else if (GameManagerOnRun.instance.player.movement.direction == Vector2.right)
                    {
                        playerDirByN = new Vector2(position.x + n, position.y);
                    }

                    float xDist = playerDirByN.x - blinky.transform.position.x;
                    float yDist = playerDirByN.y - blinky.transform.position.y;

                    target = new Vector2(playerDirByN.x + xDist, playerDirByN.y + yDist);
                    
                    break;
                
                ////////////////////
                
                case EnemyType.Clyde:

                    //if clyde is farther by 8 tiles away from pacman, he has the same target as blinky
                    //if clyde is less than 8 tiles away from pacman, his target is a bottom left tile from scatter
                    if (Vector3.Distance(transform.position, enemy.target.position) > 8)
                    {
                        target = enemy.target.position;
                    }
                    else
                    {
                        target = GetComponent<EnemyScatter>().scatterNode.position;
                    }
                    
                    break;
            }
            CalculateShortestDistToTarget(node, target);
        }
    }

}
