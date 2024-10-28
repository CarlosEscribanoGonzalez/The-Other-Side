using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TV : MonoBehaviour
{
    private VideoPlayer video; //Reproductor de vídeo
    private AudioSource source;
    [SerializeField] private Renderer screen; //Renderer del plano que hace de pantalla
    [SerializeField] private Material renderMaterial; //Material donde se reproduce el vídeo
    [SerializeField] private Material blackMaterial; //Material de la pantalla cuando está apagada
    [SerializeField] private float echoInterval; //Intervalo entre generación de grupos de echo
    private EchoGenerator echoGenerator;
    [SerializeField] private bool forceView = false; //Indica que la visualización de la televisión es forzada a una posición (final del juego)
    [SerializeField] private Transform watchTransform; //Posición desde la que se ve el vídeo si forceView es true
    private bool updateCam = false; //Indica si el update actualiza o no la cámara a watchTransform
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

    public void ToggleTV() //Enciende y apaga la tele, cambiando los materiales de la pantalla según su estado
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
            echoGenerator.GenerateEcho(source.volume/2); //No queremos que sea demasiado incómodo
        }
    }

    private void SyncAudioAndVideo(object s, EventArgs e)
    {
        source.time = (float)video.time;
    }
}
