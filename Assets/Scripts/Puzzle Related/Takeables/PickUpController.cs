using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    [SerializeField] private float pickUpDistance; //Distancia máxima a la que se puede coger un objeto
    private InventoryManager inventory;
    private PlayerMovement player;
    private Camera cam;
    private ITakeableItem itemOnSight; //Ítem actual que en rango y posición de ser cogido
    private RaycastHit hit;
    private Ray ray;

    void Start()
    {
        inventory = GetComponent<InventoryManager>();
        cam = GetComponent<Camera>();
        player = GameObject.FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.GetCanMove()) return;
        CheckItemOnSight();
        CheckGrab();
        CheckThrow();
    }

    private void CheckItemOnSight()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition); //La dirección del rayo es del centro de la cámara hacia el frente
        if (Physics.Raycast(ray, out hit, pickUpDistance, LayerMask.GetMask("Takeable"))) //Se lanza un rayo con los parámetros deseados
        {
            ITakeableItem takeableItem = hit.collider.gameObject.GetComponent<ITakeableItem>();
            if (takeableItem != null && inventory.HasSpace()) 
                //Si el rayo colisiona con un objeto ITakeable y no se tiene cogido ningún objeto se le hace hover
            {
                takeableItem.HoverEnter();
                if(itemOnSight != null && itemOnSight != takeableItem) itemOnSight.HoverExit(); //Sólo se puede mirar a un objeto
                itemOnSight = takeableItem;
                return;
            }
        }
        if (itemOnSight != null) //De lo contrario se hace hover exit y se resetea itemOnSight
        {
            itemOnSight.HoverExit();
            itemOnSight = null;
        }
    }

    private void CheckGrab()
    {
        if(itemOnSight != null && Input.GetKeyDown(KeyCode.E)) //Si hay un objeto disponible para ser cogido se coge
        {
            inventory.AddObject(itemOnSight);
        }
    }

    private void CheckThrow()
    {
        if((Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.Q))) //Si hay un objeto en la mano y se pulsa la G se suelta al suelo
        {
            inventory.ThrowObject();
        }
    }

    public bool IsItemOnSight()
    {
        if (itemOnSight == null) return false;
        return true;
    }
}
