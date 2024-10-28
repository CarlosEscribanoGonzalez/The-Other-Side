using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorChimes : MonoBehaviour
{
    private AudioSource source;
    private EchoGenerator echoGenerator;
    [SerializeField] private Rigidbody[] chimes; //Todos los tubos de metal
    private bool inCooldown = false;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        echoGenerator = GetComponent<EchoGenerator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Echo")||other.CompareTag("Chimes")) return; //No se pillan las colisiones con el echo
        if (inCooldown) return;
        StartCoroutine(Cooldown());
        source.Play();
        echoGenerator.GenerateEcho(source.volume);
        foreach(Rigidbody chime in chimes) //Fuerza aleatoria para simular el movimiento:
        {
            float x = Random.Range(-1.0f, 1.0f);
            float y = Random.Range(-1.0f, 1.0f);
            float z = Random.Range(-1.0f, 1.0f);
            chime.AddForce(new Vector3(x, y, z), ForceMode.Impulse);
        }
    }

    IEnumerator Cooldown()
    {
        inCooldown = true;
        yield return new WaitForSeconds(2f);
        inCooldown = false;
    }
}
