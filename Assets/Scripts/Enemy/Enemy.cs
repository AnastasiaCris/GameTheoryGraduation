using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int Id = 0;
    public int points = 200;
    public Movement movement{ get; private set; }
    public EnemyHome home{ get; private set; }
    public EnemyScatter scatter{ get; private set; }
    public EnemyChase chase{ get; private set; }
    public EnemyFreightened freightened{ get; private set; }
    public EnemyBehaviour behaviour;
    public Transform target;
    public GameManagerOnRun managerOnRun{ get; private set; }
    public ManagerMapSwitch mapSwitchManager{ get; private set; }


    private void Awake()
    {
        movement = GetComponent<Movement>();
        home = GetComponent<EnemyHome>();
        scatter = GetComponent<EnemyScatter>();
        chase = GetComponent<EnemyChase>();
        freightened = GetComponent<EnemyFreightened>();
        home = GetComponent<EnemyHome>();
        managerOnRun = FindObjectOfType<GameManagerOnRun>();
        mapSwitchManager = FindObjectOfType<ManagerMapSwitch>();
    }

    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        managerOnRun.newRound = true;
        gameObject.SetActive(true);
        movement.ResetState();

        freightened.Disable();
        chase.Disable();
        scatter.Disable();

        if (behaviour != null)
        {
            behaviour.Enable();
        }

        managerOnRun.newRound = false;
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
