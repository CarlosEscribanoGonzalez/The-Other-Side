using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TV : MonoBehaviour
{
    private VideoPlayer video; //Reproductor de v�deo
    private AudioSource source;
    [SerializeField] private Renderer screen; //Renderer del plano que hace de pantalla
    [SerializeField] private Material renderMaterial; //Material donde se reproduce el v�deo
    [SerializeField] private Material blackMaterial; //Material de la pantalla cuando est� apagada
    [SerializeField] private float echoInterval; //Intervalo entre generaci�n de grupos de echo
    private EchoGenerator echoGenerator;
    [SerializeField] private bool forceView = false; //Indica que la visualizaci�n de la televisi�n es forzada a una posici�n (final del juego)
    [SerializeField] private Transform watchTransform; //Posici�n desde la que se ve el v�deo si forceView es true
    private bool updateCam = false; //Indica si el update actualiza o no la c�mara a watchTransform
    private bool videoEnded = false;

    private void Awake()
    {
        video = GetComponent<VideoPlayer>();
        source = GetComponent<AudioSource>();
        video.loopPointReached += OnVideoEnded;
        echoGenerator = GetComponent<EchoGenerator>();
        GameObject.FindObjectOfType<SideChanger>().sideChanged += SyncAudioAndVideo;
    }

    private void Update()
    {
        if (!updateCam) return;
        GameObject.FindObjectOfType<PlayerMovement>().SetCanMove(false);
        Transform playerCam = GameObject.FindWithTag("MainCamera").transform;
        playerCam.position = Vector3.MoveTowards(playerCam.position, watchTransform.position, 1.5f * Time.deltaTime);
        playerCam.rotation = Quaternion.Slerp(playerCam.rotation, watchTransform.rotation, 1.5f * Time.deltaTime);
    }

    private void OnVideoEnded(object s)
    {
        StopAllCoroutines();
        videoEnded = true;
        screen.material = blackMaterial;
    }

    public void ToggleTV() //Enciende y apaga la tele, cambiando los materiales de la pantalla seg�n su estado
    {
        if (video.isPlaying)
        {
            video.Pause();
            source.Pause();
            screen.material = blackMaterial;
            StopAllCoroutines();
        }
        else if(!videoEnded)
        {
            video.Play();
            source.Play();
            screen.material = renderMaterial;
            StartCoroutine(GenerateEchos());
            if (forceView)
            {
                updateCam = true;
                GameObject.Find("GrabTransform").gameObject.SetActive(false);
                GameObject.Find("UI").gameObject.SetActive(false);
                GameObject.FindObjectOfType<ShaderManager>().GetComponent<Animator>().enabled = true;
                GameObject.FindObjectOfType<AudioManager>().GetComponent<Animator>().enabled = true;
                Monitor[] monitors = GameObject.FindObjectsOfType<Monitor>(); //Se desactiva el monitor para que se pueda hacer el Lerp bien
                foreach (Monitor m in monitors) m.enabled = false;
            }
        }
    }

    IEnumerator GenerateEchos()
    {
        while (true)
        {
            yield return new WaitForSeconds(echoInterval);
            echoGenerator.GenerateEcho(source.volume/2); //No queremos que sea demasiado inc�modo
        }
    }

    private void SyncAudioAndVideo(object s, EventArgs e)
    {
        source.time = (float)video.time;
    }
}
