using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : MonoBehaviourOwner
{
    [SerializeField] [RequireInterface(typeof(IAnimator))] Object _animator;
    [SerializeField] [RequireInterface(typeof(IInput))] Object _input;
    [SerializeField] [RequireInterface(typeof(IMover))] Object _mover;

    public IAnimator animator => (IAnimator)_animator;
    public IInput input => (IInput)_input;
    public IMover mover => (IMover)_mover;

    public override void OtherStart()
    {
        Destroy(GetComponentInChildren<Rigidbody2D>());
    }

    public override void MyUpdate()
    {

        float horizontal = input.GetHorizontal();

        if(Mathf.Abs(horizontal) < 0.01f)
        {
            animator.Play("Idle");
        }
        else
        {
            animator.Play("Run");
            transform.localScale = new Vector3(horizontal > 0 ? 1 : -1, 1, 1);

        }


        mover.Move(new Vector2(horizontal, 0));
    }


}
