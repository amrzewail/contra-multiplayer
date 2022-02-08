using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitbox
{
    public bool isPlayer { get; set; }
    public bool isInvincible { get; set; }

    public void SetVSize(float size);
    public void SetHSize(float size);

    public void SetVOffset(float offset);
    public void Hit();

    public bool IsHit();

}
