using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChasingState : AWolfState
{
    private float speed = 15; //Velocidad de movimiento al correr
    private float stopOffset = 3; //Offset a partir del cual ataca (sumado a la stoppingDistance del agente)
    private Transform player;

    public ChasingState(IWolf wolf) : base(wolf) { }

    public override void Enter()
    {
        player = GameObject.FindObjectOfType<PlayerMovement>().transform;
        wolf.GetAnimator().SetTrigger("Run");
        wolf.GetAgent().speed = speed;
        wolf.GetAgent().SetDestination(GetTargetPos());
        wolf.PlayMouthAudio("ChasingState");
        wolf.GetAnimator().SetBool("Attack", false);
    }

    public override void Exit()
    {
        wolf.GetAgent().velocity = Vector3.zero; //Se para en seco para que no deslice
        wolf.GetAgent().ResetPath();
    }

    public override void Update(float deltaTime)
    {
        wolf.GetAgent().SetDestination(GetTargetPos()); //La posición se actualiza dinámicamente para que siempre siga al jugador aunque se mueva
        if (Vector3.Distance(wolf.GetTransform().position, player.transform.position) <= stopOffset) //Si llega a posición de atacar ataca
        {
            wolf.SetState(new AttackState(wolf));
        }
        
        if(Vector3.Distance(GetTargetPos(), wolf.GetTransform().position) > 100) //Si el jugador cambia de lado antes de se alcanzado se queda quieto
        {
            wolf.SetState(new StandingState(wolf));
        }
    }

    public override void FixedUpdate(float fixedTime)
    {

    }

    private Vector3 GetTargetPos() //Devuelve la posición del jugador
    {
        NavMeshHit navHit;
        NavMesh.SamplePosition(player.position, out navHit, 200, 1);
        return navHit.position;
    }
}
