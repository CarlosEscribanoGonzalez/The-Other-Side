using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarkManager : MonoBehaviour
{
    private AudioSource source;
    private EchoGenerator echoGenerator;
    [SerializeField] private AudioClip howlClip;
    [SerializeField] private AudioClip growlClip;
    [SerializeField] private AudioClip chaseClip;
    [SerializeField] private AudioClip attackClip;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        echoGenerator = GetComponent<EchoGenerator>();
    }

    public void PlayMouthAudio(string state)
    {
        if (state.Equals("StandingState") || state.Equals("WanderState")) Growl();
        else if (state.Equals("HowlState")) Howl();
        else if (state.Equals("ChasingState")) ChaseGrowl();
        else if (state.Equals("AttackState")) Attack();
    }

    private void Howl()
    {
        source.clip = howlClip;
        source.volume = 1f;
        source.Play();
        echoGenerator.GenerateEcho(0.4f);
    }

    private void Growl()
    {
        if (source.isPlaying) return; //No se puede gruñir si está haciendo otro sonido
        int rand = Random.Range(0, 5000);
        if(rand == 0)
        {
            source.clip = growlClip;
            source.volume = 0.5f;
            source.Play();
            echoGenerator.GenerateEcho(0.05f);
        }
    }

    private void ChaseGrowl()
    {
        source.clip = chaseClip;
        source.volume = 0.8f;
        source.Play();
        echoGenerator.GenerateEcho(0.4f);
    }

    private void Attack()
    {
        source.clip = attackClip;
        source.volume = 1f;
        source.Play();
        echoGenerator.GenerateEcho(0.4f);
    }

    public void StopGrowl()
    {
        if (source.isPlaying && source.clip == growlClip) source.Stop();
    }
}
