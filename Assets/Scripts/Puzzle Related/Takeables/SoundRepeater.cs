using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SoundRepeater : ATakeableObject
{
    [Header("Sound Repeater: ")]
    [SerializeField] private Renderer isOnLight; //Luz de encendido
    [SerializeField] private Material offMaterial; //Material de la luz de encendido cuando está apagado
    [SerializeField] private Material onMaterial; //Material de la luz de encendido cuando está encendido
    [SerializeField] private AudioSource sensorSource;
    private bool isOn = false; //Si está encendido o no
    private bool cooldown = false; //Si no está esto peta Unity por sonidos repetidos infinitos. NO QUITAR.

    protected override void Use()
    {
        //Se cambian el estado y los materiales:
        isOn = !isOn;
        if (isOn) isOnLight.material = onMaterial;
        else isOnLight.material = offMaterial;
        //Se reproduce el sonido asociado:
        source.Play();
        echoGenerator.GenerateEcho(source.volume);
    }

    private void OnEnable()
    {
        if (canBeUsed) cooldown = false;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.CompareTag("Echo") && isOn && !cooldown) 
        {
            cooldown = true;
            StartCoroutine(Cooldown());
            sensorSource.Play();
            echoGenerator.GenerateEcho(sensorSource.volume);
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(0.2f);
        cooldown = false;
    }

    public void TurnOff()
    {
        isOn = false;
        isOnLight.material = offMaterial;
        source.Play();
        echoGenerator.GenerateEcho(source.volume);
    }
}
