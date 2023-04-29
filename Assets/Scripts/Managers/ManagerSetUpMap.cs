using UnityEngine;

public class ManagerSetUpMap : MonoBehaviour
{
    [Header("Player")][Space]
    public GameObject playerPrefab;
    [SerializeField] bool gridMovement;
    public GameObject playerClone{ get; private set; }
    
    [Space][Header("Enemies")][Space]
    public GameObject[] enemiesPrefab;
    public GameObject[] enemiesGameobjectClone{ get; private set; }
    
    private int typeRed;
    private int typePink;
    private int typeBlue;
    private int typeOrange;
    
    [Space][Header("Starting Positions")][Space]
    public Transform playerStartingPos;
    public Transform[] enemiesStartingPos;
    public Transform[] enemiesScatterPoints;
    public Transform[] enemiesHomePoints = new Transform[2];

    [SerializeField] private GameManagerOnRun gameManagerOnRun;

    public static ManagerSetUpMap instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        GameManagerEditor.instance.onSetUpMap = this;

        
        //Instantiating the player
        playerClone = Instantiate(playerPrefab, playerStartingPos.position, Quaternion.identity,
            transform.parent.transform.parent.transform);

        //Setting up the core game manager
        gameManagerOnRun.player = playerClone.GetComponent<Player>();

        if (gridMovement)
            gameManagerOnRun.player.movement.gridMovement = true;
        
        //Set up the enemies
        SetUpForNewEnemies(GameManagerEditor.instance.maxEnemies);
        
