using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManagerEditor : MonoBehaviour
{
    
    [Header("Editor UI")] 
    public GameObject editorUI;
    public GameObject elementsUI;
    public Image img_PlayBTN;
    public Sprite pause;
    public Sprite play;
    

    [Header("Timer")] public bool timerOn;
    public TextMeshProUGUI timerText;
    public int timer { get; private set; }
    public int remainingTimer;
    
    [Header("Map Prefabs")]
    public GameObject normalMapPrefab;
    public GameObject bigMapPrefab;
    public GameObject smallMapPrefab;
    public GameObject openMapPrefab;
    public GameObject narrowMapPrefab;
    
    public GameObject mapDestination;

    [Header("Goals")] 
    public bool killAllEnemies;
    
    [Header("Rules")] 
    public bool spawnMoreEnemiesOnKill;
    
    public static GameManagerEditor instance;
    void Awake()
    {
        if(instance == null)
            instance = this;
        
        
        //GameModeManager.gameMode = GameModeManager.GameMode.Editor;
        Instantiate(normalMapPrefab, mapDestination.transform);
        if (GameModeManager.gameMode == GameModeManager.GameMode.Classic)
        {
            editorUI.SetActive(false);
        }else
        {
            Time.timeScale = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
            Vector3 pos = Camera.main.transform.position;
            //Camera.main.transform.position = new Vector3(pos.x - 5, pos.y, pos.z);
        }
        else
        {
            
        }
    }

    #region Time
    
    public void SetUpTimer(TMP_InputField textField)
    {
        timer = Int32.Parse(textField.text);
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
        GameManagerOnRun.instance.GameOver();
        remainingTimer = timer; //reset timer
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

    #region Rules

    

    #endregion

    
}
