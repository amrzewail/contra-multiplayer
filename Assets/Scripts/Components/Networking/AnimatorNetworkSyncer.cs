using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorNetworkSyncer : MonoBehaviourOwner
{
    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _animator;

    public IAnimator animator => (IAnimator)_animator;

    public override void MyStart()
    {
        animator.AnimationChanged += AnimationChangedCallback;
    }

    private void AnimationChangedCallback(IAnimation animation)
    {
        _photonView.RPC("ChangeAnimation", RpcTarget.All, _photonView.ViewID, animation.name);
    }

    [PunRPC]
    public void ChangeAnimation(int viewID, string animationName)
    {
        if (!_photonView.IsMine && _photonView.ViewID == viewID)
        {
            animator.Play(animationName, true);
        }
    }
}
