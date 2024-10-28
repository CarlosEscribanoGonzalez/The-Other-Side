using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speaker : ATakeableObject
{
    [Header("Speaker: ")]
    [SerializeField] private AudioClip normalClip; //Canci�n feliz en el mundo normal
    [SerializeField] private AudioClip otherSideClip; //Canci�n turbia en el otro lado
    [SerializeField] private float echoInterval; //Intervalo entre generaciones de echo
    private static float clipTime; //Tiempo que se ha reproducido la m�sica (momento de la canci�n por el que se va)
    private bool wasPlaying = false;

    protected override void Awake()
    {
        base.Awake();
        GameObject.FindObjectOfType<SideChanger>().sideChanged += ChangeClip;
        ChangeClip(null, null);
    }

    private void OnEnable()
    {
        if (wasPlaying)
        {
            source.time = clipTime;
            source.Play();
            StartCoroutine(GenerateEcho());
        }
    }

    protected override void Update()
    {
        base.Update();
        if (source.isPlaying) clipTime += Time.deltaTime;
    }

    protected override void Use() //Se hace toggle al estado de la canci�n, controlando tambi�n la generaci�n de echo
    {
        if (source.isPlaying)
        {
            source.Pause();
            wasPlaying = false;
            StopAllCoroutines();
        }
        else
        {
            source.time = clipTime;
            wasPlaying = true;
            source.Play();
            StartCoroutine(GenerateEcho());
        }
    }

    public override void Take()
    {
        base.Take();
        GameObject.FindObjectOfType<DogCager>().SpeakerTaken();
    }

    private void ChangeClip(object s, EventArgs e) //Cuando se cambia de lado los clips se intercambian, manteniendo el estado de reproducci�n
    {
        //Intercambiamos los clips (lo que hace que se pausen):
        if (this.transform.position.y < 0) source.clip = otherSideClip;
        else source.clip = normalClip;
        //Ajustamos el clip para que cuadren temporalmente con el tiempo de reproducci�n:
        source.time = clipTime;
        //Se contin�a la reproducci�n si es que estaba sonando antes de cambiar de realidad:
        if (wasPlaying) source.Play();
    }

    IEnumerator GenerateEcho()
    {
        while (true)
        {
            echoGenerator.GenerateEcho(source.volume); //Igual que la tele, no queremos que sea demasiado inc�modo
            yield return new WaitForSeconds(echoInterval);
        }
    }

    public bool IsPlaying()
    {
        return source.isPlaying;
    }
}
