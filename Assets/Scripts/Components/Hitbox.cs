using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Hitbox : MonoBehaviour, IHitbox
{
    [SerializeField] bool _isPlayer;
    [SerializeField] bool _isInvincible;

    [SerializeField] Vector2 pivot = new Vector2(0.5f, 0.5f);

    private BoxCollider2D _col;
    private bool _isHit = false;

    internal void Start()
    {
        _col = GetComponent<BoxCollider2D>();
    }

    public bool isPlayer { get => _isPlayer; set => _isPlayer = value; }
    public bool isInvincible { get => _isInvincible; set => _isInvincible = value; }

    public void Hit()
    {
        if (isInvincible) return;
        _isHit = true;
    }

    public bool IsHit()
    {
        if (_isHit)
        {
            _isHit = false;
            return true;
        }
        return false;
    }

    public void SetHSize(float size)
    {
        var s = _col.size;
        if (s.x == size) return;
        var diff = size - s.x;
        s.x = size;
        _col.size = s;
        var off = 0.5f - pivot.x;
        _col.offset += Vector2.right * off * diff;
    }

    public void SetVSize(float size)
    {
        var s = _col.size;
        if (s.y == size) return;
        var diff = size - s.y;
        s.y = size;
        _col.size = s;
        var off = 0.5f - pivot.y;
        _col.offset += Vector2.up * off * diff;
    }
}
