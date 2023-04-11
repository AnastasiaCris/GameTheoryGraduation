using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerEditor : MonoBehaviour
{
    
    [Header("Editor UI")] 
    public GameObject editorUI;
    public GameObject elementsUI;

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

    [Header("Rules")] public bool spawnMoreEnemiesOnKill = true;
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

        spawnMoreEnemiesOnKill = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeModes()
    {
        if (GameModeManager.gameMode == GameModeManager.GameMode.Editor)
        {
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
            editorUI.SetActive(true);
            elementsUI.SetActive(true);
            GameModeManager.gameMode = GameModeManager.GameMode.Editor;
            Time.timeScale = 0;
        }
    }

    #region Time

    public void SetTimer()
    {
        if (!timerOn)
        {
            timerOn = true;
        }
        else
        {
            timerOn = false;
        }
        
    }

    public void SetUpTimer(TMP_InputField textField)
    {
        timer = Int32.Parse(textField.text);
        remainingTimer = timer;
        timerText.text = $"{remainingTimer / 60:00} : {remainingTimer % 60:00}";
    }

    public void StartTimer()
    {
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
            Debug.Log("before: " + Time.timeScale);
            //make sure you destroy any map already in there before instantiating a new one
            if (mapDestination.GetComponentInChildren<Transform>() != null)
            {
                foreach (Transform child in mapDestination.transform)
                {
                    Destroy(child.gameObject);
                }
            }
    
    
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
            Debug.Log("after: " + Time.timeScale);
        }

    #endregion
    

    
}
