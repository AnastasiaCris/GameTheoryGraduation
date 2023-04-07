using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameManager gamaManager;

    public TextMeshProUGUI score;
    public TextMeshProUGUI life;

    static public UIManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