        for (int i = 0; i < GameManagerEditor.instance.maxEnemies; i++)
        {
            SetUpIndividualEnemies(GameManagerEditor.instance.enemyTypes[i], i);
        }
        if(enemiesGameobjectClone[0].GetComponent<EnemyChase>().blinky == null)//if blinky is not set up
            AfterAllEnemiesInstantiated();
    }

    public void DeleteAllEnemies()
    {
        if (enemiesGameobjectClone.Length > 0)
        {
            for (int i = 0; i < enemiesGameobjectClone.Length; i++)
            {
                Destroy(enemiesGameobjectClone[i]);
                gameManagerOnRun.enemies.Clear();
            }
            
        }
        enemiesGameobjectClone = new GameObject[0];
    }

    public void SetUpForNewEnemies(int nrOfEnemies)
    {
        enemiesGameobjectClone = new GameObject[nrOfEnemies];
        for (int i = 0; i < nrOfEnemies; i++)
        {
            gameManagerOnRun.enemies.Add(new Enemy());
        }
        
        if (GameManagerEditor.instance.extraEnemies.Count > 0) //if there are any extra enemies, delete them
        {
            for (int i = 0; i < GameManagerEditor.instance.extraEnemies.Count; i++)
            {
                Destroy(GameManagerEditor.instance.extraEnemies[i]);
            }
            GameManagerEditor.instance.extraEnemies.Clear();
        }

         typeRed = 0;
         typePink = 0;
         typeBlue = 0;
         typeOrange = 0;
    }


    public void SetUpIndividualEnemies(int enemyType, int nrInTheList)
    {
        if (GameManagerEditor.instance.extraEnemies.Count > 0) //if there are any extra enemies, delete them
        {
            for (int i = 0; i < GameManagerEditor.instance.extraEnemies.Count; i++)
            {
                Destroy(GameManagerEditor.instance.extraEnemies[i]);
            }
            GameManagerEditor.instance.extraEnemies.Clear();
        }
        if (enemiesGameobjectClone[nrInTheList] != null) //if this gameobject is null then continue, otherwise delete it first
        {
            //check first which type of enemy it is
            switch (enemiesGameobjectClone[nrInTheList].GetComponent<EnemyChase>().enemyType)
            {
                case EnemyChase.EnemyType.Blinky:
                    typeRed--;
                    break;
                case EnemyChase.EnemyType.Pinky:
                    typePink--;
                    break;
                case EnemyChase.EnemyType.Inky:
                    typeBlue--;
                    break;
                case EnemyChase.EnemyType.Clyde:
                    typeOrange--;
                    break;
            }
            Destroy(enemiesGameobjectClone[nrInTheList]);
        }
        
        GameObject enemyClone = Instantiate(enemiesPrefab[enemyType], enemiesStartingPos[enemyType].position, Quaternion.identity, transform.parent.transform.parent.transform);

        enemiesGameobjectClone[nrInTheList] = enemyClone;

        //Set Up the enemy
        Enemy enemy = enemyClone.GetComponent<Enemy>();

        enemy.target = playerClone.transform;
        enemy.home.home = enemiesHomePoints[0];
        enemy.home.exit = enemiesHomePoints[1];
        enemy.scatter.scatterNode = enemiesScatterPoints[enemy.Id];
        
        //Setting up multiple enemies of the same type
        int randomNrx;
        int randomNry;

        switch (enemyType) //if the enemy you added right now has a similar type as another one in the list then increase it's home duration
        {
            case 0:
                if (Random.value < 0.5f)
                {
                    randomNrx = Random.Range(-4, 0);
                }
                else
                {
                    randomNrx = Random.Range(1, 5);
                }
                
                if (typeRed == 0)
                {
                    randomNrx = 0;
                }
                
                enemyClone.transform.position = new Vector3(enemyClone.transform.position.x + randomNrx, enemyClone.transform.position.y);
                enemy.movement.startPos = enemyClone.transform.position;
                
                enemy.home.duration += typeRed;
                typeRed++;
                break;
            case 1:
                if (Random.value < 0.5f)
                {
                    randomNrx = Random.Range(-1, 0);
                }
                else
                {
                    randomNrx = Random.Range(1, 2);
                }
                if (Random.value < 0.5f)
                {
                    randomNry = Random.Range(-1, 0);
                }
                else
                {
                    randomNry = Random.Range(1, 2);
                }
                
                if (typePink == 0)
                {
                    randomNrx = 0;
                    randomNry = 0;
                }
                enemyClone.transform.position = new Vector3(enemyClone.transform.position.x + randomNrx, enemyClone.transform.position.y + randomNry);
                enemy.movement.startPos = enemyClone.transform.position;
                
                enemy.home.duration += typePink;
                typePink++;
                break;
            case 2:
                if (Random.value < 0.5f)
                {
                    randomNrx = Random.Range(-1, 0);
                }
                else
                {
                    randomNrx = Random.Range(1, 2);
                }
                if (Random.value < 0.5f)
                {
                    randomNry = Random.Range(-1, 0);
                }
                else
                {
                    randomNry = Random.Range(1, 2);
                }
                
                if (typeBlue == 0)
                {
                    randomNrx = 0;
                    randomNry = 0;
                }
                enemyClone.transform.position = new Vector3(enemyClone.transform.position.x + randomNrx,
                    enemyClone.transform.position.y + randomNry);

                enemy.movement.startPos = enemyClone.transform.position;
                
                enemy.home.duration += typeBlue ;
                typeBlue++;
                break;
            case 3:
                if (Random.value < 0.5f)
                {
                    randomNrx = Random.Range(-1, 0);
                }
                else
                {
                    randomNrx = Random.Range(1, 2);
                }
                if (Random.value < 0.5f)
                {
                    randomNry = Random.Range(-1, 0);
                }
                else
                {
                    randomNry = Random.Range(1, 2);
                }
                
                if (typeOrange == 0)
                {
                    randomNrx = 0;
                    randomNry = 0;
                }
                enemyClone.transform.position = new Vector3(enemyClone.transform.position.x + randomNrx,
                    enemyClone.transform.position.y + randomNry);
                enemy.movement.startPos = enemyClone.transform.position;
                
                enemy.home.duration += typeOrange;
                typeOrange++;
                break;
        }

        //Setting up the core game manager
        gameManagerOnRun.enemies[nrInTheList] = enemy;
        
        GameManagerEditor.instance.enemyTypes[nrInTheList] = enemyType;

    }

    public void AfterAllEnemiesInstantiated()
    {
        for (int i = 0; i < enemiesGameobjectClone.Length; i++) //since inky follows Blinky's movement check if Blinky is in the scene if not then inky would follow the enemies movement in this order: Blinky > Pinky > Clyde > Inky
        {
            if (enemiesGameobjectClone[i].GetComponent<EnemyChase>().enemyType == EnemyChase.EnemyType.Blinky)
            {
                for (int j = 0; j < enemiesGameobjectClone.Length; j++)
                {
                    enemiesGameobjectClone[j].GetComponent<EnemyChase>().blinky = enemiesGameobjectClone[i];
                }
                break;
            }
            else if (enemiesGameobjectClone[i].GetComponent<EnemyChase>().enemyType == EnemyChase.EnemyType.Pinky)
            {
                for (int j = 0; j < enemiesGameobjectClone.Length; j++)
                {
                    enemiesGameobjectClone[j].GetComponent<EnemyChase>().blinky = enemiesGameobjectClone[i];
                }
                break;            
            }
            else if (enemiesGameobjectClone[i].GetComponent<EnemyChase>().enemyType == EnemyChase.EnemyType.Clyde)
            {
                for (int j = 0; j < enemiesGameobjectClone.Length; j++)
                {
                    enemiesGameobjectClone[j].GetComponent<EnemyChase>().blinky = enemiesGameobjectClone[i];
                }
                break;            
            }
            else if (enemiesGameobjectClone[i].GetComponent<EnemyChase>().enemyType == EnemyChase.EnemyType.Inky)
            {
                for (int j = 0; j < enemiesGameobjectClone.Length; j++)
                {
                    enemiesGameobjectClone[j].GetComponent<EnemyChase>().blinky = enemiesGameobjectClone[i];
                }
                break;     
            }
        }
    }

}
