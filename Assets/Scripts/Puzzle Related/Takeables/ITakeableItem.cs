using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITakeableItem
{
    public void HoverEnter();

    public void HoverExit();

    public void Take();

    public void Throw();

    public void TryUse();

    public GameObject GetGameObject();
}
