using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int numSlots = 3;
    PlayerMovement player;
    private int currentSlot = 0;
    private ITakeableItem[] objects;
    private int numObjects = 0;
    
    void Awake()
    {
        player = GameObject.FindObjectOfType<PlayerMovement>();
        objects = new ITakeableItem[numSlots];
    }


    void Update()
    {
        if (!player.GetCanMove()) return;
        int scroll = Mathf.RoundToInt(Input.GetAxis("Scroll") * 10f);
        if(scroll != 0) ChangeObject(scroll);
    }

    public bool HasSpace()
    {
        return numObjects < 3;
    }

    public void AddObject(ITakeableItem item)
    {
        for(int i = 0; i < numSlots; i++)
        {
            if (objects[i] == null)
            {
                item.Take();
                objects[i] = item;
                numObjects++;
                if (objects[currentSlot] == null) currentSlot = i;
                if (i != currentSlot) item.GetGameObject().SetActive(false);
                return;
            }
        }
    }

    public void ThrowObject()
    {
        if (numObjects <= 0) return;
        if (objects[currentSlot] != null)
        {
            objects[currentSlot].Throw();
            objects[currentSlot] = null;
            numObjects--;
        }
    }

    private void ChangeObject(int scroll)
    {
        if (objects[currentSlot] != null) objects[currentSlot].GetGameObject().SetActive(false);
        currentSlot += scroll;
        if (currentSlot >= numSlots) currentSlot = 0;
        else if (currentSlot < 0) currentSlot = numSlots - 1;
        if (objects[currentSlot] != null) objects[currentSlot].GetGameObject().SetActive(true);
    }
}
