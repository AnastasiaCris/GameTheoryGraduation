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
    public GameManagerOnRun managerOnRun{ get; private set; }

    private void Awake()
    {
        movement = GetComponent<Movement>();
        home = GetComponent<EnemyHome>();
        scatter = GetComponent<EnemyScatter>();
        chase = GetComponent<EnemyChase>();
        freightened = GetComponent<EnemyFreightened>();
        home = GetComponent<EnemyHome>();
        managerOnRun = FindObjectOfType<GameManagerOnRun>();
      
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
                managerOnRun.EnemyDamaged(this);
            }
            else
            {
                managerOnRun.PlayerDamaged();
            }
        }
    }
}
