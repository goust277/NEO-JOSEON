using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public abstract class EnemyState : MonoBehaviour
{
    protected Enemy actor;

    protected virtual void Awake()
    {
        actor = GetComponent<Enemy>();
    }

    public void LoadActor()
    {
        if (actor == null)
            actor = GetComponent<Enemy>();
        actor = GetComponent<Enemy>();
    }

    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
}
