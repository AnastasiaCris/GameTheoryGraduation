using UnityEngine;

public class ManagerSetUpMap : MonoBehaviour
{
    [Header("Player")][Space]
    public GameObject playerPrefab;
    public GameObject[] multiplayerPlayerPrefabs;
    [SerializeField] bool gridMovement;
    public Transform playerStartingPos;
    public GameObject[] playersGameobjectClone{ get; private set; }
    public GameObject playerClone{ get; private set; }
    
    [Space][Header("Enemies")][Space]
    public GameObject[] enemiesPrefab;
    public GameObject[] enemiesGameobjectClone{ get; private set; }
    public Transform[] enemiesStartingPos;
    public Transform[] enemiesScatterPoints;
    public Transform[] enemiesHomePoints = new Transform[2];
    
    //int to keep track how many enemies of the same type are
    private int typeRed;
    private int typePink;
    private int typeBlue;
    private int typeOrange;
    
    [Space][SerializeField] private GameManagerOnRun gameManagerOnRun;

    public static ManagerSetUpMap instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        GameManagerEditor.instance.onSetUpMap = this;
        playersGameobjectClone = new GameObject[2];

        if (!GameManagerEditor.instance.multiplayer)
        {
            SetUpPlayer();
            SetUpDefaultEnemies();
        }
        else
        {
            SetUpForMultiplayer();
            SetUpDefaultEnemies();
        }
    }

    public void SetUpPlayer()
    {
        //Instantiating the player
        playerClone = Instantiate(playerPrefab, playerStartingPos.position, Quaternion.identity,
            transform.parent.transform.parent.transform);

        //Setting up the core game manager
        gameManagerOnRun.player = playerClone.GetComponent<Player>();

        if (gridMovement)
            gameManagerOnRun.player.movement.gridMovement = true;
    }

    public void SetUpDefaultEnemies()
    {
        //Set up the enemies
        SetUpForNewEnemies(GameManagerEditor.instance.maxEnemies);

        for (int i = 0; i < GameManagerEditor.instance.maxEnemies; i++)
        {
            SetUpIndividualEnemies(GameManagerEditor.instance.enemyTypes[i], i);
        }

        if (enemiesGameobjectClone.Length > 0)
            if (enemiesGameobjectClone[0].GetComponent<EnemyChase>().inkyDependant ==
                null) //if blinky is not set up
                AfterAllEnemiesInstantiated();
    }

    public void DeleteAllEnemies()
    {
        if(enemiesGameobjectClone != null)
            if (enemiesGameobjectClone.Length > 0)
            {
                for (int i = 0; i < enemiesGameobjectClone.Length; i++)
                {
                    enemiesGameobjectClone[i].GetComponent<Enemy>().freightened.Damaged();
                    Destroy(enemiesGameobjectClone[i]);
                }
            }
        
        
        gameManagerOnRun.enemies.Clear();
        enemiesGameobjectClone = new GameObject[0];

        if (GameManagerEditor.instance.multiplayer && playersGameobjectClone[1] != null)
        {
            gameManagerOnRun.enemies.Add(playersGameobjectClone[1].GetComponent<Enemy>());
        }
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
    
    /// <summary>
    /// Instantiates a specific enemy type and adds it to the list enemyGameObjectClone in a specific order
    /// </summary>
    /// <param name="enemyType"> What enemy type to instantiate </param>
    /// <param name="nrInTheList"> At what order in the list should it be added to </param>
    public void SetUpIndividualEnemies(int enemyType, int nrInTheList)
    {
        if (GameManagerEditor.instance.extraEnemies.Count > 0) //if there are any extra enemies, delete them(has to do with the rule kill an enemy and 2 enemies respawn)
        {
            for (int i = 0; i < GameManagerEditor.instance.extraEnemies.Count; i++)
            {
                Destroy(GameManagerEditor.instance.extraEnemies[i]);
            }
            GameManagerEditor.instance.extraEnemies.Clear();
        }
        
        if (enemiesGameobjectClone[nrInTheList] != null) //if this gameobject is null then continue, otherwise delete it first
        {
            //check first which type of enemy it is and set up the rest of the same enemy types accordingly
            switch (enemiesGameobjectClone[nrInTheList].GetComponent<EnemyChase>().enemyType)
            {
                case EnemyChase.EnemyType.Blinky:
                    typeRed--;
                    SetUpTheRestOfThisType(EnemyChase.EnemyType.Blinky, nrInTheList);

                    break;
                case EnemyChase.EnemyType.Pinky:
                    typePink--;
                    SetUpTheRestOfThisType(EnemyChase.EnemyType.Pinky, nrInTheList);

                    break;
                case EnemyChase.EnemyType.Inky:
                    typeBlue--;
                    SetUpTheRestOfThisType(EnemyChase.EnemyType.Inky, nrInTheList);

                    break;
                case EnemyChase.EnemyType.Clyde:
                    typeOrange--;
                    SetUpTheRestOfThisType(EnemyChase.EnemyType.Clyde, nrInTheList);
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
        
        //Make sure that if there is no pinky and blinky that the home duration of the other enemy types will be reduced
        if (!TypeOfEnemyIsPresent(EnemyChase.EnemyType.Blinky) && !TypeOfEnemyIsPresent(EnemyChase.EnemyType.Pinky))
        {
            if (TypeOfEnemyIsPresent(EnemyChase.EnemyType.Inky))
            {
                for (int i = 0; i < enemiesGameobjectClone.Length; i++) //all enemies of type inky will have home duration 2 and if there is clyde they will have 7
                {
                    if (enemiesGameobjectClone[i] == null)
                    {
                        break;
                    } 
                    
                    Enemy enemyScript = enemiesGameobjectClone[i].GetComponent<Enemy>();
                    if (enemyScript.chase.enemyType == EnemyChase.EnemyType.Inky)
                    {
                        enemyScript.home.originalDuration = 2;
                    }
                    else if (enemyScript.chase.enemyType == EnemyChase.EnemyType.Clyde)
                    {
                        enemyScript.home.originalDuration = 7;
                    }
                    
                    enemyScript.home.duration = enemyScript.home.originalDuration * GameManagerEditor.instance.changedSpeedMultiplier + enemyScript.typeAmount;
                }
            }
            else if (!TypeOfEnemyIsPresent(EnemyChase.EnemyType.Inky) && TypeOfEnemyIsPresent(EnemyChase.EnemyType.Clyde))
            {
                for (int i = 0; i < enemiesGameobjectClone.Length; i++) //all enemies of type clyde will have home duration 2
                {
                    if (enemiesGameobjectClone[i] == null)
                    {
                        break;
                    } 
                    
                    Enemy enemyScript = enemiesGameobjectClone[i].GetComponent<Enemy>();
                    if (enemyScript.chase.enemyType == EnemyChase.EnemyType.Clyde)
                    {
                        enemyScript.home.originalDuration = 2;

                    }
                    enemyScript.home.duration = enemyScript.home.originalDuration * GameManagerEditor.instance.changedSpeedMultiplier + enemyScript.typeAmount;
                }
            }
        }
        else //reset it
        {
            if (TypeOfEnemyIsPresent(EnemyChase.EnemyType.Inky) || TypeOfEnemyIsPresent(EnemyChase.EnemyType.Clyde))
            {
                for (int i = 0; i < enemiesGameobjectClone.Length; i++)
                {
                    if (enemiesGameobjectClone[i] == null)
                    {
                        break;
                    } 
                    
                    Enemy enemyScript = enemiesGameobjectClone[i].GetComponent<Enemy>();  
                    if (enemyScript.chase.enemyType == EnemyChase.EnemyType.Inky)
                    {
                        enemyScript.home.originalDuration = 7;
                        
                    }
                    else if (enemyScript.chase.enemyType == EnemyChase.EnemyType.Clyde)
                    {
                        enemyScript.home.originalDuration = 14;

                    }
                    enemyScript.home.duration = enemyScript.home.originalDuration * GameManagerEditor.instance.changedSpeedMultiplier + enemyScript.typeAmount;
                }
            }
        }

        //Setting up enemies of the same type
        float randomNrx;
        float randomNry;
        enemy.home.duration = enemy.home.originalDuration * GameManagerEditor.instance.changedSpeedMultiplier;

        switch (enemyType) //if the enemy you added right now has a similar type as another one in the list then increase it's home duration (so they don't all go at the same time) and give it a random position
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
                enemy.typeAmount = typeRed;
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
                enemy.typeAmount = typePink;
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
                enemy.typeAmount = typeBlue;
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
                enemy.typeAmount = typeOrange;
                typeOrange++;
                break;
        }

        //Setting up the core game manager
        gameManagerOnRun.enemies[nrInTheList] = enemy;
        GameManagerEditor.instance.enemyTypes[nrInTheList] = enemyType;

    }

    /// <summary>
    ///  Gets called after all enemies are instantiated
    ///  Makes sure that Inky's dependant gets changed based on other types of enemies
    /// </summary>
    public void AfterAllEnemiesInstantiated()
    {
        //since inky follows Blinky's movement check if Blinky is in the scene if not then inky would follow the enemies movement in this order: Blinky > Pinky > Clyde > Inky
        if (TypeOfEnemyIsPresent(EnemyChase.EnemyType.Blinky))
        {
            for (int j = 0; j < enemiesGameobjectClone.Length; j++)
            {
                enemiesGameobjectClone[j].GetComponent<EnemyChase>().inkyDependant = enemiesGameobjectClone[GetTheFirstEnemyOfType(EnemyChase.EnemyType.Blinky)];
            }
        }else if (TypeOfEnemyIsPresent(EnemyChase.EnemyType.Pinky))
        {
            for (int j = 0; j < enemiesGameobjectClone.Length; j++)
            {
                enemiesGameobjectClone[j].GetComponent<EnemyChase>().inkyDependant = enemiesGameobjectClone[GetTheFirstEnemyOfType(EnemyChase.EnemyType.Pinky)];
            }
        } else if (TypeOfEnemyIsPresent(EnemyChase.EnemyType.Clyde))
        {
            for (int j = 0; j < enemiesGameobjectClone.Length; j++)
            {
                enemiesGameobjectClone[j].GetComponent<EnemyChase>().inkyDependant = enemiesGameobjectClone[GetTheFirstEnemyOfType(EnemyChase.EnemyType.Clyde)];
            }
        }else if (TypeOfEnemyIsPresent(EnemyChase.EnemyType.Inky))
        {
            for (int j = 0; j < enemiesGameobjectClone.Length; j++)
            {
                enemiesGameobjectClone[j].GetComponent<EnemyChase>().inkyDependant = enemiesGameobjectClone[GetTheFirstEnemyOfType(EnemyChase.EnemyType.Inky)];
            }
        }
        
        if (GameManagerEditor.instance.multiplayer && playersGameobjectClone[1] != null)
        {
            gameManagerOnRun.enemies[gameManagerOnRun.enemies.Count-1] = playersGameobjectClone[1].GetComponent<Enemy>();
        }
    }

    /// <summary>
    /// Checks if the type of enemy given is present in the list enemyGameObjectClone
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool TypeOfEnemyIsPresent(EnemyChase.EnemyType type)
    {
        bool present = false;
        for (int i = 0; i < enemiesGameobjectClone.Length; i++)
        {
            if(enemiesGameobjectClone[i] == null)
            {
                return present;
            }
            if (enemiesGameobjectClone[i].GetComponent<EnemyChase>().enemyType == type)
            {
                present = true;
                return present;
            }
        }
        return present;
    }

    /// <summary>
    /// Gets the first enemy in the list enemyGameObjectClone of a certain type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    int GetTheFirstEnemyOfType(EnemyChase.EnemyType type)
    {
        int id = 99;
        for (int i = 0; i < enemiesGameobjectClone.Length; i++)
        {
            if(enemiesGameobjectClone[i] == null)
            {
                return id;
            }
            if (enemiesGameobjectClone[i].GetComponent<EnemyChase>().enemyType == type)
            {
                id = i;
                return id;
            }
        }

        return id;
    }
    
    /// <summary>
    /// if there are enemies in the list of the same type that have a higher enemyTypeAmount then the one in the list , then adjust the home duration
    /// </summary>
    /// <param name="type"></param>
    void SetUpTheRestOfThisType(EnemyChase.EnemyType type, int nrInTheList)
    {
        if (TypeOfEnemyIsPresent(type))
        {
            for (int i = 0; i < enemiesGameobjectClone.Length; i++)
            {
                if (enemiesGameobjectClone[i] == null)
                {
                    break;
                } 
                            
                Enemy enemyScript = enemiesGameobjectClone[i].GetComponent<Enemy>();
                if (enemyScript.chase.enemyType == type && enemyScript.typeAmount > enemiesGameobjectClone[nrInTheList].GetComponent<Enemy>().typeAmount)
                {
                    enemyScript.typeAmount--;
                    enemyScript.home.duration = enemyScript.home.originalDuration * GameManagerEditor.instance.changedSpeedMultiplier + enemyScript.typeAmount;
                }
            }
        }
    }

    public void SetUpForMultiplayer()
    {
        //Instantiating the players
        playerClone = Instantiate(multiplayerPlayerPrefabs[0], playerStartingPos.position, Quaternion.identity,
            transform.parent.transform.parent.transform); //player

        Vector3 newpos = new Vector3(enemiesStartingPos[0].position.x - 1, enemiesStartingPos[0].position.y,
            enemiesStartingPos[0].position.z);

        GameObject playerClone2 = Instantiate(multiplayerPlayerPrefabs[1], newpos, Quaternion.identity,
            transform.parent.transform.parent.transform);//enemy
        
        //Set them up
        Enemy enemy = playerClone2.GetComponent<Enemy>();

        enemy.target = playerClone.transform;
        enemy.home.home = enemiesHomePoints[0];
        enemy.home.exit = enemiesHomePoints[1];
        enemy.scatter.scatterNode = enemiesScatterPoints[0];
        
        //Setting up the core game manager
        gameManagerOnRun.player = playerClone.GetComponent<Player>();
        gameManagerOnRun.player2 = playerClone2.GetComponent<Player>();

        if (gridMovement)
            gameManagerOnRun.player.movement.gridMovement = true;

        playersGameobjectClone = new GameObject[] { playerClone, playerClone2 };
        
        gameManagerOnRun.enemies.Add(enemy);
        //GameManagerEditor.instance.enemyTypes = new int[] { 0,1,2,3 };
    }
}
