using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance;
    public CompositeState freezeInputState;

    public bool AreInputFrozen => freezeInputState.IsOn;

    private void Awake()
    {
        freezeInputState = new CompositeState();

        Instance = this;
    }
}