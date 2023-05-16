using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConsoleDebug : MonoBehaviour
{
    public bool showDebug;
    public bool showHelp;
    
    private string input;

    public static DebugCommand KILL_ENEMIES;
    public static DebugCommand KILL_HERO;
    public static DebugCommand WIN_ROUND;
    public static DebugCommand LOSE_ROUND;
    public static DebugCommand INVINCIBLE_ON;
    public static DebugCommand INVINCIBLE_OFF;
    public static DebugCommand POWERUP_ON;
    public static DebugCommand PAUSE_ON;
    public static DebugCommand PAUSE_OFF;
    public static DebugCommand<int> SET_SCORE;
    public static DebugCommand<int> SET_LIVES;
    
    public static DebugCommand HELP;
    public List<object> commandList;
    
    //UI

    public TMP_InputField field;
    public GameObject helpFieldMain;
    public TextMeshProUGUI helpField;
    

    private void Awake()
    {
        field.gameObject.SetActive(showDebug);
        helpFieldMain.SetActive(showHelp);

        
        KILL_ENEMIES = new DebugCommand("kill_enemies", "Kills all enemies", "kill_enemies",
            () => GameManagerEditor.instance.onSetUpMap.DeleteAllEnemies());
        
        KILL_HERO = new DebugCommand("kill_hero", "Kills the hero", "kill_hero",
            () => GameManagerEditor.instance.onRunManager.PlayerDamaged());
        
        WIN_ROUND = new DebugCommand("win_round", "Win the round", "win_round",
            () => GameManagerEditor.instance.onRunManager.ShowScene(true));
        
        LOSE_ROUND = new DebugCommand("lose_round", "Lose the round", "lose_round",
            () => GameManagerEditor.instance.onRunManager.ShowScene(false));
        
        INVINCIBLE_ON = new DebugCommand("hero_invincible_on", "The hero is immune to enemies", "hero_invincible_on",
            () => GameManagerEditor.instance.onRunManager.player.InvincibilityDebug(true));
        
        INVINCIBLE_OFF = new DebugCommand("hero_invincible_off", "The hero is not immune to enemies", "hero_invincible_off",
            () => GameManagerEditor.instance.onRunManager.player.InvincibilityDebug(false));
        
        POWERUP_ON = new DebugCommand("powerup_on", "Enables the powerup effect", "powerup_on",
            () => GameManagerEditor.instance.onRunManager.PowerUpCollectedDebug(float.PositiveInfinity));

        PAUSE_ON = new DebugCommand("time_stop", "Stops the time", "time_stop",
            () => Time.timeScale = 0);
        
        PAUSE_OFF = new DebugCommand("time_start", "Starts the time", "time_start",
            () => Time.timeScale = 1);

        SET_SCORE = new DebugCommand<int>("set_score", "Sets the amount of score", "set_score <score amount>", (x) =>
        {
            GameManagerEditor.instance.onRunManager.SetScore(x);
        });

        SET_LIVES = new DebugCommand<int>("set_lives", "Sets the amount of lives", "set_lives <lives amount>", (x) =>
        {
            GameManagerEditor.instance.onRunManager.SetLives(x, true);
        });

        HELP = new DebugCommand("help", "Shows a list of commands", "help", () => Display());
        
        commandList = new List<object>
        {
            KILL_ENEMIES,
            KILL_HERO,
            WIN_ROUND,
            LOSE_ROUND,
            INVINCIBLE_ON,
            INVINCIBLE_OFF,
            POWERUP_ON,
            PAUSE_ON,
            PAUSE_OFF,
            SET_SCORE,
            SET_LIVES,
            HELP
        };
    }

    public void SetUpInput(TMP_InputField textField)
    {
        input = textField.text;
        HandleInput();
        input = "";
        field.text = input;
    }

    void Update()
    {
        if (!GameManagerEditor.instance.commandLine || GameModeManager.gameMode == GameModeManager.GameMode.Editor)
            return;
        if (Input.GetKeyDown(KeyCode.F2) || Input.GetKeyDown(KeyCode.BackQuote))
        {
            showDebug = !showDebug;

            field.gameObject.SetActive(showDebug);
            if(!showDebug)
                helpFieldMain.gameObject.SetActive(showDebug);
        }
    }

    private void HandleInput()
    {
        string[] properties = input.Split(' ');
        
        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if (input.Contains(commandBase.commandID))
            {
                if (commandList[i] as DebugCommand != null)
                {
                    (commandList[i] as DebugCommand).Invoke();
                }else if (commandList[i] as DebugCommand<int> != null)
                {
                    (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                }
            }
        }
    }

    private void Display()
    {
        showHelp = true;
        helpFieldMain.SetActive(showHelp);
        helpField.text = "";

        Transform parent = helpField.transform.parent;
        Vector2 delta = parent.GetComponent<RectTransform>().sizeDelta;

        parent.GetComponent<RectTransform>().sizeDelta = new Vector2(
            delta.x, delta.y + 50 * commandList.Count);

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase command = commandList[i] as DebugCommandBase;

            helpField.text += $"{command.commandFormat} - {command.commandDescription}\n\n";
        }
    }

    public void SetWriting(bool write)
    {
        GameManagerEditor.instance.writing = write;
    }
}
