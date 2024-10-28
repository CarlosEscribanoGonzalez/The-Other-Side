using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public void Enter();

    public void Exit();

    public void Update(float deltaTime);

    public void FixedUpdate(float fixedTime);
}
