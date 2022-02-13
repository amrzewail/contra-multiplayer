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
    [SyncVar]
    private bool _isInvader;

    [SerializeField] Material invaderMaterial;

    public override void Start()
    {
        base.Start();

        GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(x => x.color = _color);

        if (_isInvader)
        {
            GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(x => x.material = invaderMaterial);
            GetComponentInChildren<ShooterNetworkSyncer>().isInvader = true;
            GetComponentsInChildren<Hitbox>().ToList().ForEach(x => x.supportedTypes = new List<DamageType>() { DamageType.Player, DamageType.Invader });
            GetComponent<Player>().isInvader = true;
        }
    }

    public void SetColor(Color col)
    {
        _color = col;
    }

    public void SetName(string name)
    {
        _name = name;
    }

    public void SetAsInvader()
    {
        _isInvader = true;
    }

}
