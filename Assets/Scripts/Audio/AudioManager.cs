using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup normalMixer; //AudioMixer del mundo normal
    [SerializeField] private AudioMixerGroup otherSideMixer; //AudioMixer del otro lado
    [SerializeField] private AudioListener[] listeners;
    private List<AudioSource> sources; //Todos los AudioSources de la escena
    private GameObject player;

    [Header("Final animation: ")]
    [SerializeField] private float volume;
    [SerializeField] private float pitch;
    [SerializeField] private float distortion;
    [SerializeField] private float cutoff;
    [SerializeField] private float decay;
    [SerializeField] private float delay;
    [SerializeField] private float drymix;
    [SerializeField] private float wetmix;
    private bool animated = false;

    private void Awake()
    {
        UpdateSources(null);
        GameObject.FindObjectOfType<SideChanger>().sideChanged += ToggleMixer;
        player = GameObject.FindWithTag("Player");
        ToggleMixer(null, null);
        GetComponent<Animator>().enabled = false;
    }

    private void Start()
    {
        InitializeMaxDistance();
    }

    private void Update()
    {
        if (!animated) return;
        normalMixer.audioMixer.SetFloat("normal_volume", volume);
        //normalMixer.audioMixer.SetFloat("normal_pitch", pitch/100);
        normalMixer.audioMixer.SetFloat("normal_distortion", distortion);
        normalMixer.audioMixer.SetFloat("normal_cutoff", cutoff);
        normalMixer.audioMixer.SetFloat("normal_decay", decay/100);
        normalMixer.audioMixer.SetFloat("normal_delay", delay);
        normalMixer.audioMixer.SetFloat("normal_wetmix", wetmix / 100);
        normalMixer.audioMixer.SetFloat("normal_drymix", drymix / 100);
    }

    private void ToggleMixer(object s, EventArgs e) //Cuando se cambia de lado se intercambian los mixers
    {
        foreach(AudioSource a in sources)
        {
            if (player.transform.position.y > 0) a.outputAudioMixerGroup = normalMixer;
            else a.outputAudioMixerGroup = otherSideMixer;
        }
        if (player.transform.position.y > 0) GetComponent<AudioSource>().Play();
        else GetComponent<AudioSource>().Pause();
    }

    private void InitializeMaxDistance()
    {
        AnimationCurve customCurve = new AnimationCurve();
        customCurve.AddKey(0.1f, 1);
        customCurve.AddKey(10f, 0.5f);
        customCurve.AddKey(90f, 0.25f);
        customCurve.AddKey(100f, 0f);
        foreach(AudioSource a in sources)
        {
            a.rolloffMode = AudioRolloffMode.Custom;
            a.maxDistance = 100;
            a.SetCustomCurve(AudioSourceCurveType.CustomRolloff, customCurve);
        }
    }

    public void UpdateSources(AudioSource newSource)
    {
        if (newSource == null) sources = GameObject.FindObjectsOfType<AudioSource>().ToList<AudioSource>();
        else sources.Add(newSource);
        InitializeMaxDistance();
    }

    public void OnAnimationStarted()
    {
        transform.Find("RemoteSFX").GetComponent<AudioSource>().Play(); //Se hace el sonido del mando, el cual no puede hacerlo al estar desactivado
        animated = true;
    }

    public void OnAnimationEnded()
    {
        animated = false;
        normalMixer.audioMixer.SetFloat("normal_volume", 0);
        normalMixer.audioMixer.SetFloat("normal_pitch", 1);
        normalMixer.audioMixer.SetFloat("normal_distortion", 0);
        normalMixer.audioMixer.SetFloat("normal_cutoff", 22000);
        normalMixer.audioMixer.SetFloat("normal_decay", 0);
        normalMixer.audioMixer.SetFloat("normal_delay", 1);
        normalMixer.audioMixer.SetFloat("normal_wetmix", 0);
        normalMixer.audioMixer.SetFloat("normal_drymix", 1);
    }

    public void ToggleListeners()
    {
        foreach(AudioListener l in listeners) l.enabled = !l.enabled;
    }
}
