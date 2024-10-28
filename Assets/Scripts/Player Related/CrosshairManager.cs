using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairManager : MonoBehaviour
{
    [SerializeField] private GameObject defaultCrosshair;
    [SerializeField] private GameObject handCrosshair;
    private PickUpController pickUpController;

    private void Awake()
    {
        pickUpController = GameObject.FindObjectOfType<PickUpController>();
    }

    void Update()
    {
        if (pickUpController.IsItemOnSight())
        {
            handCrosshair.SetActive(true);
            defaultCrosshair.SetActive(false);
        }
        else
        {
            handCrosshair.SetActive(false);
            defaultCrosshair.SetActive(true);
        }
    }
}
