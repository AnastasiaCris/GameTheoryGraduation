
using UnityEngine;

public class PowerUp : Points
{
  //50 points
  public float duration = 8;
  
  protected override void Collect()
  {
    FindObjectOfType<GameManagerOnRun>().PowerUpCollected(this);
  }
}
