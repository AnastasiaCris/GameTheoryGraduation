using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI score;
    public TextMeshProUGUI highScore;
    
    [Header("Life")]
    public GameObject lifeBar;
    public Slider healthSlider;
    public GameObject lifeHearts;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    static public UIManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SwitchOnOff(GameObject targetObj)
    {
        if (targetObj.activeSelf)
        {
            targetObj.SetActive(false);
        }
        else
        {
            targetObj.SetActive(true);
        }
    }
}
