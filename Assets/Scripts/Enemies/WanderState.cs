using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class WanderState : AWolfState
{
    private float wanderRadius = 5; //Radio en el que elige una posici�n a la que desplazarse alrededor de �l
    private float speed = 5; //Velocidad de movimiento al caminar

    public WanderState(IWolf wolf) : base(wolf) { }

    public override void Enter()
    {
        wolf.GetAnimator().SetTrigger("Walk");
        Vector3 newPos = GetTargetPos(wolf.GetTransform().position, wanderRadius, 1); //Importante que s�lo el suelo est� en layermask 1
        if (wolf.IsIntelligent()) wanderRadius = 20; //El lobo del laberinto se mueve a un �rea mucho mayor
        wolf.GetAgent().speed = speed;
        wolf.GetAgent().SetDestination(newPos);
    }

    public override void Exit()
    {
        wolf.GetAgent().ResetPath();
    }

    public override void Update(float deltaTime)
    {
        //Puede gru�ir de vez en cuando
        wolf.PlayMouthAudio("WanderState");
        //Si llega a su destino o escucha un sonido se queda quieto. Si ve al jugador lo persigue
        if (wolf.GetAgent().remainingDistance <= wolf.GetAgent().stoppingDistance && !wolf.GetAgent().pathPending) wolf.SetState(new StandingState(wolf));
        else if (wolf.PlayerOnSight()) wolf.SetState(new ChasingState(wolf));
        else if (wolf.SoundHeard()) wolf.SetState(new StandingState(wolf));
    }

    public override void FixedUpdate(float fixedTime)
    {
        
    }

    private Vector3 GetTargetPos(Vector3 origin, float dist, int layermask)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist; //Posici�n aleatoria dentro de una esfera de radio dist
        randomDirection += origin; //Trasladamos la esfera a la posici�n del lobo

        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, dist, layermask); //Sampleamos una posici�n en el NavMesh correspondiente a la de la esfera

        return navHit.position;
    }
}
