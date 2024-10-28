using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : AWolfState
{
    private float speed = 0; //Velocidad de ataque, editable para hacer bien el jumpscare
    private float timer = 2.0f; //Temporizador que cuando llega a 0 fuerza un cambio de estado,
                                //para solucionar un bug que lo deja en este estado después de atacar
    public AttackState(IWolf wolf) : base(wolf) { }

    public override void Enter()
    {
        wolf.GetAnimator().SetBool("Attack", true);
        wolf.GetAgent().speed = speed;
        //wolf.GetAgent().SetDestination(GameObject.FindWithTag("Player").transform.position);
        wolf.PlayMouthAudio("AttackState");
    }

    public override void Exit()
    {
        wolf.GetAnimator().SetBool("Attack", false);
    }

    public override void Update(float deltaTime)
    {
        //En el momento que acaba la animación se cambia de estado al defecto
        AnimatorStateInfo stateInfo = wolf.GetAnimator().GetCurrentAnimatorStateInfo(0);
        /*if (stateInfo.IsName("breathes"))
        {
            wolf.SetState(new StandingState(wolf));
        }*/
        timer -= deltaTime;
        if (timer <= 0) wolf.SetState(new StandingState(wolf));
    }

    public override void FixedUpdate(float fixedTime)
    {

    }
}
