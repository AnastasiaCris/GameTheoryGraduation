using UnityEngine;

public class EnemyFreightened : EnemyBehaviour
{
    public SpriteRenderer body;
    public SpriteRenderer deadBody;
    
    public Color originalBodyCol;
    public Color freightenedBodyCol;
    public Color flashFreightenedBodyCol;
    public Color bodyEaten;
    public bool eaten { get; private set; }
    private bool reachedHouse;

    public override void Enable(float duration)
    {
        base.Enable(duration);
        body.color = freightenedBodyCol;
        
        Invoke(nameof(Flash), duration/2);
    }

    public override void Disable()
    {
        base.Disable();
        body.color = originalBodyCol;
    }
    void Flash()
    {
        if (!eaten)
        {
            body.color = flashFreightenedBodyCol;
        }
    }

    private void Damaged()
    {
        eaten = true;

        Vector3 pos =  enemy.home.home.transform.position;
        pos.z = enemy.transform.position.z;

        enemy.transform.position = pos;
        
        body.enabled = false;
        deadBody.enabled = true;
        
        enemy.home.Enable(duration);
    }
    private void OnEnable()
    {
        enemy.movement.speedMultiplier = 0.5f;
        eaten = false;
        body.enabled = true;
        deadBody.enabled = false;
    }

    private void OnDisable()
    {
        enemy.movement.speedMultiplier = 1f;
        eaten = false;
        
        body.enabled = true;
        deadBody.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Node node = col.GetComponent<Node>();
        if (node != null && enabled)//if you hit a node calculate the longest pos from the player
        {
            CalculateDistToTarget(node, enemy.target.position, false);
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("Spawn") && eaten)
        {
            body.enabled = true;
            deadBody.enabled = false;
            Disable();
        }
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (enabled)
            {
                Damaged();
            }
        }
    }

}
