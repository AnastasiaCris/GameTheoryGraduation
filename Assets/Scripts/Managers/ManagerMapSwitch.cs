using UnityEngine;

public class ManagerMapSwitch : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject playerClone{ get; private set; }
    public GameObject[] enemiesPrefab;
    public GameObject[] enemiesClone{ get; private set; }
    
    public Transform playerStartingPos;
    public Transform[] enemiesStartingPos;
    public Transform[] enemiesScatterPoints;
    public Transform[] enemiesHomePoints = new Transform[2];

    [SerializeField] private GameManagerOnRun gameManagerOnRun;
    void Awake()
    {
        //Instantiating the player and enemies
        playerClone = Instantiate(playerPrefab, playerStartingPos.position, Quaternion.identity, transform.parent.transform);
        
        enemiesClone = new GameObject[4];
        for (int i = 0; i < enemiesPrefab.Length; i++)
        {
             enemiesClone[i] = Instantiate(enemiesPrefab[i], enemiesStartingPos[i].position, Quaternion.identity,transform.parent.transform);
        }
        
        //Setting up the enemies
        for (int i = 0; i < enemiesClone.Length; i++)
        {
            enemiesClone[i].GetComponent<Enemy>().target = playerClone.transform;
            enemiesClone[i].GetComponent<EnemyHome>().home = enemiesHomePoints[0];
            enemiesClone[i].GetComponent<EnemyHome>().exit = enemiesHomePoints[1];
            enemiesClone[i].GetComponent<EnemyScatter>().scatterNode = enemiesScatterPoints[i];
            enemiesClone[i].GetComponent<EnemyChase>().blinky = enemiesClone[1];
        }

        //Setting up the core game manager
        gameManagerOnRun.player = playerClone.GetComponent<Player>();
        gameManagerOnRun.enemies = new Enemy[4];
        for (int i = 0; i < enemiesClone.Length; i++)
        {
            gameManagerOnRun.enemies[i] = enemiesClone[i].GetComponent<Enemy>();
        }
        
    }

}
