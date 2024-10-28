using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Remote : ATakeableObject
{
    [SerializeField] private TV[] tvs;
    [SerializeField] private ColorDoor door;
    private bool used = false;

    protected override void Use()
    {
        source.Play(); //Suena independientemente de si puede funcionar o no
        echoGenerator.GenerateEcho(source.volume);
        if (controller.transform.position.y < 0) return; //El mando sólo funciona en el mundo normal
        if (!used && door != null) //La primera vez que se usa el mando se disuelve la puerta y se da la siguiente pista
        {
            door.DissolveDoor();
            Notepad.AddMaxDisplay();
            used = true;
        }
        
        foreach(TV t in tvs)
        {
            t.ToggleTV();
        }
    }
}
