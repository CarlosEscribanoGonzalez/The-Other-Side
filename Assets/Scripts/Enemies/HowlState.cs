using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowlState : AWolfState
{
    public HowlState(IWolf wolf) : base(wolf) { }

    public override void Enter()
    {
        wolf.GetAnimator().SetTrigger("Howl");
        wolf.PlayMouthAudio("HowlState"); //Reproducir el sonido y el echo
    }

    public override void Exit()
    {

    }

    public override void Update(float deltaTime)
    {
        //En el momento que acaba la animación se cambia de estado
        AnimatorStateInfo stateInfo = wolf.GetAnimator().GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("howl"))
        {
            if (stateInfo.normalizedTime >= 1.0f) //Cuando acaba la animación decide si perseguir al jugador si lo ha visto o seguir caminando
            {
                if(wolf.PlayerOnSight()) wolf.SetState(new ChasingState(wolf));
                else wolf.SetState(new WanderState(wolf));
            }
        }
    }

    public override void FixedUpdate(float fixedTime)
    {

    }
}
