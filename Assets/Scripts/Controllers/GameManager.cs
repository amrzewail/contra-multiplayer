using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    internal void Awake()
    {
        Application.targetFrameRate = 120;
    }
}
