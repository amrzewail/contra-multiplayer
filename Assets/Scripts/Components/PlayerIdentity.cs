using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerIdentity : NetworkBehaviourOwner
{
    [SyncVar]
    private Color _color;
    [SyncVar]
    private string _name;

    public override void Start()
    {
        base.Start();

        GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(x => x.color = _color);
    }

    public void SetColor(Color col)
    {
        _color = col;
    }

    public void SetName(string name)
    {
        _name = name;
    }

}
