using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ConsoleDebug : MonoBehaviour
{
    public bool showDebug;
    public bool showHelp;
    
    private string input;

    public static DebugCommand KILL_ENEMIES;
    public static DebugCommand DELETE_ENEMIES;
    public static DebugCommand KILL_HERO;
    public static DebugCommand WIN_ROUND;
    public static DebugCommand LOSE_ROUND;
    public static DebugCommand INVINCIBLE_ON;
    public static DebugCommand INVINCIBLE_OFF;
    public static DebugCommand POWERUP_ON;
    public static DebugCommand<int> SET_SCORE;
    public static DebugCommand<int> SET_LIVES;
    
    public static DebugCommand HELP;
    public List<object> commandList;
    
    //UI

    public TMP_InputField field;
    public TextMeshProUGUI helpField;
    public Animator consoleAnim;
    public GameObject consoleUI;
    public GameObject indicatorsUI;

    public static ConsoleDebug instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        consoleUI.SetActive(false);
        indicatorsUI.SetActive(false);

        KILL_ENEMIES = new DebugCommand("kill_enemies", "Kills all enemies", "kill_enemies",
            () => GameManagerEditor.instance.onSetUpMap.DeleteEnemiesDebug(false,true));
        
        DELETE_ENEMIES = new DebugCommand("delete_enemies", "Deletes all enemies", "delete_enemies",
            () => GameManagerEditor.instance.onSetUpMap.DeleteEnemiesDebug());
        
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
        
        POWERUP_ON = new DebugCommand("powerup_on", "Enables the powerup effect until a powerup is collected", "powerup_on",
            () => GameManagerEditor.instance.onRunManager.PowerUpCollectedDebug(float.PositiveInfinity));
        

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
            DELETE_ENEMIES,
            KILL_HERO,
            WIN_ROUND,
            LOSE_ROUND,
            INVINCIBLE_ON,
            INVINCIBLE_OFF,
            POWERUP_ON,
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
        field.Select();
        field.ActivateInputField();
    }

    public IEnumerator DisplayConsole()
    {
        yield return new WaitUntil((() =>
            GameManagerEditor.instance.commandLine && GameModeManager.gameMode == GameModeManager.GameMode.Classic &&
            !UIManager.instance.timeStart.transform.parent.gameObject.activeSelf &&
            !UIManager.instance.goalDescription.activeSelf && !GameManagerEditor.instance.onRunManager.sceneEnabled));
        consoleUI.SetActive(true);
        indicatorsUI.SetActive(true);
    }

    public void ConsoleButton(bool showUp)
    {
        showDebug = showUp ? false : true;
        bool animBool = showUp ? true : false;
        consoleAnim.SetBool("Up", animBool);
        Time.timeScale = showDebug ? 0 : 1;
        
        if(showDebug)
            field.Select();
        
    }

    private void HandleInput()
    {
        if (InputSuccessful())
        {
            Transform parent = helpField.transform.parent;
            Vector2 delta = parent.GetComponent<RectTransform>().sizeDelta;
            parent.GetComponent<RectTransform>().sizeDelta = new Vector2(
                delta.x, delta.y + 40);
            helpField.text += $"Command {input} is successful\n\n";
        }
        else
        {
            if (input != "") //make sure input is not empty
            {
                Transform parent = helpField.transform.parent;
                Vector2 delta = parent.GetComponent<RectTransform>().sizeDelta;
                parent.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    delta.x, delta.y + 40);
                helpField.text += $"Command {input} is unknown\n\n";
            }

        }
        field.Select();
        field.ActivateInputField();
    }

    private bool InputSuccessful()
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
                    return true;

                }else if (commandList[i] as DebugCommand<int> != null)
                {
                    if (properties.Length < 2 || properties.Length > 2 || properties[1] != null && !int.TryParse(properties[1], out int n))
                    {
                        return false;
                    }
                    
                    (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                        
                    return true;
                }
            }
        }
        return false;
    }
    private void Display()
    {
        showHelp = true;

        Transform parent = helpField.transform.parent;
        Vector2 delta = parent.GetComponent<RectTransform>().sizeDelta;

        parent.GetComponent<RectTransform>().sizeDelta = new Vector2(
            delta.x, delta.y + 40 * commandList.Count);

        for (int i = 0; i < commandList.Count; i++)
        {
            DebugCommandBase command = commandList[i] as DebugCommandBase;

            helpField.text += $"{command.commandFormat} - {command.commandDescription}\n\n";
        }
    }
}
