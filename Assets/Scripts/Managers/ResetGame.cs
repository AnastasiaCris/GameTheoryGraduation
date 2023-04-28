using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour
{
    public GameObject mainMenu;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ResetTheGame()
    {
        StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        var asyncLoadLevel = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);

        yield return new WaitUntil((() => asyncLoadLevel.isDone));
        mainMenu = GameObject.FindWithTag("MainMenu");
        mainMenu.SetActive(false);
        GameManagerEditor.instance.ChangeSpecificMode("editor");
        Destroy(gameObject);
    }
}
