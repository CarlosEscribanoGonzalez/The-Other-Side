using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Clock : MonoBehaviour
{
    private AudioSource source;
    private EchoGenerator[] echoGenerators;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        echoGenerators = GetComponents<EchoGenerator>();
    }

    void Update()
    {
        if (!source.isPlaying)
        {
            source.Play();
            StartCoroutine(Echo());
        }
    }

    IEnumerator Echo()
    {
        foreach(EchoGenerator generator in echoGenerators) generator.GenerateEcho(source.volume);
        yield return new WaitForSeconds(1.5f);
        foreach (EchoGenerator generator in echoGenerators) generator.GenerateEcho(source.volume-0.2f);
    }
}
