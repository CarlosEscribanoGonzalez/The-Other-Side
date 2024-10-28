using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IWolf
{
    public void SetState(AWolfState state);

    public AWolfState GetState();

    public Animator GetAnimator();

    public NavMeshAgent GetAgent();

    public Transform GetTransform();

    public bool PlayerOnSight();

    public bool SoundHeard();

    public Quaternion GetSoundDirection();

    public void PlayMouthAudio(string state);

    public bool IsTactic();

    public bool IsIntelligent();
}
