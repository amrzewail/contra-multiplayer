using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxNetworkSyncer : NetworkBehaviourOwner
{
    [SerializeField] bool serverAuthority = false;
    [SerializeField] [RequireInterface(typeof(IHitbox))] Object _hitbox;

    public IHitbox hitbox => (IHitbox)_hitbox;

    [SyncVar]
    private bool _isInvincible;

    private bool _lastInvincibility;

    public override void MyStart()
    {
        if (serverAuthority) return;

        hitbox.OnHSizeChanged += HSizeChangedCallback;
        hitbox.OnVSizeChanged += VSizeChangedCallback;
        hitbox.OnVOffsetChanged += VOffsetChangedCallback;

        CmdChangeInvinciblity(hitbox.isInvincible);
        _lastInvincibility = hitbox.isInvincible;
    }

    public override void MyUpdate()
    {
        if (serverAuthority) return;
        if(hitbox.isInvincible != _lastInvincibility)
        {
            CmdChangeInvinciblity(hitbox.isInvincible);
            _lastInvincibility = hitbox.isInvincible;
        }
    }

    public override void OtherUpdate()
    {
        if (serverAuthority) return;
        hitbox.isInvincible = _isInvincible;
    }

    public override void ServerStart()
    {
        if (!serverAuthority) return;

        hitbox.OnHSizeChanged += HSizeChangedCallback;
        hitbox.OnVSizeChanged += VSizeChangedCallback;
        hitbox.OnVOffsetChanged += VOffsetChangedCallback;
    }

    public override void ServerUpdate()
    {
        if (!serverAuthority) return;

        _isInvincible = hitbox.isInvincible;
    }

    public override void ClientUpdate()
    {
        if (!serverAuthority) return;
        hitbox.isInvincible = _isInvincible;
    }

    private void HSizeChangedCallback(float size)
    {
        CmdChangeHSize(identity.netId, size);
    }

    private void VSizeChangedCallback(float size)
    {
        CmdChangeVSize(identity.netId, size);
    }

    private void VOffsetChangedCallback(float offset)
    {
        CmdChangeVOffset(identity.netId, offset);
    }

    [Command]
    private void CmdChangeInvinciblity(bool invincible)
    {
        _isInvincible = invincible;
    }

    [Command]
    private void CmdChangeVSize(uint netID, float size)
    {
        RpcChangeVSize(netID, size);
    }

    [ClientRpc]
    private void RpcChangeVSize(uint netID, float size)
    {
        if(netID == identity.netId && !isMine)
        {
            hitbox.SetVSize(size);
        }
    }

    [Command]
    private void CmdChangeHSize(uint netID, float size)
    {
        RpcChangeHSize(netID, size);
    }

    [ClientRpc]
    private void RpcChangeHSize(uint netID, float size)
    {
        if (netID == identity.netId && !isMine)
        {
            hitbox.SetHSize(size);
        }
    }

    [Command]
    private void CmdChangeVOffset(uint netID, float size)
    {
        RpcChangeVOffset(netID, size);
    }

    [ClientRpc]
    private void RpcChangeVOffset(uint netID, float size)
    {
        if (netID == identity.netId && !isMine)
        {
            hitbox.SetVOffset(size);
        }
    }

}
