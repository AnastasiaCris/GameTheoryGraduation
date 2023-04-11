using UnityEngine;

public class EnemyScatter : EnemyBehaviour
{
    public Transform scatterNode; 

    private void OnDisable()
    {
        //switchDirection
        if(!enemy.freightened.enabled)
            GoOppositeDir();
        enemy.chase.Enable();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Node node = col.GetComponent<Node>();
        if (node != null && enabled && !enemy.freightened.enabled)//if you hit a node and you're in scattered and not in frightened mode
        {
            CalculateDistToTarget(node, scatterNode.position);
        }
    }

}
