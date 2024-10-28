using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoCamera : ATakeableObject
{
    [Header("Camera: ")]
    [SerializeField] private GameObject otherCam; //Cámara que graba lo que pasa al otro lado
    [SerializeField] private Renderer screen; //Pantalla sobre la que se reproduce el vídeo
    [SerializeField] private float cooldownTime;
    public float grabDefaultHeight; //Altura del grab transform. Actualizado desde el SideChanger
    private bool available = true; //Indica si está disponible o no (en cooldown)

    //LA PANTALLA SE APAGA ENTRE TRANSICIONES; SCRIPT -> SideChanger.

    protected override void Awake()
    {
        base.Awake();
        GameObject.FindObjectOfType<SideChanger>().sideChanged += ToggleShaders;
        ToggleShaders(null, null); //Se inicializan los shaders
        grabDefaultHeight = GameObject.Find("GrabTransform").transform.position.y;
    }

    protected override void Update()
    {
        base.Update();
        float playerOffset = GameObject.FindObjectOfType<CharacterController>().height; //La cámara varía con la altura del jugador (si está agachado o no)
        otherCam.transform.position = new Vector3(this.transform.position.x, -grabDefaultHeight + playerOffset, this.transform.position.z);
        otherCam.transform.forward = -this.transform.up;
    }

    private void OnEnable()
    {
        ToggleShaders(null, null);
        grabDefaultHeight = GameObject.Find("GrabTransform").transform.position.y;
        if (GameObject.FindObjectOfType<PlayerMovement>().IsCrouching()) grabDefaultHeight += 1;
        if (canBeUsed) available = true;
    }

    protected override void Use()
    {
        if (available)
        {
            source.Play();
            echoGenerator.GenerateEcho(source.volume); //Si está en el otro lado genera ondas en persona
            otherCam.GetComponent<EchoGenerator>().GenerateEcho(source.volume); //Genera ondas en el otro lado (para el vídeo)
            available = false;
            StartCoroutine(Cooldown());
        }
    }

    public override void Take() //La pantalla sólo se activa cuando se tiene en la mano
    {
        base.Take();
        screen.gameObject.SetActive(true);
    }

    public override void Throw()
    {
        base.Throw();
        screen.gameObject.SetActive(false);
    }

    private void ToggleShaders(object s, EventArgs e) //Cambia los shaders que imitan el shader full screen activo
    {
        if(GameObject.FindWithTag("Player").transform.position.y < 0)
        {
            screen.material.SetFloat("_ditherActive", 1);
            screen.material.SetFloat("_noiseActive", 1);
            screen.material.SetFloat("_colorActive", 1);
        }
        else
        {
            screen.material.SetFloat("_ditherActive", 0);
            screen.material.SetFloat("_noiseActive", 0.5f);
            screen.material.SetFloat("_colorActive", 0);
        }
    }

    public void ToggleScreen(bool active) //Llamado por SideChanger, desactiva temporalmente la cámara mientras dura el cambio de lado
    {
        screen.gameObject.SetActive(active);
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        available = true;
    }
}
