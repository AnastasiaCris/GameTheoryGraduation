using System.Collections;
using UnityEngine;

public class EnemyHome : EnemyBehaviour
{
    public Transform home;
    public Transform exit;

    private void OnEnable()
    {
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        if(gameObject.activeSelf)
            StartCoroutine(ExitHouse());
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (enabled && col.gameObject.layer == LayerMask.NameToLayer("Ostacle"))
        {
            enemy.movement.SetDirection(-enemy.movement.direction);
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
            Vector3 newPos = Vector3.Lerp(position, home.position, elapsed/duration);
            newPos.z = position.z;
            enemy.transform.position = newPos;
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0;
        
        while (elapsed < duration)
        {
            Vector3 newPos = Vector3.Lerp(home.position, exit.position, elapsed/duration);
            newPos.z = position.z;
            enemy.transform.position = newPos;
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        enemy.movement.SetDirection(new Vector2(Random.value < 0.5f ? -1.0f : 1.0f, 0), true);
        enemy.movement.rb.isKinematic = false;
        enemy.movement.enabled = true;
    }
}
