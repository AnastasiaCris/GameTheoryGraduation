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
        
    }
}
