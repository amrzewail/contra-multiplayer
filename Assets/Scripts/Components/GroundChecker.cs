using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : GroundCheckerBase
{
    public override void MyOnTriggerEnter2D(Collider2D collision)
    {
        base.BaseOnTriggerEnter2D(collision);
    }

    public override void MyOnTriggerExit2D(Collider2D collision)
    {
        base.BaseOnTriggerExit2D(collision);
    }

    public override void MyFixedUpdate()
    {
        base.BaseFixedUpdate();
    }
}
