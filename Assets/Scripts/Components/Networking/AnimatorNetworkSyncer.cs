using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorNetworkSyncer : NetworkBehaviourOwner
{
    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _animator;

    public IAnimator animator => (IAnimator)_animator;

    public bool serverAuthority = false;

    [SyncVar]
    private string _currentAnimation;

    private string Guid;

    public override void Start()
    {
        base.Start();

        Guid = System.Guid.NewGuid().ToString();
    }

    public override void MyStart()
    {
        if (serverAuthority) return;
        animator.AnimationChanged += AnimationChangedCallback;
    }

    public override void OtherStart()
    {
        if (string.IsNullOrEmpty(_currentAnimation)) return;
        animator.Play(_currentAnimation, true);
    }

    public override void ServerStart()
    {
        if (!serverAuthority) return;
        animator.AnimationChanged += AnimationChangedCallback;
    }

    private void AnimationChangedCallback(IAnimation animation)
    {
        if (isServer)
        {
            _currentAnimation = animation.name;
            RpcChangeAnimation(identity.netId, Guid, animation.name);
        }
        else
        {
            CmdChangeAnimation(identity.netId, Guid, animation.name);
        }
    }

    [ClientRpc]
    public void RpcChangeAnimation(uint netID, string guid, string animationName)
    {
        if (netID == identity.netId && !isMine && !guid.Equals(Guid))
        {
            animator.Play(animationName, true);
        }
    }

    [Command(channel = Channels.Unreliable)]
    public void CmdChangeAnimation(uint netID, string guid, string animationName)
    {
        _currentAnimation = animationName;
        RpcChangeAnimation(netID, guid, animationName);
    }
}
