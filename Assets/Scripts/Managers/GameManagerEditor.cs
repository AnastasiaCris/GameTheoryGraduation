using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerEditor : MonoBehaviour
{
    
    [Header("Editor UI")] [Space]
    public GameObject editorUI;
    public GameObject elementsUI;
    public Image img_PlayBTN;
    public Sprite pause;
    public Sprite play;
    

    [Space][Header("Timer")] [Space]
    public bool timerOn;
    public TextMeshProUGUI timerText;
    public int timer { get; private set; }
    public int remainingTimer;

    public Slider gameSpeedTimer;
    public TextMeshProUGUI gameSpeedText;
    public float changedSpeedMultiplier = 1;
    
    [Space][Header("Map Prefabs")][Space]
    public GameObject normalMapPrefab;
    public GameObject bigMapPrefab;
    public GameObject smallMapPrefab;
    public GameObject openMapPrefab;
    public GameObject narrowMapPrefab;
    public GameObject mapDestination;
    
    [Space][Header("Agents")][Space]
    public GameObject enemyTypeUI;
    public List<GameObject> enemyTypeList = new List<GameObject>();
    public int maxEnemies = 4;
    public int[] enemyTypes = new int[] { 0, 1, 2, 3 };
    
    [Space][Header("Goals")] [Space]
    public bool killAllEnemies;
    
    [Space][Header("Rules")] [Space]
    public bool spawnMoreEnemiesOnKill;
    public int maxLives = 3;
    
    public static GameManagerEditor instance;
    void Awake()
    {
        if(instance == null)
            instance = this;
        
        //GameModeManager.gameMode = GameModeManager.GameMode.Editor;
        Instantiate(normalMapPrefab, mapDestination.transform);
        if (GameModeManager.gameMode == GameModeManager.GameMode.Classic)
        {
            img_PlayBTN.sprite = pause;
            elementsUI.SetActive(false);
        }
        Time.timeScale = 0;
    }

    #region ModeChanges

    private bool bigger;
    public void ChangeModes()
        {
            if (GameModeManager.gameMode == GameModeManager.GameMode.Editor)
            {
                img_PlayBTN.sprite = pause;
                elementsUI.SetActive(false);
                GameModeManager.gameMode = GameModeManager.GameMode.Classic;
                Time.timeScale = 1;
                if (timerOn)
                {
                    StartTimer();
                }
    
                if (bigger)
                {
                    SetUpCamera(false);
                    bigger = false;
                }
    
                FindObjectOfType<GameManagerOnRun>().StartNewRound();
            }
            else
            {
                if (!bigger)
                {
                    SetUpCamera(true);
                    bigger = true;
                }
    
                img_PlayBTN.sprite = play;
                editorUI.SetActive(true);
                elementsUI.SetActive(true);
                GameModeManager.gameMode = GameModeManager.GameMode.Editor;
                Time.timeScale = 0;
            }
        }
    
        public void ChangeSpecificMode(string mode = "")
        {
            if (mode == "run")
            {
                img_PlayBTN.sprite = pause;
                elementsUI.SetActive(false);
                GameModeManager.gameMode = GameModeManager.GameMode.Classic;
                Time.timeScale = 1;
                if (timerOn)
                {
                    StartTimer();
                }
    
                if (bigger)
                {
                    SetUpCamera(false);
                    bigger = false;
                }
    
                FindObjectOfType<GameManagerOnRun>().StartNewRound();
            }
            else if (mode == "editor")
            {
                if (!bigger)
                {
                    SetUpCamera(true);
                    bigger = true;
                }
    
                img_PlayBTN.sprite = play;
                editorUI.SetActive(true);
                elementsUI.SetActive(true);
                GameModeManager.gameMode = GameModeManager.GameMode.Editor;
                Time.timeScale = 0;
            }
        }
    

    #endregion

    #region Settings

    public void SetBoolean(int index)
        {
            switch (index)
            {
                case 0:
                    timerOn = !timerOn;
                    break;
                case 1:
                    spawnMoreEnemiesOnKill = !spawnMoreEnemiesOnKill;
                    break;
                case 2:
                    killAllEnemies = !killAllEnemies;
                    break;
            }
        }
    
        public void SetUpCamera(bool paused)
        {
            if (paused)
            {
                Camera.main.orthographicSize += 3;
            }
            else
            {
                Camera.main.orthographicSize -= 3;
            }
        }

    #endregion
    
    #region Time
    
    public void SetUpTimer(TMP_InputField textField)
    {
        timer = Int32.Parse(textField.text);
        if (timer <= 0)
        {
            timer = 0;
            textField.text = timer.ToString();
        }
        remainingTimer = timer;
        timerText.text = $"{remainingTimer / 60:00} : {remainingTimer % 60:00}";
    }

    public void StartTimer()
    {
        StopCoroutine(UpdateTimer());
        StartCoroutine(UpdateTimer());
    }

    public IEnumerator UpdateTimer()
    {
        while (remainingTimer >= 0)
        {
            timerText.text = $"{remainingTimer / 60:00} : {remainingTimer % 60:00}";
            remainingTimer--;
            yield return new WaitForSeconds(1);
        }

        yield return new WaitUntil(() => remainingTimer <= 0);
        FindObjectOfType<GameManagerOnRun>().GameOver();
        remainingTimer = timer; //reset timer
    }
    
    public void GameSpeed()
    {
        GameManagerOnRun onRunManager = FindObjectOfType<GameManagerOnRun>();

        gameSpeedText.text = "x"+ gameSpeedTimer.value.ToString("0.00");

        for (int i = 0; i < onRunManager.enemies.Count; i++)
        {
            onRunManager.enemies[i].movement.speedMultiplier = gameSpeedTimer.value;
            onRunManager.enemies[i].behaviour.duration *= gameSpeedTimer.value;
        }

        onRunManager.player.movement.speedMultiplier = gameSpeedTimer.value;
        changedSpeedMultiplier = gameSpeedTimer.value;
    }

    #endregion
    
    #region Space

    public void ChangeMap(string mapType)
        {
            //make sure you destroy any map already in there before instantiating a new one
            if (mapDestination.GetComponentInChildren<Transform>() != null)
            {
                foreach (Transform child in mapDestination.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            StartCoroutine(AddMap(mapType));
        }

    IEnumerator AddMap(string mapType)
    {
        yield return new WaitForEndOfFrame();
        switch (mapType)
        {
            case "normal":
                Instantiate(normalMapPrefab, mapDestination.transform);
                break;
            case "big":
                Instantiate(bigMapPrefab, mapDestination.transform);
                break;
            case "small":
                Instantiate(smallMapPrefab, mapDestination.transform);
                break;
            case "open":
                Instantiate(openMapPrefab, mapDestination.transform);
                break;
            case "narrow":
                Instantiate(narrowMapPrefab, mapDestination.transform);
                break;
        }
        SetUpCamera(true);
    }

    #endregion

    #region System Agents
        
        public void SetUpNrOfEnemies(TMP_InputField textField)
             {
                int nrOfEnemies = Int32.Parse(textField.text);
                if (nrOfEnemies < 0)
                {
                    nrOfEnemies = 0;
                    textField.text = nrOfEnemies.ToString();
                }
                ManagerMapSwitch onMapSwitch = FindObjectOfType<ManagerMapSwitch>();
                onMapSwitch.DeleteAllEnemies();
                onMapSwitch.SetUpForNewEnemies(nrOfEnemies);
                
                //Change the Height of the parent
                RectTransform rectTransformParent= textField.gameObject.transform.parent.parent.GetComponent<RectTransform>();
                float newHeight = rectTransformParent.sizeDelta.y * (nrOfEnemies + 1);
                rectTransformParent.sizeDelta = new Vector2(rectTransformParent.sizeDelta.x, newHeight);
    
                
                //Create the buttons
                if (enemyTypeList.Count > 0) //Remove first if there are any
                {
                    for (int i = 0; i < enemyTypeList.Count; i++)
                    {
                        Destroy(enemyTypeList[i]);
                    }
                    enemyTypeList.Clear();
                }
                for (int i = 0; i < nrOfEnemies; i++)
                {
                    GameObject enemyTypeUIBox = Instantiate(enemyTypeUI, textField.gameObject.transform.parent.parent);
                    enemyTypeList.Add(enemyTypeUIBox);
                    TMP_InputField enemyTypeText = enemyTypeList[i].GetComponentInChildren<TMP_InputField>();
                    enemyTypeText.onEndEdit.AddListener((string arg0) => SetUpTypeOfEnemy(enemyTypeText, enemyTypeList.IndexOf(enemyTypeUIBox)));
                }
    
                maxEnemies = nrOfEnemies;
                enemyTypes = new int[nrOfEnemies];
            }
    
            public void SetUpTypeOfEnemy(TMP_InputField textField, int nrInTheList)
            {
                int enemyType = Int32.Parse(textField.text);
                if (enemyType < 0)
                {
                    enemyType = 0;
                    textField.text = enemyType.ToString();
                }
                else if (enemyType > 3)
                {
                    enemyType = 3;
                    textField.text = enemyType.ToString();
                }
    
                ManagerMapSwitch onMapSwitch = FindObjectOfType<ManagerMapSwitch>();
                onMapSwitch.SetUpIndividualEnemies(enemyType, nrInTheList);
    
                enemyTypes[nrInTheList] = enemyType;
            }
        #endregion
    
    #region Rules

    public void SetUpLives(TMP_InputField textField)
    {
        GameManagerOnRun onRunManager = FindObjectOfType<GameManagerOnRun>();
        int life = Int32.Parse(textField.text);
        if (life <= 0)
        {
            life = 1;
            textField.text = life.ToString();
        }

        maxLives = life;
        onRunManager.SetLives(life, true);
    }

    #endregion
    
    
}
