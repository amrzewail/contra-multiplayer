using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimatorController
{

    IAnimation GetAnimation(int index);
    IAnimation GetAnimation(string name);

    bool HasAnimations();
}
