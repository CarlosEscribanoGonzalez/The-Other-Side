using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCloser : MonoBehaviour
{
    [SerializeField] private Transform[] doors;
    [SerializeField] private GameObject[] DestroyableObjects;
    [SerializeField] private float timer = 0;
    private bool closeDoor = false;
    private bool audioReproduced = false;

    private void Update()
    {
        if (!closeDoor) return;
        foreach(Transform door in doors)
        {
            door.localRotation = Quaternion.Slerp(door.localRotation, Quaternion.identity, 10*Time.deltaTime);
            if (Quaternion.Dot(door.localRotation,Quaternion.identity) > 0.90)
            {
                if (!audioReproduced)
                {
                    door.GetComponent<AudioSource>().Play();
                    door.GetComponent<EchoGenerator>().GenerateEcho(0.5f);
                }
                audioReproduced = true;
                Destroy(this.gameObject, 0.9f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Invoke("CloseDoor", timer);
            if (DestroyableObjects.Length == 0) return;
            foreach (GameObject o in DestroyableObjects)
            {
                Destroy(o);
            }
        }
    }

    private void CloseDoor()
    {
        closeDoor = true;
    }
}
