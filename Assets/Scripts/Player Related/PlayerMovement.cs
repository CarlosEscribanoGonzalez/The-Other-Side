using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed; //Velocidad de caminar
    [SerializeField] private float crouchSpeed; //Velocidad al ir agachado
    private float currentSpeed; //Velocidad actual
    private CharacterController controller;
    private Animator anim;
    private Vector3 movement; //Vector de movimiento del jugador
    [SerializeField] private bool canMove = true; //Indica si se puede mover o no
    private bool isCrouching = false; //Indica si está agachado o no
    public bool canBeSeen = false; //Indica si el jugador puede ser visto o no (al ser colisionado por un echo)
    [SerializeField] private float timeOnSight = 0.1f; //Ventana de tiempo en la que el jugador puede ser visto

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = movementSpeed;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!canMove) return;
        //Cálculo de las velocidades en los ejes:
        float movementX = Input.GetAxis("Horizontal");
        float movementZ = Input.GetAxis("Vertical");
        float movementY = 0;
        //Aplicación del movimiento:
        movement = new Vector3(movementX, movementY, movementZ); //Se crea el vector de movimiento (coordenadas globales)
        if (movementX != 0 && movementZ != 0) movement.Normalize(); //Se normaliza la velocidad para no ir más rápido en diagonal
        movement *= currentSpeed * Time.deltaTime;
        if (!controller.isGrounded) movement.y = -9.81f * Time.deltaTime; //Si no está tocando el suelo se calcula la fuerza de gravedad
        movement = transform.TransformDirection(movement); //Se pasa el vector de movimiento a coordenadas locales del jugador
        controller.Move(movement); //Se mueve al jugador

        CheckCrouch(); //Controla el agachado
    }

    private void CheckCrouch() //Si el jugador se agacha se intercambian las velocidades y se realiza la animación correspondiente
    {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl)) 
        {
            anim.SetTrigger("Toggle");
        }
    }

    public void ToggleSpeed()
    {
        if (currentSpeed == movementSpeed)
        {
            isCrouching = true;
            currentSpeed = crouchSpeed;
        }
        else
        {
            currentSpeed = movementSpeed;
            isCrouching = false;
        }
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }

    public void SetCanMove(bool m)
    {
        canMove = m;
    }

    public bool GetCanMove()
    {
        return canMove;
    }

    public float GetMovement()
    {
        return movement.magnitude;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Echo"))
        {
            StopAllCoroutines();
            StartCoroutine(CanBeSeen());
        }
        
        if (other.CompareTag("WolfAttack")) GetComponent<LifeManager>().PlayerDeath();
    }

    IEnumerator CanBeSeen()
    {
        canBeSeen = true;
        yield return new WaitForSeconds(timeOnSight);
        canBeSeen = false;
    }
}
