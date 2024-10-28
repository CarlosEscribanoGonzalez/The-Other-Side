using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teddy : ATakeableObject
{
    public bool taken = false;

    public override void Take()
    {
        base.Take();
        taken = true;
    }
    protected override void Use() //Este objeto genera también ondas invisibles
    {
        source.Play();
        echoGenerator.GenerateEcho(source.volume/2);
    }
}
