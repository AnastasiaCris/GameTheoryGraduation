using UnityEngine;

public class EnemyScatter : EnemyBehaviour
{
    public Transform scatterNode; 

    private void OnDisable()
    {
        if (enemy.multiplayer)
            return;
        //switchDirection
        if(!enemy.freightened.enabled)
            GoOppositeDir();
        if(!enemy.managerOnRun.newRound)
            enemy.chase.Enable();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (enemy.multiplayer)
            return;
        Node node = col.GetComponent<Node>();
        
        if (node != null && enabled && !enemy.freightened.enabled)//if you hit a node and you're in scattered and not in frightened mode
        {
            CalculateDistToTarget(node, scatterNode.position);
            
        }
    }

}
