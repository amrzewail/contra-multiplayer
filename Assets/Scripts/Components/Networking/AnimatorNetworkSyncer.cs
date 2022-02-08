using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorNetworkSyncer : NetworkBehaviourOwner
{
    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _animator;

    public IAnimator animator => (IAnimator)_animator;

    public override void MyStart()
    {
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
        if (!isMine && identity.netId == netID)
        {
            animator.Play(animationName, true);
        }
    }

    [Command(channel = Channels.Unreliable)]
    public void CmdChangeAnimation(uint netID, string animationName)
    {
        RpcChangeAnimation(netID, animationName);
    }
}
