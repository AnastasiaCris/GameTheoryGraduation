using UnityEngine;

public class ManagerMapSwitch : MonoBehaviour
{
    [Header("Player")][Space]
    public GameObject playerPrefab;
    public GameObject playerClone{ get; private set; }
    
    [Space][Header("Enemies")][Space]
    public GameObject[] enemiesPrefab;
    public GameObject[] enemiesClone{ get; private set; }
    
    private int typeRed;
    private int typePink;
    private int typeBlue;
    private int typeOrange;

    private int timeInterval = 1;
    
    [Space][Header("Starting Positions")][Space]
    public Transform playerStartingPos;
    public Transform[] enemiesStartingPos;
    public Transform[] enemiesScatterPoints;
    public Transform[] enemiesHomePoints = new Transform[2];

    [SerializeField] private GameManagerOnRun gameManagerOnRun;

    public static ManagerMapSwitch instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        //Instantiating the player
        playerClone = Instantiate(playerPrefab, playerStartingPos.position, Quaternion.identity,
            transform.parent.transform.parent.transform);

        //Setting up the core game manager
        gameManagerOnRun.player = playerClone.GetComponent<Player>();
        
        //Set up the enemies
        SetUpForNewEnemies(GameManagerEditor.instance.maxEnemies);
        
        for (int i = 0; i < GameManagerEditor.instance.maxEnemies; i++)
        {
            SetUpIndividualEnemies(GameManagerEditor.instance.enemyTypes[i], i);
        }
    }

    public void DeleteAllEnemies()
    {
        if (enemiesClone.Length > 0)
        {
            for (int i = 0; i < enemiesClone.Length; i++)
            {
                Destroy(enemiesClone[i]);
                gameManagerOnRun.enemies.Clear();
            }
            
        }
        enemiesClone = new GameObject[0];
    }

    public void SetUpForNewEnemies(int nrOfEnemies)
    {
        enemiesClone = new GameObject[nrOfEnemies];
        for (int i = 0; i < nrOfEnemies; i++)
        {
            gameManagerOnRun.enemies.Add(new Enemy());
        }

         typeRed = 0;
         typePink = 0;
         typeBlue = 0;
         typeOrange = 0;
    }


    public void SetUpIndividualEnemies(int enemyType, int nrInTheList)
    {
        if (enemiesClone[nrInTheList] != null) //if this gameobject is null then continue, otherwise delete it first
        {
            Destroy(enemiesClone[nrInTheList]);
        }

        GameObject enemyClone = Instantiate(enemiesPrefab[enemyType], enemiesStartingPos[enemyType].position,
            Quaternion.identity,
            transform.parent.transform.parent.transform);

        enemiesClone[nrInTheList] = enemyClone;

        //Set Up the enemy
        Enemy enemy = enemyClone.GetComponent<Enemy>();
        EnemyHome enemyHome = enemyClone.GetComponent<EnemyHome>();

        enemy.target = playerClone.transform;
        enemyHome.home = enemiesHomePoints[0];
        enemyHome.exit = enemiesHomePoints[1];
        enemyClone.GetComponent<EnemyScatter>().scatterNode = enemiesScatterPoints[enemy.Id];
        enemyClone.GetComponent<EnemyChase>().blinky = enemiesClone[0];

        //if the enemy you added right now has a similar type as another one in the list then increase it's home duration
        switch (enemyType)
        {
            case 0:
                enemyHome.duration += typeRed * timeInterval;
                typeRed++;
                break;
            case 1:
                enemyHome.duration += typePink * timeInterval;
                typePink++;
                break;
            case 2:
                enemyHome.duration += typeBlue * timeInterval;
                typeBlue++;
                break;
            case 3:
                enemyHome.duration += typeOrange * timeInterval;
                typeOrange++;
                break;
        }

        //Setting up the core game manager
        gameManagerOnRun.enemies[nrInTheList] = enemyClone.GetComponent<Enemy>();
        
        GameManagerEditor.instance.enemyTypes[nrInTheList] = enemyType;

    }

}
