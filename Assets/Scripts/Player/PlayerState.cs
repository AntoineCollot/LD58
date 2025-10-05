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
    public CompositeState freezeMoveState;
    public CompositeState freezeClickState;

    public bool CanMove => !freezeMoveState.IsOn;
    public bool CanClick => !freezeClickState.IsOn;

    private void Awake()
    {
        freezeMoveState = new CompositeState();
        freezeClickState = new CompositeState();

        Instance = this;
    }
}