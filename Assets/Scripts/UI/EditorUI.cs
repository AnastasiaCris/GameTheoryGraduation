using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorUI : MonoBehaviour
{
    
    void Start()
    {
        
    }

   
    void Update()
    {
        
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
