using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int Id = 0;
    public int typeAmount = 0;
    public int points = 200;
    public Movement movement{ get; private set; }
    public EnemyHome home{ get; private set; }
    public EnemyScatter scatter{ get; private set; }
    public EnemyChase chase{ get; private set; }
    public EnemyFreightened freightened{ get; private set; }
    public Transform target;
    public GameManagerOnRun managerOnRun{ get; private set; }
    public ManagerSetUpMap setUpMapManagerSetUp{ get; private set; }

    public bool multiplayer;
    public bool canMove;
    
    private void Awake()
    {
        movement = GetComponent<Movement>();
        home = GetComponent<EnemyHome>();
        scatter = GetComponent<EnemyScatter>();
        chase = GetComponent<EnemyChase>();
        freightened = GetComponent<EnemyFreightened>();
        home = GetComponent<EnemyHome>();
        managerOnRun = FindObjectOfType<GameManagerOnRun>();
        setUpMapManagerSetUp = FindObjectOfType<ManagerSetUpMap>();
        
    }

    public void ResetState()
    {
        managerOnRun.newRound = true;
        gameObject.SetActive(true);
        movement.ResetState();

        freightened.Disable();
        chase.Disable();
        scatter.Disable();
        home.Enable();
        
        managerOnRun.newRound = false;
        canMove = true;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (freightened.enabled)
            {
                managerOnRun.EnemyDamaged(this);
            }
            else if(!freightened.enabled && !col.gameObject.GetComponent<Player>().currentlyInvincible)
            {
                managerOnRun.PlayerDamaged();
            }
        }
    }
}
