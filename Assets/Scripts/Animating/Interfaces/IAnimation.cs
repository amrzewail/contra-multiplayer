using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimation 
{
    Sprite GetFrame(int index);

    void UpdateEvents(int currentFrame, ref List<string> invokeList);

    int length { get; }

    string name { get; }

    bool loop { get; }

    float interval { get; }
}
