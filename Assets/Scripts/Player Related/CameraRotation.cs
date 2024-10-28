using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private float sensitivityX;
    [SerializeField] private float sensitivityY;
    private PlayerMovement player;
    private float yRotation = 0; //Acumulación de rotación vertical
    
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked; //Se bloquea el ratón
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (!player.GetCanMove()) return;
        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        player.transform.Rotate(Vector3.up, mouseX); //Se rota al jugador respecto al eje X
        yRotation -= mouseY; //La rotación en Y se tiene que almacenar en una variable, pues no se puede rotar al jugador al igual que en X
        yRotation = Mathf.Clamp(yRotation, -75, 75); //No puede rotar demasiado
        transform.localRotation = Quaternion.Euler(yRotation, 0, 0); //Se rota únicamente la cámara
    }

    public void ResetYRotation() //Se resetea la rotación cuando se cambia de lados
    {
        yRotation = 0;
    }

    public void SetSensitivity(float newSensitivity)
    {
        sensitivityX = newSensitivity;
        sensitivityY = newSensitivity;
    }
}
