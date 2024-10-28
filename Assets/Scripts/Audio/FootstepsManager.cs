using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsManager : MonoBehaviour
{
    private AudioSource source;
    private EchoGenerator echoGenerator; //Encargado de generar ondas de echo al recibir la potencia con la que se ha reproducido un audio
    private PlayerMovement player;
    private float movement = 0; //En esta variable se almacena el movimiento del jugador para saber si se ha quedado quieto o no
    [SerializeField] private float stepInterval; //Intervalo de tiempo entre paso y paso
    private float stepTimer; 
    private bool disabled = false; //Indica si puede reproducir el audio o no (para cuando cambia de realidad)

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        echoGenerator = GetComponent<EchoGenerator>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        stepTimer = stepInterval;
        GameObject.FindObjectOfType<SideChanger>().sideChanged += TemporalDisable;
    }

    private void Update()
    {
        if (!player.GetCanMove()) return;

        if (disabled || player.IsCrouching()) //Si no puede reproducir audios se resetea el timer
        {
            stepTimer = stepInterval;
            return;
        }

        movement = player.GetMovement();
        if (movement > 0) //Si se está moviendo el temporizador decrece. Al llegar a 0 se reinicia y se da un paso
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                DoStep();
                stepTimer = stepInterval;
            }
        }
        else stepTimer = stepInterval;
    }

    public void DoStep()
    {
        source.Play();
        //Se cambia de pie el origen del sonido (para efectuar ondas desde ambos pies y que sea más inmersivo)
        transform.localPosition = new Vector3(-transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        //Sólo se generan ondas en el otro lado
        echoGenerator.GenerateEcho(source.volume);
    }

    private void TemporalDisable(object s, EventArgs e) //Cuando se cambia de lado se para temporalmente la generación de sonido
    {
        StartCoroutine(Disable());
    }

    IEnumerator Disable()
    {
        disabled = true;
        yield return new WaitForSeconds(0.5f);
        disabled = false;
    }
}
