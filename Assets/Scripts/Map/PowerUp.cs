
using System;
using UnityEngine;

public class PowerUp : Points
{
  //50 points
  public float duration = 8;
  public float scaleSize = 1.4f;

  private void Start()
  {
    transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
  }

  protected override void Collect()
  {
    FindObjectOfType<GameManagerOnRun>().PowerUpCollected(this);
  }
}
