using System.Collections;
using UnityEngine;

public class EnemyHome : EnemyBehaviour
{
    [HideInInspector]public Transform home;
    [HideInInspector]public Transform exit;
    public bool blinky;
    private void OnEnable()
    {
        StopAllCoroutines();
        if(blinky && !enemy.freightened.canExit)
            enemy.movement.direction = Vector2.zero;
    }

    private void OnDisable()
    {
        if(gameObject.activeSelf)
            StartCoroutine(ExitHouse());
        enemy.scatter.Enable();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!blinky || enemy.freightened.canExit)
        {
            if (enabled && col.gameObject.layer == LayerMask.NameToLayer("Ostacle"))
            {
                enemy.movement.SetDirection(-enemy.movement.direction);
            }
        }
    }

    private IEnumerator ExitHouse()
    {
        enemy.movement.SetDirection(Vector2.up, true);
        enemy.movement.rb.isKinematic = true;
        enemy.movement.enabled = false;

        Vector3 position = transform.position;
        float duration = 0.5f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            Vector3 newPos = Vector3.Lerp(position, home.position, elapsed / duration);
            newPos.z = position.z;
            enemy.transform.position = newPos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0;

        while (elapsed < duration)
        {
            Vector3 newPos = Vector3.Lerp(home.position, exit.position, elapsed / duration);
            newPos.z = position.z;
            enemy.transform.position = newPos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        enemy.movement.SetDirection(new Vector2(Random.value < 0.5f ? -1.0f : 1.0f, 0), true);
        enemy.movement.rb.isKinematic = false;
        enemy.movement.enabled = true;
        enemy.freightened.body.enabled = true;
        enemy.freightened.deadBody.enabled = false;
        enemy.freightened.canExit = false;
        enemy.freightened.dead = false;

    }
}
