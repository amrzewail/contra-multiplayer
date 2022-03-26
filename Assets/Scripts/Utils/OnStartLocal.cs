using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnStartLocal : MonoBehaviour
{

    public UnityEvent StartEvent;

    public void Start()
    {
        StartEvent?.Invoke();
    }

}
