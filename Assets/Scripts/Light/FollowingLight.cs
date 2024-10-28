using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingLight : MonoBehaviour
{
    [SerializeField] private Transform lightTransform;
    private Transform playerTransform;
    private bool playerInArea = false;

    void Awake()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        GameObject.FindObjectOfType<SideChanger>().sideChanged += DisableFollow;
    }

    void Update()
    {
        if (!playerInArea) return;
        Vector3 newPos = new Vector3(playerTransform.position.x, lightTransform.position.y, playerTransform.position.z);
        lightTransform.position = Vector3.Lerp(lightTransform.position, newPos, 30 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInArea = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInArea = false;
    }

    private void DisableFollow(object s, EventArgs e)
    {
        playerInArea = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<Collider>().enabled = true;
    }
}
