using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfSpawner : MonoBehaviour
{
    [SerializeField] private GameObject wolf;
    [SerializeField] private GameObject prevWolf;
    [SerializeField] private GameObject navObstacle; //Activa un obstáculo para definir un nuevo área de movimiento del lobo

    private void Awake()
    {
        if(navObstacle != null) navObstacle.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            wolf.SetActive(true);
            if(prevWolf != null) prevWolf.SetActive(false);
            if(navObstacle != null) navObstacle.SetActive(true);
            GetComponent<Collider>().enabled = false;
            GameObject.FindObjectOfType<AudioManager>().UpdateSources(wolf.GetComponent<AudioSource>());
        }
    }
}
