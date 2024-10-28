using UnityEngine;

public class StandingState : AWolfState
{
    private float standingTimer;
    private bool soundHeard;
    private float timerDivider = 1;

    public StandingState(IWolf wolf) : base(wolf) { }

    public override void Enter()
    {
        wolf.GetAnimator().SetTrigger("Stand");
        wolf.GetAnimator().SetBool("Attack", false);
        if (wolf.IsIntelligent()) timerDivider = 3;
        standingTimer = Random.Range(3.0f, 10.0f)/timerDivider;
    }

    public override void Exit()
    {
        
    }

    public override void Update(float deltaTime)
    {
        if (wolf.PlayerOnSight()) wolf.SetState(new ChasingState(wolf)); //Si ve al jugador lo persigue
        if (wolf.IsTactic()) return; //Los lobos tácticos sólo esperan y persiguen para matar

        //Si ha escuchado un ruido se gira en esa dirección
        if (wolf.SoundHeard())
        {
            Transform wolfTransform = wolf.GetTransform();
            wolfTransform.rotation = Quaternion.Slerp(wolfTransform.rotation, wolf.GetSoundDirection(), 5 * deltaTime);
            soundHeard = true;
            GameObject.FindObjectOfType<BarkManager>().StopGrowl();
        }

        if(!soundHeard) //Si no ha escuchado nada gruñe de vez en cuando
        {
            wolf.PlayMouthAudio("StandingState");
        }

        standingTimer -= deltaTime;
        if (standingTimer <= 0) //Cuando pasa el tiempo si ha escuchado un sonido hay posibilidades de que aúlle para ver. Si no se pone a caminar
        {
            if (soundHeard)
            {
                int rand = Random.Range(0, 3);
                if (rand == 0) wolf.SetState(new HowlState(wolf));
                else wolf.SetState(new WanderState(wolf));
            }
            else wolf.SetState(new WanderState(wolf));
        }
    }

    public override void FixedUpdate(float fixedTime)
    {
        
    }
}
