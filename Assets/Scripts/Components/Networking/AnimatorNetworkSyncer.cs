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
        if (isServer) RpcChangeAnimation(identity.netId, animation.name);
        else CmdChangeAnimation(identity.netId, animation.name);
    }

    [ClientRpc]
    public void RpcChangeAnimation(uint netID, string animationName)
    {
        if (!isMine)
        {
            animator.Play(animationName, true);
        }
    }

    [Command(channel = Channels.Unreliable)]
    public void CmdChangeAnimation(uint netID, string animationName)
    {
        _currentAnimation = animationName;
        RpcChangeAnimation(netID, animationName);
    }
}
