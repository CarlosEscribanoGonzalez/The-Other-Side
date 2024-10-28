using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideChanger : MonoBehaviour
{
    [SerializeField] private float offset = 0.5f;
    private CharacterController characterController;
    private Animator anim; //Animador de la cámara para la transición entre mundos
    private PlayerMovement playerMovement;
    public EventHandler sideChanged; //Borra las ondas existentes y su producción, pausa temporalmente el SFX de caminar...
    [SerializeField] private bool canChange = true;
    private AudioSource source;
    [SerializeField] private AudioClip[] clips; //0 -> Comenzar cambio; 1 -> Finalizar cambio
    private bool inCooldown = false;
    [SerializeField] private float cooldownTime = 1;

    private void Awake()
    {
        characterController = GameObject.FindObjectOfType<CharacterController>();
        anim = GetComponent<Animator>();
        anim.enabled = false; //Las transiciones son automáticas. Hay que desactivarlo para evitar que comience la primera animación sola
        playerMovement = GameObject.FindObjectOfType<PlayerMovement>();
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canChange && playerMovement.GetCanMove())
        {
            if (!inCooldown) PerformChange();
        }
    }

    public void PerformChange()
    {
        anim.enabled = true; //Comienza la animación de cambio de mundo
        VideoCamera cam = GameObject.FindObjectOfType<VideoCamera>();
        if(cam != null) cam.ToggleScreen(false); //Se apaga la cámara de vídeo
        playerMovement.SetCanMove(false);
        source.clip = clips[0];
        source.Play();
    }

    public void ChangeSide() //Llamado por la animación de cambio de mundo
    {
        characterController.enabled = false; //Da problemas al alterar la transform, así que se desactiva
        this.transform.parent.position = new Vector3(transform.parent.position.x, -transform.parent.position.y + offset, transform.parent.position.z); //Alterna entre mundos
        characterController.enabled = true;
        sideChanged?.Invoke(this, EventArgs.Empty); //Notifica a aquellos interesados
        if (anim.GetBool("Dead")) ToggleAudioListener();
        source.clip = clips[1];
        source.Play();
    }

    public void OnAnimationEnded() //Llamada por la animación de cambio de mundo una vez termina. Devuelve el control al jugador
    {
        anim.enabled = false;
        anim.SetBool("Dead", false);
        playerMovement.SetCanMove(true);
        GetComponent<CameraRotation>().ResetYRotation();
        VideoCamera videoCam = GameObject.FindObjectOfType<VideoCamera>();
        if (videoCam != null)
        {
            videoCam.ToggleScreen(true);
            videoCam.grabDefaultHeight = transform.Find("GrabTransform").position.y;
            if (playerMovement.IsCrouching()) videoCam.grabDefaultHeight -= 1;
        }
        StartCoroutine(Cooldown());
    }

    public void SetCanChange(bool b)
    {
        canChange = b;
    }

    public void PerformDeadAudios()
    {
        GameObject.FindObjectOfType<LifeManager>().PlayAudios();
    }

    public void ToggleAudioListener()
    {
        GameObject.FindObjectOfType<AudioManager>().ToggleListeners();
    }

    IEnumerator Cooldown()
    {
        inCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        inCooldown = false;
    }
}
