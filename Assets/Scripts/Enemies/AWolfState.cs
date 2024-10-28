using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AWolfState : IState
{
    protected IWolf wolf;

    public AWolfState(IWolf wolf)
    {
        this.wolf = wolf;
    }

    public abstract void Enter();

    public abstract void Exit();

    public abstract void Update(float deltaTime);

    public abstract void FixedUpdate(float fixedTime);
}
