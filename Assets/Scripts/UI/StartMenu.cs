using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void GoToMode(string gameMode)
    {
        switch (gameMode)
        {
            case "run":
               GameModeManager.gameMode = GameModeManager.GameMode.Run;
               break;
            case "edit":
               GameModeManager.gameMode = GameModeManager.GameMode.Editor;
               break;
        }
        
        SceneManager.LoadScene(1);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}




