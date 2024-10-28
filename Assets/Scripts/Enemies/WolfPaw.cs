using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfPaw : MonoBehaviour
{
    private AudioSource source;
    private EchoGenerator echoGenerator;
    private WolfBehaviour wolf;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        echoGenerator = GetComponent<EchoGenerator>();
        wolf = GameObject.FindObjectOfType<WolfBehaviour>();
    }

    private void Update() //Cuando corre el audio es más fuerte
    {
        if (wolf.GetState().ToString().Equals("ChasingState")) source.volume = 0.4f;
        else source.volume = 0.1f;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            source.Play();
            echoGenerator.GenerateEcho(source.volume/3.5f);
        }
    }
}
