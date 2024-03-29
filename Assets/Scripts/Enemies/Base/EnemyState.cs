using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public abstract class EnemyState : MonoBehaviour
{
    protected Enemy actor;

    private void Awake()
    {
        actor = GetComponent<Enemy>();
    }

    public void LoadActor()
    {
        if (actor == null)
            actor = GetComponent<Enemy>();
    }

    private bool HasAnimParam(string name)
    {
        if (actor.AnimControl == null)
            return false;
        AnimatorControllerParameter[] a = actor.AnimControl.parameters;
        foreach (AnimatorControllerParameter param in a)
        {
            if (param.name == name)
                return true;
        }
        return false;
    }

    protected bool TrySetAnimBool(string name, bool b)
    {
        if (HasAnimParam(name))
        {
            actor.AnimControl.SetBool(name, b);
        }
        return false;
    }

    protected bool TrySetAnimFloat(string name, float f)
    {
        if (HasAnimParam(name))
        {
            actor.AnimControl.SetFloat(name, f);
        }
        return false;
    }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}
