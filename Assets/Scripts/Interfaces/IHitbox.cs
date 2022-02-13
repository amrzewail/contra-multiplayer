using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitbox
{
    public List<DamageType> supportedTypes { get; set; }
    public GameObject gameObject { get; }
    public bool isInvincible { get; set; }

    public void SetVSize(float size);
    public void SetHSize(float size);

    public void SetVOffset(float offset);
    public bool Hit(DamageType type);

    public bool IsHit(out int hitsCount);

    Action<float> OnVSizeChanged { get; set; }
    Action<float> OnHSizeChanged { get; set; }
    Action<float> OnVOffsetChanged { get; set; }

}
public enum DamageType
{
    Player,
    Invader,
    Enemy,
    General
}