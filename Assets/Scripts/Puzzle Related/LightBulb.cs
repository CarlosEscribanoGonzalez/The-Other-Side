using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulb : MonoBehaviour
{
    [SerializeField] private Material offMaterial; //Material del cristal cuando está apagado
    [SerializeField] private Material onMaterial; //Material del cristal cuando está encendido
    [SerializeField] private float onInterval = 3; //Tiempo que están encendidas las luces al ser golpeadas por el echo
    [SerializeField] private Renderer bulbRenderer; //Renderer del cristal
    private bool isOn; //Si está encendida ya o no
    private LightPanel[] panels; //Panel de luces del puzle
    private AudioSource source;
    private EchoGenerator echoGenerator;

    private void Awake()
    {
        panels = GameObject.FindObjectsOfType<LightPanel>();
        source = GetComponent<AudioSource>();
        echoGenerator = GetComponent<EchoGenerator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Echo")) return; //Si se ha completado el puzle las bombillas quedan encendidas
        if (other.GetComponent<Echolocation>().GetColor() != Color.white) return;
        bulbRenderer.material = onMaterial;
        if (!isOn) //Si está apagada se enciende y notifica que se ha encendido
        {
            source.Play();
            echoGenerator.GenerateEcho(source.volume/2);
            foreach(LightPanel p in panels) p.TurnOnLight();
            isOn = true;
        }
        StopAllCoroutines(); //Se cancela cualquier corrutina anterior para resetear el apagado
        if (!panels[0].completed) StartCoroutine(ResetAfterTimer());
    }

    IEnumerator ResetAfterTimer()
    {
        yield return new WaitForSeconds(onInterval);
        bulbRenderer.material = offMaterial;
        foreach (LightPanel p in panels) p.TurnOffLight();
        isOn = false;
        source.Play();
        echoGenerator.GenerateEcho(source.volume/2);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Rigidbody>().AddForce(-collision.transform.forward * 5, ForceMode.Impulse);
    }
}
