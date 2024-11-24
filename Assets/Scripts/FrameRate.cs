using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRate : MonoBehaviour
{
    public int targetFPS = 120;

    void Awake()
    {
        Application.targetFrameRate = targetFPS;
    }
}
