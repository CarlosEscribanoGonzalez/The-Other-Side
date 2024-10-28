using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogCager : MonoBehaviour
{
    private bool caged = false;
    private bool added = false; //Indica que la nota al cuaderno ya ha sido añadida
    [SerializeField] private WolfBehaviour wolf;
    [SerializeField] private Transform cageTransform;
    private Vector3 initialPos;
    private Quaternion initialRot;

    private void Start()
    {
        initialPos = wolf.initialPosition;
        initialRot = wolf.initialRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        Speaker speaker = other.GetComponent<Speaker>();
        if (speaker != null)
        {
            if (!speaker.IsPlaying()) return; //Sólo funciona si está activo el altavoz
            if(!added) {Notepad.AddMaxDisplay(); added=true;}
            caged = true;
            wolf.initialPosition = cageTransform.position;
            wolf.initialRotation = cageTransform.rotation;
        }
    }

    public void SpeakerTaken()
    {
        if (!caged) return;
        caged = false;
        wolf.initialPosition = initialPos;
        wolf.initialRotation = initialRot;
    }
}
