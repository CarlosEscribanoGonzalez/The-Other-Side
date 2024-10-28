using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallPreventer : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private float tpDistance;

    void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = new Vector3(player.transform.position.x, transform.position.y + tpDistance, player.transform.position.z);
            player.GetComponent<CharacterController>().enabled = true;
        }
    }
}
