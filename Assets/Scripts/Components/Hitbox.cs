using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Hitbox : MonoBehaviourOwner, IHitbox
{
    [SerializeField] List<DamageType> _supportedTypes;
    [SerializeField] bool _isInvincible;

    [SerializeField] Vector2 pivot = new Vector2(0.5f, 0.5f);

    private BoxCollider2D _col;
    private bool _isHit = false;
    private Vector2 _startingOffset;
    private int _hitsCount = 0;

    public Action<float> OnVSizeChanged { get; set; }
    public Action<float> OnHSizeChanged { get; set; }
    public Action<float> OnVOffsetChanged { get; set; }
    public List<DamageType> supportedTypes { get => _supportedTypes; set => _supportedTypes = value; }

    public override void MyStart()
    {
        _col = GetComponent<BoxCollider2D>();
        _startingOffset = _col.offset;
    }
    public override void OtherStart()
    {
        MyStart();
    }

    public override void MyLateUpdate()
    {
        _hitsCount = 0;
    }

    public bool isInvincible { get => _isInvincible; set => _isInvincible = value; }

    public bool Hit(DamageType type)
    {
        if (!supportedTypes.Contains(type) && type != DamageType.General) return false;
        if (isInvincible) return false;
        _isHit = true;
        _hitsCount++;
        return true;
    }

    public bool IsHit(out int hitsCount)
    {
        hitsCount = 0;
        if (_isHit)
        {
            hitsCount = _hitsCount;
            _hitsCount = 0;
            _isHit = false;
            return true;
        }
        return false;
    }

    public void SetHSize(float size)
    {
        if (!_col) return;

        var s = _col.size;
        if (Mathf.Abs(s.x - size) < 0.01f) return;
        var diff = size - s.x;
        s.x = size;
        _col.size = s;

        OnHSizeChanged?.Invoke(size);
    }

    public void SetVSize(float size)
    {
        if (!_col) return;

        var s = _col.size;
        if (Mathf.Abs(s.y - size) < 0.01f) return;
        var diff = size - s.y;
        s.y = size;
        _col.size = s;

        OnVSizeChanged?.Invoke(size);
    }

    public void SetVOffset(float offset)
    {
        if (!_col) return;

        var off = _startingOffset + new Vector2(0, offset);
        if (Mathf.Abs(off.y - _col.offset.y) < 0.01f) return;
        _col.offset = off;
        OnVOffsetChanged?.Invoke(offset);

    }
}
