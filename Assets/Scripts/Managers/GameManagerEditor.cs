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
    
    [Space][Header("Timer Element")] [Space]
    public bool timerOn;
    public TextMeshProUGUI timerText;
    public int timer { get; private set; }
    public int remainingTimer;
    public Slider gameSpeedTimer;
    public TextMeshProUGUI gameSpeedText;
    public float changedSpeedMultiplier = 1;
    
    [Space][Header("Space Element")][Space]
    public GameObject normalMapPrefab;
    public GameObject bigMapPrefab;
    public GameObject smallMapPrefab;
    public GameObject openMapPrefab;
    public GameObject narrowMapPrefab;
    public GameObject mapDestination;
    
    [Space][Header("System Agents Element")][Space]
    public int maxEnemies = 4;
    public int[] enemyTypes = new int[] { 0, 1, 2, 3 };
    public GameObject enemyTypeUIPrefab;
    public List<GameObject> enemyTypeUIPrefabList = new List<GameObject>();
    [SerializeField] RectTransform rectTransformParentUI;
    private Vector2 rectTransformParentUISize;
    
    [Space][Header("Objects")][Space]
    public bool normalPowerUp = true;
    public bool invincible;
    public bool speed;
    
    [Space][Header("Goals Element")] [Space]
    public bool killAllEnemies;
    
    [Space][Header("Rules Element")] [Space]
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

    private void Start()
    {
        normalPowerUp = true;
        rectTransformParentUISize = rectTransformParentUI.sizeDelta;
    }

    #region Settings
    
        private bool _bigger;
        public void ChangeSpecificMode(string mode = "")
        {
            if (mode == "run" || mode == "" && GameModeManager.gameMode == GameModeManager.GameMode.Editor)
            {
                img_PlayBTN.sprite = pause;
                elementsUI.SetActive(false);
                GameModeManager.gameMode = GameModeManager.GameMode.Classic;
                Time.timeScale = 1;
                if (timerOn)
                {
                    StartTimer();
                }

                if (_bigger)
                {
                    SetUpCamera(false);
                    _bigger = false;
                }

                FindObjectOfType<GameManagerOnRun>().StartNewRound();
            }
            else if (mode == "editor" || mode == "" && GameModeManager.gameMode == GameModeManager.GameMode.Classic)
            {
                if (!_bigger)
                {
                    SetUpCamera(true);
                    _bigger = true;
                }

                img_PlayBTN.sprite = play;
                editorUI.SetActive(true);
                elementsUI.SetActive(true);
                GameModeManager.gameMode = GameModeManager.GameMode.Editor;
                Time.timeScale = 0;
            }
        }

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
                case 3:
                    normalPowerUp = !normalPowerUp;
                    if (normalPowerUp)
                    {
                        invincible = false;
                        speed = false;
                    }
                    break;
                case 4:
                    invincible = !invincible;
                    if (invincible)
                    {
                        normalPowerUp = false;
                        speed = false;
                    }
                    break;
                case 5:
                    speed = !speed;
                    if (speed)
                    {
                        normalPowerUp = false;
                        invincible = false;
                    }
                    break;
            }
        }

        private void SetUpCamera(bool paused)
        {
            if (Camera.main == null)return;
                
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
            else if (nrOfEnemies > 10)
            {
                nrOfEnemies = 10;
                textField.text = nrOfEnemies.ToString();
            }

            //First delete any enemies and set up for new enemies
            ManagerMapSwitch onMapSwitch = FindObjectOfType<ManagerMapSwitch>();
            onMapSwitch.DeleteAllEnemies();
            onMapSwitch.SetUpForNewEnemies(nrOfEnemies);

            //Change the Height of the UI parent according to how many objects there are
            float newHeight = rectTransformParentUISize.y * (nrOfEnemies + 1);
            rectTransformParentUI.sizeDelta = new Vector2(rectTransformParentUI.sizeDelta.x, newHeight);

            //Create the buttons
            if (enemyTypeUIPrefabList.Count > 0) //Remove first if there are any
            {
                for (int i = 0; i < enemyTypeUIPrefabList.Count; i++)
                {
                    Destroy(enemyTypeUIPrefabList[i]);
                }

                enemyTypeUIPrefabList.Clear();
            }

            for (int i = 0; i < nrOfEnemies; i++)
            {
                GameObject enemyTypeUIBox = Instantiate(enemyTypeUIPrefab, textField.gameObject.transform.parent.parent);
                enemyTypeUIPrefabList.Add(enemyTypeUIBox);
                ChangeEnemyType changeEnemyType = enemyTypeUIBox.GetComponent<ChangeEnemyType>();

                changeEnemyType.arrowLeft.onClick.AddListener(() =>
                    onMapSwitch.SetUpIndividualEnemies(changeEnemyType.enemyType, enemyTypeUIPrefabList.IndexOf(enemyTypeUIBox)));
                changeEnemyType.arrowRight.onClick.AddListener(() =>
                    onMapSwitch.SetUpIndividualEnemies(changeEnemyType.enemyType, enemyTypeUIPrefabList.IndexOf(enemyTypeUIBox)));
            }

            maxEnemies = nrOfEnemies;
            enemyTypes = new int[nrOfEnemies];
        }

    #endregion

    #region Objects

    

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
