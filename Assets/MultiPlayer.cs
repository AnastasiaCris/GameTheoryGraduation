using UnityEngine;

public class MultiPlayer : Player
{
    public GameObject indicationKeys;

    public override void Update()
    {
        base.Update();
        if (indicationKeys.activeSelf && (ButtonPressed(Up) || ButtonPressed(Down) || ButtonPressed(Left) || ButtonPressed(Right)))
            indicationKeys.SetActive(false);
    }
    
}
