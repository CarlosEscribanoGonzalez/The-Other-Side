using System;
using TMPro;
using UnityEngine;

public class Monitor : MonoBehaviour
{
    private PlayerMovement player;
    private Transform playerCam;

    [Header("Audio Related: ")]
    [SerializeField] private AudioClip[] clips; //Clips de correcto(1) e incorrecto(0)
    private AudioSource source;
    private EchoGenerator echoGenerator;

    [Header("Password Related: ")]
    [SerializeField] private TextMeshPro passwordText; //Texto en el que se muestra la contraseña
    [SerializeField] private TextMeshPro inputText; //Texto en el que se muestra el input
    private static string password; //Contraseña que se muestra
    private static string input; //Texto del input del jugador
    private const int PASSWORD_LENGTH = 6;
    private int currentLength = 0; //Longitud actual de la cadena de caracteres
    private const string AVAILABLE_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private System.Random rand = new System.Random();

    [Header("Use Related: ")]
    [SerializeField] private bool operative; //Indica si el ordenador está operativo (si tiene o no teclado)
    [SerializeField] private GameObject keyboard;
    [SerializeField] private GameObject advisoryText; //Texto que indica con qué botón usarlo al acercarte
    [SerializeField] private Transform onUseTransform; //Posición de la cámara del jugador al usar el ordenador
    Vector3 defaultPosition; //Posición por defecto de la cámara del jugador
    private bool onRange = false; //Si está en rango de ser usado
    private bool onUse = false; //Si está en uso
    private static bool guessed = false; //Si la contraseña ha sido acertada

    [SerializeField] private ColorDoor door;
    

    void Awake()
    {
        SetNewPassword(null, null);
        GameObject.FindObjectOfType<SideChanger>().sideChanged += SetNewPassword; //Cada cambio de lugar cambia la contraseña
        GameObject.FindObjectOfType<SideChanger>().sideChanged += TriggerExit; //Un problema hace que se buggee al cambiar de lado cerca del ordenador
        player = GameObject.FindObjectOfType<PlayerMovement>();
        playerCam = GameObject.FindWithTag("MainCamera").transform;
        defaultPosition = playerCam.localPosition;
        advisoryText.SetActive(false);
        source = GetComponent<AudioSource>();
        echoGenerator = GetComponent<EchoGenerator>();
    }

    private void Update()
    {
        passwordText.text = password; //Como son estáticos, hay que hacer esto en el update por si los datos se han cambiado en el otro lado
        inputText.text = input;
        advisoryText.transform.forward = advisoryText.transform.position - playerCam.transform.position;

        if (!operative) return;

        if (onUse) IntroduceInput(); //Si está en uso se pilla entrada por teclado, si no se restablece la posición normal de la cámara
        else if(playerCam.localPosition != defaultPosition) playerCam.localPosition = Vector3.Lerp(playerCam.localPosition, defaultPosition, 10 * Time.deltaTime);

        if (onRange && Input.GetKeyDown(KeyCode.E)) //Si se le da a la E estando en rango se usa el ordenador
        {
            if (GameObject.FindObjectOfType<PickUpController>().IsItemOnSight()) return; //Si hay un objeto disponible a ser agarrado no se usa el ordenador
            onUse = true;
            player.SetCanMove(false);
            advisoryText.GetComponent<TextMeshPro>().text = "Pulsa 'ESC' para salir";
        }
        else if (onUse && Input.GetKeyUp(KeyCode.Escape)) //Si está en uso y se le da al ESC se deja de usar
        {
            onUse = false;
            player.SetCanMove(true);
            advisoryText.GetComponent<TextMeshPro>().text = "Pulsa 'E' para usar";
        }
    }

    private void SetNewPassword(object s, EventArgs e)
    {
        if (guessed) return; //El ordenador "deja de funcionar" cuando se ha acertado la contraseña
        password = "";
        input = "";
        currentLength = 0;
        for (int i = 0; i < PASSWORD_LENGTH; i++)
        {
            password += AVAILABLE_CHARS[rand.Next(AVAILABLE_CHARS.Length)];
        }
    }

    private void IntroduceInput() //Pone la cámara del jugador en el lugar correcto y pilla su input
    {
        playerCam.position = Vector3.Lerp(playerCam.position, onUseTransform.position, 10*Time.deltaTime);
        playerCam.rotation = Quaternion.Slerp(playerCam.rotation, onUseTransform.rotation, 10*Time.deltaTime);
        if (guessed) return; //No hay input que valga si se ha adivinado ya
        foreach (char c in Input.inputString)
        {
            keyboard.GetComponent<AudioSource>().Play();
            keyboard.GetComponent<EchoGenerator>().GenerateEcho(keyboard.GetComponent<AudioSource>().volume/this.transform.localScale.x);

            if (c == '\b' && input.Length != 0) //Se presiona hacia atrás
            {
                input = input.Substring(0, input.Length - 1);
                currentLength--;
            }
            else if (AVAILABLE_CHARS.Contains(c) && currentLength < PASSWORD_LENGTH)
            {
                input += c;
                currentLength++;
            }

            if (currentLength == PASSWORD_LENGTH)
            {
                CheckIfCorrect();
            }
        }
    }

    private void CheckIfCorrect() //Se comprueba si se ha acertado o no y se reproduce el audio asociado
    {
        if(password.Equals(input))
        {
            door.DissolveDoor();
            Notepad.AddMaxDisplay();
            source.clip = clips[1];
            guessed = true;
        }
        else
        {
            SetNewPassword(null, null);
            source.clip = clips[0];
        }
        source.Play();
        echoGenerator.GenerateEcho(source.volume);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && operative)
        {
            onRange = true;
            advisoryText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && operative)
        {
            onRange = false;
            advisoryText.SetActive(false);
        }
    }

    private void TriggerExit(object s, EventArgs e)
    {
        onRange = false;
        advisoryText.SetActive(false);
    }
}
