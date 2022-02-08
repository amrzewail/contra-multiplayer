using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheckerServer : GroundCheckerBase
{

    public override void ServerOnTriggerEnter2D(Collider2D collision)
    {
        base.BaseOnTriggerEnter2D(collision);
    }

    public override void ServerOnTriggerExit2D(Collider2D collision)
    {
        base.BaseOnTriggerExit2D(collision);
    }

    public override void ServerFixedUpdate()
    {
        base.BaseFixedUpdate();
    }

}
