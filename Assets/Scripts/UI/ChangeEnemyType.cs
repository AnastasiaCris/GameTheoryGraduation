using UnityEngine;
using UnityEngine.UI;

public class ChangeEnemyType : MonoBehaviour
{
    public Button arrowLeft;
    public Button arrowRight;
    public Image enemySprite;

    public Color[] enemyColors;
    public int enemyType = 0;//between 0-3
    void Start()
    {
        enemyType = 4;
        ChangeType(true);
    }

    public void ChangeType(bool right)
    {
        if (right)
        {
            if(enemyType >= 0 && enemyType < 3)
                enemyType++;
            else if (enemyType >= 3)
                enemyType = 0;
        }
        else
        {
            if(enemyType > 0 && enemyType <= 4)
                enemyType--;
            else if (enemyType <= 0)
                enemyType = 3;
            
        }

        enemySprite.color = enemyColors[enemyType];
        
        Hover();
        
    }

    public void Hover()
    {
        GameManagerEditor.instance.videos[0].SetActive(false);
        switch (enemyType)
        {
            case 0:
                
                GameManagerEditor.instance.videos[1].SetActive(true);
                GameManagerEditor.instance.videos[2].SetActive(false);
                GameManagerEditor.instance.videos[3].SetActive(false);
                GameManagerEditor.instance.videos[4].SetActive(false);
                break;
            case 1:
                GameManagerEditor.instance.videos[1].SetActive(false);
                GameManagerEditor.instance.videos[2].SetActive(true);
                GameManagerEditor.instance.videos[3].SetActive(false);
                GameManagerEditor.instance.videos[4].SetActive(false);
                break;
            case 2:
                GameManagerEditor.instance.videos[1].SetActive(false);
                GameManagerEditor.instance.videos[2].SetActive(false);
                GameManagerEditor.instance.videos[3].SetActive(true);
                GameManagerEditor.instance.videos[4].SetActive(false);
                break;
            case 3:
                GameManagerEditor.instance.videos[1].SetActive(false);
                GameManagerEditor.instance.videos[2].SetActive(false);
                GameManagerEditor.instance.videos[3].SetActive(false);
                GameManagerEditor.instance.videos[4].SetActive(true);
                break;
        }
    }

    public void All()
    {
        GameManagerEditor.instance.videos[0].SetActive(true);
        GameManagerEditor.instance.videos[1].SetActive(false);
        GameManagerEditor.instance.videos[2].SetActive(false);
        GameManagerEditor.instance.videos[3].SetActive(false);
        GameManagerEditor.instance.videos[4].SetActive(false);
    }
}
